using LiveSupport.AI.Models;
using System.Collections.Concurrent;

namespace LiveSupport.AI.Data
{
    public class Dependency
    {
        public readonly ConcurrentDictionary<string, UserConnection> _connections; // ConnectionId, UserConnection
        public readonly ConcurrentDictionary<string, string> _userRoom; // Email, RoomId
        public readonly ConcurrentDictionary<string, List<string>> _adminConnection; // Email, List<RoomId>
        public Dependency()
        {
            _connections = new();
            _userRoom = new();
            _adminConnection = new();

        }
    }
}
