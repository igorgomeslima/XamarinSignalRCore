using System;
using Core.Utils;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace Core.Hubs
{
    public class MoveViewHub : Hub
    {
        public async Task MoveViewFromServer(float newX, float newY)
        {
            Console.WriteLine($"Receive position from Server app: {newX}/{newY}");

            await Clients.Others.SendAsync(nameof(SignalRMethodName.ReceiveNewPosition), newX, newY);
        }
    }
}
