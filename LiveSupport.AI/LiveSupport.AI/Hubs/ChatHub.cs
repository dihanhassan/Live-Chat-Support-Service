using LiveSupport.AI.Data;
using LiveSupport.AI.Models;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LiveSupport.AI.Hubs
{
    public class ChatHub : Hub
    {
        private readonly IDictionary<string, UserConnection> _connections;
        private readonly IDictionary<string, string> _userRoom;
        private readonly IDictionary<string, List<string>> _adminConnection;

        public ChatHub(
            IDictionary<string, UserConnection> connections,
            IDictionary<string, string> userRoom,
            IDictionary<string, List<string>> adminConnection)
        {
            _connections = connections;
            _userRoom = userRoom;
            _adminConnection = adminConnection;
        }

        public async Task<string> JoinRoom(UserConnection userConnection)
        {
            string roomID = null;

            if (userConnection.IsAdmin)
            {
                if (!_adminConnection.ContainsKey(userConnection.Email))
                {
                    _adminConnection[userConnection.Email] = new List<string>();
                }
                foreach (var room in _adminConnection[userConnection.Email])
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, room);
                }
                _connections[Context.ConnectionId] = userConnection;
                await SendConnectedUsers(userConnection.Email);
            }
            else
            {
                roomID = GetOrCreateRoomId(userConnection);
                await Groups.AddToGroupAsync(Context.ConnectionId, roomID);
                _connections[Context.ConnectionId] = userConnection;
                await NotifyAdminsOfNewUser(userConnection, roomID);
                var message = new Message();
                message.User = "Bot";
                message.Email = userConnection.Email;
                message.Text = $"{userConnection.Name} has joined the group";
                message.Time = DateTime.Now;

                await Clients.Group(roomID).SendAsync("ReceiveMessage", message);
                await SendConnectedUsers(userConnection.Email);
            }

            return roomID;
        }

        private string GetOrCreateRoomId(UserConnection userConnection)
        {
            if (!_userRoom.TryGetValue(userConnection.Email, out var roomID))
            {
                roomID = Guid.NewGuid().ToString();
                _userRoom[userConnection.Email] = roomID;

                if (SD._siteAdmin.TryGetValue(userConnection.SiteId, out var admins))
                {
                    foreach (var admin in admins)
                    {
                        if (!_adminConnection.ContainsKey(admin))
                        {
                            _adminConnection[admin] = new List<string>();
                        }
                        _adminConnection[admin].Add(roomID);
                        SendConnectedUsers(admin);
                    }
                }
            }
            return roomID;
        }

        private async Task NotifyAdminsOfNewUser(UserConnection userConnection, string roomID)
        {
            var admins = _adminConnection.Where(kvp => kvp.Value.Contains(roomID)).Select(kvp => kvp.Key).ToList();
            foreach (var admin in admins)
            {
                await SendConnectedUsers(admin);
            }
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            if (_connections.TryGetValue(Context.ConnectionId, out var userConnection))
            {
                if (_userRoom.TryGetValue(userConnection.Email, out var roomId))
                {
                    foreach (var admin in _adminConnection.Keys.ToList())
                    {
                        var adminRooms = _adminConnection[admin];
                        if (adminRooms.Contains(roomId))
                        {
                            adminRooms.Remove(roomId);
                            await SendConnectedUsers(admin);
                        }
                    }
                    _userRoom.Remove(userConnection.Email);
                }
                _connections.Remove(Context.ConnectionId);
            }
            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessageByAdmin (PrivateMessage messages, UserConnection userConnection)
        {
            if(messages.Message is not null)
            {
                var message = new Message();
                message.User = "admin";
                message.Email = userConnection.Email;
                message.Text = messages.Message;
                message.Time = DateTime.Now;
                message.Room = messages.RoomID;
                await Clients.Group(messages.RoomID).SendAsync("ReceiveMessage", message);
            }
        }

        public async Task SendMessage(PrivateMessage privateMessage )
        {
            if (_connections.TryGetValue(Context.ConnectionId, out var userConnection))
            {
                if (userConnection.IsAdmin)
                {
                    await SendMessageByAdmin(privateMessage, userConnection);
                }
                else
                {
                    if (_userRoom.TryGetValue(userConnection.Email, out var roomID))
                    {
                        var message = new Message();
                        message.User =userConnection.Name;
                        message.Email = userConnection.Email;
                        message.Text = privateMessage.Message;
                        message.Time = DateTime.Now;
                        message.Room = privateMessage.RoomID;
                        await Clients.Group(privateMessage.RoomID).SendAsync("ReceiveMessage", message);
                    }
                }
            }
        }

        public async Task SendConnectedUsers(string admin)
        {
            var adminConnectionId = _connections
                .Where(x => x.Value.Email == admin && x.Value.IsAdmin)
                .Select(x => x.Key)
                .FirstOrDefault();

            if (adminConnectionId != null && _adminConnection.TryGetValue(admin, out var roomIDs))
            {
                var connectedUsersEmail = new List<string>();
                var connectedUsers = new List<Tuple<string, string,string>>();
                foreach (var roomID in roomIDs)
                {
                    for (int i = 0; i < _userRoom.Count; i++)
                    {
                        if (_userRoom.ElementAt(i).Value == roomID )
                        {
                          
                            connectedUsersEmail.Add(_userRoom.ElementAt(i).Key);
                        }
                    }
                }
                foreach (var userEmail in connectedUsersEmail)
                {
                    //  var userConnection = _connections.Values.FirstOrDefault(x => userEmail == x.Email && x.IsAdmin ==false ).Name;
                    for (int i = 0; i < _connections.Count; i++)
                    {
                        if (_connections.ElementAt(i).Value.Email == userEmail && _connections.ElementAt(i).Value.IsAdmin == false)
                        {
                            connectedUsers.Add(new Tuple<string,string,string>(_connections.ElementAt(i).Value.Name,_userRoom[_connections.ElementAt(i).Value.Email], _connections.ElementAt(i).Value.Email));
                        }
                    }

                }

                await Clients.Client(adminConnectionId).SendAsync("ConnectedUsers", connectedUsers);
            }
            else if (adminConnectionId != null)
            {
                await Clients.Client(adminConnectionId).SendAsync("ConnectedUsers", new List<string>());
            }
        }
    }
}
