using AutoMapper;
using UndergroundBank.Common.Middlewares;
using UndergroundBank.MonitoringService.Application.Dto;
using UndergroundBank.MonitoringService.Application.Interfaces;
using UndergroundBank.MonitoringService.Domain.Entities;
using UndergroundBank.MonitoringService.Infrastructure;


namespace UndergroundBank.HistoryService.Infrastructure.Services;

public class MonitorService : IMonitoringService
{
    private readonly MonitoringDbContext _dbContext;
    private readonly IMapper _mapper;

    public MonitorService(
        MonitoringDbContext dbContext,
        IMapper mapper
    )
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task AddTrace(TraceDto traceDto)
    {
        if (traceDto == null)
        {
            throw new BadRequestException("Incorrect format of trace");
        }
        var trace = _mapper.Map<TraceSpan>(traceDto);
        await _dbContext.AddAsync(trace);
    }

    public async Task AddLogs(LogsDto logsDto)
    {
        if (logsDto == null)
        {
            throw new BadRequestException("Incorrect format of logs");
        }
        var log = _mapper.Map<LogEntry>(logsDto);
        await _dbContext.AddAsync(log);
    }

}

