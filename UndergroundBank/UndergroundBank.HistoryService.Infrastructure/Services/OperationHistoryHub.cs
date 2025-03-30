using Microsoft.AspNetCore.SignalR;
using UndergroundBank.Common.Data.Constants;
using UndergroundBank.HistoryService.Application.DTO;

namespace UndergroundBank.HistoryService.Infrastructure.Services
{
    public class OperationHistoryHub : Hub
    {
        public async Task NotifyNewTransaction(OperationsHistoryDto operationHistoryElement)
        {
            await Clients.All.SendAsync(WebSockets.TRANSACTION_UPDATED, operationHistoryElement);
        }
    }
}
