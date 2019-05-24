using Android.OS;
using Android.App;
using Android.Views;
using Android.Widget;
using Android.Support.V7.App;
using static Android.Views.View;
using Microsoft.AspNetCore.SignalR.Client;

namespace SignalRCoreXamarinClient
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity, IOnTouchListener
    {
        Button btn_start;
        View view_move;

        HubConnection hubConnection;

        public bool OnTouch(View v, MotionEvent e)
        {
            view_move.SetX(e.RawX);
            view_move.SetY(e.RawY);

            if (hubConnection.State == HubConnectionState.Connected)
            {
                hubConnection.SendAsync("MoveViewFromServer", e.RawX, e.RawY);
            }

            return true;
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

            btn_start = FindViewById<Button>(Resource.Id.btn_start);
            view_move = FindViewById<View>(Resource.Id.view_move);

            //"IP Url" generated/proxy by "Conveyor" plugin (https://bit.ly/2pHjn2u) to access Hub from emulators and devices...
            hubConnection = new HubConnectionBuilder().WithUrl("http://192.168.15.18:45455/movehub").Build();

            hubConnection.On<float, float>("ReceiveNewPosition", (newX, newY) =>
            {
                view_move.SetX(newX);
                view_move.SetY(newY);
            });

            btn_start.Click += async delegate
            {
                if (btn_start.Text.ToLower().Equals("start"))
                {
                    if (hubConnection.State == HubConnectionState.Disconnected)
                    {
                        try
                        {
                            await hubConnection.StartAsync();
                            btn_start.Text = "STOP";
                        }
                        catch (System.Exception ex)
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
                        catch (System.Exception)
                        {

                        }
                    }
                }
            };

            view_move.SetOnTouchListener(this);
        }
    }
}