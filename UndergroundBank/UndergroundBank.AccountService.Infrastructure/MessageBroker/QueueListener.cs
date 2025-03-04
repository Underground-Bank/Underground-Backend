using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EasyNetQ;
using Microsoft.Extensions.DependencyInjection;
using UndergroundBank.AccountService.Application.Interfaces;
using UndergroundBank.Common.Dto.AccountService;

namespace UndergroundBank.AccountService.Infrastructure.MessageBroker
{
    public static class QueueListener
    {
        public static void QueueSubscribe(this IServiceCollection services)
        {
            var serviceProvider = services.BuildServiceProvider();
            var bus = RabbitHutch.CreateBus("host=localhost");
            var profileService = serviceProvider.GetRequiredService<IProfileService>();

            bus.Rpc.Respond<Guid, ProfileDto>(
                async request =>
                {
                    return await profileService.GetUserProfile(request.ToString());
                },
                x => x.WithQueueName("bank_UserProfileResponse")
            );
        }
    }
}
