using Microsoft.AspNetCore.Mvc;

public class NotificationController : ControllerBase
{
    private readonly NotificationService _notificationService;

    public NotificationController(NotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    [HttpPost("send-notification")]
    public async Task<IActionResult> SendNotification(string deviceToken, string title, string body)
    {
        await _notificationService.SendNotificationAsync(deviceToken, title, body);
        return Ok();
    }
}
