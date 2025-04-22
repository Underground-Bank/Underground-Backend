using AutoMapper;
using UndergroundBank.MonitoringService.Application.Dto;
using UndergroundBank.MonitoringService.Domain.Entities;

namespace UndergroundBank.MonitoringService.Application.Helpers.Automapper
{
    public class MonitoringServiceMapper : Profile
    {
        public MonitoringServiceMapper()
        {
            CreateMap<LogsDto, LogEntry>().ReverseMap();
            CreateMap<TraceDto, TraceSpan>().ReverseMap();
        }
    }
}
