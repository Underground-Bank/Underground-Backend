using EasyNetQ;
using Microsoft.Extensions.DependencyInjection;
using UndergroundBank.Common.Data.Constants;
using UndergroundBank.Common.Dto.AccountService;
using UndergroundBank.Common.Dto.Notification;
using UndergroundBank.Common.Dto.Transaction;

public static class QueueListener
{
    public static void QueueSubscribe(this IServiceCollection services)
    {
        var serviceProvider = services.BuildServiceProvider();
        var bus = RabbitHutch.CreateBus("host=localhost");
        var notifService = serviceProvider.GetRequiredService<NotificationService>();

        bus.PubSub.Subscribe<NotificationDto>(
            Queues.NOTIFICATION_SEND,
            data => notifService.SendNotificationAsync(data)
        );
    }
}
