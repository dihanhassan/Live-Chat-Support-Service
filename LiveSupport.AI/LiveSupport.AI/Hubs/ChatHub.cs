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

        private readonly Dependency _dependency;
        public ChatHub(
            Dependency dependency
            )
        {
            _dependency = dependency;
        }

        public async Task<string> JoinRoom(UserConnection userConnection)
        {
            try
            {
                string roomID = null;

                if (userConnection.IsAdmin)
                {
                    if (!_dependency._adminConnection.ContainsKey(userConnection.Email))
                    {
                        _dependency._adminConnection[userConnection.Email] = new List<string>();
                    }
                    foreach (var room in _dependency._adminConnection[userConnection.Email])
                    {
                        await Groups.AddToGroupAsync(Context.ConnectionId, room);
                    }
                    _dependency._connections[Context.ConnectionId] = userConnection;
                    await SendConnectedUsers(userConnection.Email);
                }
                else
                {
                    _dependency._connections[Context.ConnectionId] = userConnection;
                    roomID = await GetOrCreateRoomId(userConnection);
                    await Groups.AddToGroupAsync(Context.ConnectionId, roomID);

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
            }catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        private async Task<string> GetOrCreateRoomId(UserConnection userConnection)
        {
            try
            {
                if (!_dependency._userRoom.TryGetValue(userConnection.Email, out var roomID))
                {
                    roomID = Guid.NewGuid().ToString();
                    _dependency._userRoom[userConnection.Email] = roomID;

                    if (SD._siteAdmin.TryGetValue(userConnection.SiteId, out var admins))
                    {
                        foreach (var admin in admins)
                        {
                            if (!_dependency._adminConnection.ContainsKey(admin))
                            {
                                _dependency._adminConnection[admin] = new List<string>();
                            }
                            _dependency._adminConnection[admin].Add(roomID);
                            var adminContextId = _dependency._connections.FirstOrDefault(x => admin == x.Value.Email && x.Value.IsAdmin == true).Key??""; // when admin context is is not found send empty string
                            await Groups.AddToGroupAsync(adminContextId, roomID);
                            await SendConnectedUsers(admin);
                        }
                    }
                }
                return roomID;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        private async Task NotifyAdminsOfNewUser(UserConnection userConnection, string roomID)
        {
            try
            {
                var admins = _dependency._adminConnection.Where(kvp => kvp.Value.Contains(roomID)).Select(kvp => kvp.Key).ToList();
                foreach (var admin in admins)
                {
                    await SendConnectedUsers(admin);
                }
            }catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            try
            {
                if (_dependency._connections.TryGetValue(Context.ConnectionId, out var userConnection))
                {
                    if (_dependency._userRoom.TryGetValue(userConnection.Email, out var roomId))
                    {
                        foreach (var admin in _dependency._adminConnection.Keys.ToList())
                        {
                            var adminRooms = _dependency._adminConnection[admin];
                            if (adminRooms.Contains(roomId))
                            {
                                adminRooms.Remove(roomId);
                                await SendConnectedUsers(admin);
                            }
                        }
                        _dependency._userRoom.TryRemove(userConnection.Email, out _);
                    }
                    _dependency._connections.TryRemove(Context.ConnectionId, out _);
                }
                await base.OnDisconnectedAsync(exception);
            }catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public async Task SendMessageByAdmin (PrivateMessage messages, UserConnection userConnection)
        {
            try
            {
                if (messages.Message is not null)
                {
                    var message = new Message();
                    message.User = "admin";
                    message.Email = userConnection.Email;
                    message.Text = messages.Message;
                    message.Time = DateTime.Now;
                    message.Room = messages.RoomID;
                    await Clients.Group(messages.RoomID).SendAsync("ReceiveMessage", message);
                }
            }catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public async Task SendMessage(PrivateMessage privateMessage )
        {
            try
            {
                if (_dependency._connections.TryGetValue(Context.ConnectionId, out var userConnection))
                {
                    if (userConnection.IsAdmin)
                    {
                        await SendMessageByAdmin(privateMessage, userConnection);
                    }
                    else
                    {
                        if (_dependency._userRoom.TryGetValue(userConnection.Email, out var roomID))
                        {
                            var message = new Message();
                            message.User = userConnection.Name;
                            message.Email = userConnection.Email;
                            message.Text = privateMessage.Message;
                            message.Time = DateTime.Now;
                            message.Room = privateMessage.RoomID;
                            await Clients.Group(privateMessage.RoomID).SendAsync("ReceiveMessage", message);
                        }
                    }
                }
            }catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }


        public async Task RemoveOtherAdminFromRoom(RemoveOtherAdmins otherAdmins)
        {
            try
            {
                   /* for (int i = 0; i < SD._siteAdmin[otherAdmins.SiteId].Count; ++i)
                    {
                        var mail = SD._siteAdmin[otherAdmins.SiteId][i];
                        if (mail != otherAdmins.AdminMail)
                        {
                            for (int j = 0; j < _dependency._adminConnection[mail].Count; ++j)
                            {
                                if (_dependency._adminConnection[mail][j] == otherAdmins.RoomID)
                                {
                                    // Groups.Remove(Context.ConnectionId, roomName);
                                    _dependency._adminConnection[mail].RemoveAt(j);
                                    await SendConnectedUsers(mail);
                                }
                            }
                        }
                    }*/

               

                if(SD._siteAdmin.TryGetValue(otherAdmins.SiteId,out var admins))
                {
                    foreach(var admin in admins)
                    {
                        if( admin !=otherAdmins.AdminMail && _dependency._adminConnection.ContainsKey(admin))
                        {
                            if (_dependency._adminConnection[admin].Contains(otherAdmins.RoomID))
                            {
                                _dependency._adminConnection[admin].Remove(otherAdmins.RoomID);
                                await SendConnectedUsers(admin);
                            }
                        }
                    }
                }




            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }


        public async Task SendConnectedUsers(string admin)
        {
            try
            {
                var adminConnectionId = _dependency._connections
                    .Where(x => x.Value.Email == admin && x.Value.IsAdmin)
                    .Select(x => x.Key)
                    .FirstOrDefault();

                if (adminConnectionId != null && _dependency._adminConnection.TryGetValue(admin, out var roomIDs))
                {
                    var connectedUsersEmail = new List<string>();
                    var connectedUsers = new List<Tuple<string, string, string>>();
                   /* foreach (var roomID in roomIDs)
                    {
                        for (int i = 0; i < _dependency._userRoom.Count; i++)
                        {
                            if (_dependency._userRoom.ElementAt(i).Value == roomID)
                            {

                                connectedUsersEmail.Add(_dependency._userRoom.ElementAt(i).Key);
                            }
                        }
                    }*/


                    roomIDs.ForEach(roomId =>

                    {
                        _dependency._userRoom.ToList().ForEach(x =>
                        {
                            if (x.Value == roomId)
                            {
                                connectedUsersEmail.Add(x.Key);
                            }
                        });
                    });



                    foreach (var userEmail in connectedUsersEmail)
                    {
                        //  var userConnection = _connections.Values.FirstOrDefault(x => userEmail == x.Email && x.IsAdmin ==false ).Name;
                        for (int i = 0; i < _dependency._connections.Count; i++)
                        {
                            if (_dependency._connections.ElementAt(i).Value.Email == userEmail && _dependency._connections.ElementAt(i).Value.IsAdmin == false)
                            {
                                connectedUsers.Add(new Tuple<string, string, string>(_dependency._connections.ElementAt(i).Value.Name, _dependency._userRoom[_dependency._connections.ElementAt(i).Value.Email], _dependency._connections.ElementAt(i).Value.Email));
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
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
