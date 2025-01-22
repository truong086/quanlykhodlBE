using Microsoft.AspNetCore.SignalR;
using Vonage.Users;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace quanlykhodl.ChatHub
{
    public class NotificationHub : Hub
    {
        private readonly onlineUser _onlineUserService;
        public NotificationHub(onlineUser onlineUserService)
        {
            _onlineUserService = onlineUserService;
        }
        public async Task SendNotification(string message)
        {
            await Clients.All.SendAsync("ReceiveNotification", message);
        }

        // Hàm này nghĩa là khi chúng ta vừa Login xong có nghĩa là chúng ta chuẩn bị Connect thì sẽ gọi hàm này "OnConnectedAsync()" (Hàm này Client sẽ gọi)
        public override async Task OnConnectedAsync()
        {
            // Lấy thông tin từ query string
            string name = Context.GetHttpContext()?.Request.Query["name"];
            string avatarUrl = Context.GetHttpContext()?.Request.Query["avatarUrl"];

            // Thêm người dùng vào danh sách
            _onlineUserService.AddUser(Context.ConnectionId, 1, name ?? "Guest", avatarUrl ?? "/images/default-avatar.png");

            // Gửi danh sách người dùng mới cho tất cả client
            await Clients.All.SendAsync("UpdateOnlineUsers", _onlineUserService.GetOnlineUsers());
            await Clients.Caller.SendAsync("getProfileInfo", "Test");
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            // Xóa người dùng khỏi danh sách
            _onlineUserService.RemoveUser(Context.ConnectionId);

            // Gửi danh sách người dùng mới cho tất cả client
            await Clients.All.SendAsync("UpdateOnlineUsers", _onlineUserService.GetOnlineUsers());
            await Clients.Caller.SendAsync("onError", "OnDisconnected:");

            await base.OnDisconnectedAsync(exception);
            
        }
    }
}
