using LiveSupport.AI.Models;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LiveSupport.AI.Models;
namespace LiveSupport.AI.Hubs
{
    public class ChatHub : Hub
    {
        private readonly IDictionary<string, UserConnection> _connections;

        public ChatHub(IDictionary<string, UserConnection> connections)
        {
            _connections = connections;
        }

        public async Task JoinRoom(UserConnection userConnection)
        {



            await Groups.AddToGroupAsync(Context.ConnectionId, userConnection.Room);
            _connections[Context.ConnectionId] = userConnection;
            if (!userConnection.IsAdmin)
            {
                await Clients.Group(userConnection.Room)
                .SendAsync("ReceiveMessage", "Bot", $"{userConnection.Name} has joined the group", DateTime.Now);
            }
            
            await SendConnectedUsers(userConnection.Room);
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            if (_connections.TryGetValue(Context.ConnectionId, out var userConnection))
            {
                await Clients.Group(userConnection.Room)
                    .SendAsync("ReceiveMessage", "Bot", $"{userConnection.Name} has left the group");
                _connections.Remove(Context.ConnectionId);
                await SendConnectedUsers(userConnection.Room);
            }
            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(string message)
        {
            if (_connections.TryGetValue(Context.ConnectionId, out var userConnection))
            {
                await Clients.Group(userConnection.Room).SendAsync("ReceiveMessage", userConnection.Name, message, DateTime.Now);
            }
        }

        public Task SendConnectedUsers(string room)
        {
            var users = _connections.Values
                .Where(ur => ur.Room == room && ur.IsAdmin==false)
                .Select(s => s.Name)
                .ToList();
            return Clients.Group(room).SendAsync("ConnectedUsers", users);
        }

        

    }
}

