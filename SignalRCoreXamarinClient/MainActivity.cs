using System;
using Android.OS;
using Core.Utils;
using Android.App;
using Java.Interop;
using Android.Views;
using Android.Widget;
using Android.Support.V7.App;
using Microsoft.AspNetCore.SignalR.Client;

using static Android.Views.View;

namespace SignalRCoreXamarinClient
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity, IOnTouchListener
    {
        View view_move;

        HubConnection hubConnection;

        public bool OnTouch(View v, MotionEvent e)
        {
            view_move.SetX(e.RawX);
            view_move.SetY(e.RawY);

            if (hubConnection.State == HubConnectionState.Connected)
            {
                hubConnection.SendAsync(nameof(SignalRMethodName.MoveViewFromServer), e.RawX, e.RawY);
            }

            return true;
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

            view_move = FindViewById<View>(Resource.Id.view_move);

            // "IP Url" generated/proxy by "Conveyor by Keyoti" plugin (https://bit.ly/2pHjn2u)
            // to access Hub from emulators and devices...
            hubConnection = new HubConnectionBuilder()
                .WithUrl("http://192.168.15.18:45458/movehub")
                .Build();

            hubConnection.On<float, float>(nameof(SignalRMethodName.ReceiveNewPosition), ReceiveNewPosition);

            view_move.SetOnTouchListener(this);
        }

        void ReceiveNewPosition(float newX, float newY)
        {
            view_move.SetX(newX);
            view_move.SetY(newY);
        }

        [Export(nameof(On_btn_start_Click))]
        public async void On_btn_start_Click(View view)
        {
            var btn_start = (Button)view;

            if (btn_start.Text.ToLower().Equals("start"))
            {
                if (hubConnection.State == HubConnectionState.Disconnected)
                {
                    try
                    {
                        await hubConnection.StartAsync();
                        btn_start.Text = "STOP";
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
            else if (btn_start.Text.ToLower().Equals("stop"))
            {
                if (hubConnection.State == HubConnectionState.Connected)
                {
                    try
                    {
                        await hubConnection.StopAsync();
                        btn_start.Text = "START";
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
        }
    }
}