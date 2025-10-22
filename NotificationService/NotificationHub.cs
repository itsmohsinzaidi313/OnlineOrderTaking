using Microsoft.AspNetCore.SignalR;
using PointofSaleModels;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using System.Threading.Channels;

namespace NotificationService
{
    public class NotificationHub : Hub
    {



        public void RequestRestaurantMenu()
        {

        }

        public void SendRestaurantMenu(string connectionId, object menu)
        {
            Clients.Client(connectionId).SendAsync("MenuRequest", menu);
        }
    }
}
