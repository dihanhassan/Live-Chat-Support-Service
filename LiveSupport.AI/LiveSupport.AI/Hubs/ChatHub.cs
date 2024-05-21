using LiveSupport.AI.Models;
using Microsoft.AspNetCore.SignalR;
namespace LiveSupport.AI.Hubs
{
    public class ChatHub : Hub
    {
        private readonly IDictionary<string, UserRoomConnection> _connection;
        public ChatHub(IDictionary<string, UserRoomConnection> connection)
        {
            _connection = connection;
        }
        public async Task JoinRoom(UserRoomConnection userConnection)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, userConnection.Room);
            _connection.Add(Context.ConnectionId, userConnection);
            await Clients.Group(userConnection.Room!)
                .SendAsync("ReceiveMessage", "Bot",$"{userConnection.User} Has Join The Group",DateTime.Now);
            SendConnectedUser(userConnection.Room);
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            if (!_connection.TryGetValue(Context.ConnectionId, out UserRoomConnection userConnection))
            {
                return base.OnDisconnectedAsync(exception);
            }
            Clients.Group(userConnection.Room)
                .SendAsync("ReceiveMessage", "Bot",$"{userConnection.User} Has Left The Group");
           _connection.Remove(Context.ConnectionId);
           SendConnectedUser(userConnection.Room!);
            return base.OnDisconnectedAsync(exception);
            
        }

        public async Task SendMessage (string message)
        {
            if(_connection.TryGetValue(Context.ConnectionId, out UserRoomConnection userConnection))
            {
                await Clients.Groups(userConnection.Room).SendAsync("ReceiveMessage",userConnection.User,message, DateTime.Now);
            }
        }
        public Task SendConnectedUser(string room)
        {
            var users = _connection.Values
                .Where(ur => ur.Room == room)
                .Select(s=> s.User);
            return Clients.Group(room).SendAsync("ConnectedUser",users);
        }
    }
}
 