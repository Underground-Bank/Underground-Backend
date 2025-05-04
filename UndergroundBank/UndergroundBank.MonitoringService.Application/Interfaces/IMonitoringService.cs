using UndergroundBank.MonitoringService.Application.Dto;

namespace UndergroundBank.MonitoringService.Application.Interfaces;

public interface IMonitoringService
{
    public Task AddLogs(LogsDto logsDto);
    public Task AddTrace(TraceDto traceDto);
}

