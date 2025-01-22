using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using quanlykhodl.ChatHub;

namespace quanlykhodl.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HubController : ControllerBase
    {
        private readonly IHubContext<NotificationHub> _hubContext;
        public HubController(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendNotification([FromBody] string message)
        {
            await _hubContext.Clients.All.SendAsync("ReceiveNotification", message);
            return Ok(new { Message = "Notification sent successfully!" });
        }
    }
}
