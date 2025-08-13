using Microsoft.AspNetCore.Mvc;

namespace NotificationService.Controllers
{
    [ApiController]
    [Route("api/v1")]
    public class NotificationsController : ControllerBase
    {
        [HttpPost("notifications")]
        public IActionResult Send([FromBody] object orderSummary)
        {
            Console.WriteLine($"[NotificationService] Received: {orderSummary}");
            return Ok(new { message = "Notification sent" });
        }
    }
}
