using System.Collections.Concurrent;

namespace quanlykhodl.ChatHub
{
    public class onlineUser
    {
        private readonly ConcurrentDictionary<string, UserInfo> _onlineUsers = new ConcurrentDictionary<string, UserInfo>();

        public class UserInfo
        {
            public int id { get; set; }
            public string? Name { get; set; }
            public string? AvatarUrl { get; set; }
        }

        // Thêm người dùng
        public void AddUser(string connectionId, int id, string name, string avatarUrl)
        {
            _onlineUsers[connectionId] = new UserInfo
            {
                id = id,
                Name = name,
                AvatarUrl = avatarUrl
            };
        }

        public void AddUser(int id, string name, string avatarUrl)
        {
            _onlineUsers[id.ToString()] = new UserInfo
            {
                id = id,
                Name = name,
                AvatarUrl = avatarUrl
            };
        }

        // Xóa người dùng
        public void RemoveUser(string connectionId)
        {
            _onlineUsers.TryRemove(connectionId, out _);
        }

        // Lấy danh sách người dùng
        public List<UserInfo> GetOnlineUsers()
        {
            return _onlineUsers.Values.ToList();
        }
    }
}
