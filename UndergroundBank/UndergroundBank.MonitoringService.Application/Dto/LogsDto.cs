using System.Text.Json.Nodes;

namespace UndergroundBank.MonitoringService.Application.Dto
{
    public class LogsDto
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public DateTime Timestamp { get; set; }

        public string? Level { get; set; }

        public string? Message { get; set; }

        public string? TraceId { get; set; }

        public string? SpanId { get; set; }

        public string? ServiceName { get; set; }

        public JsonObject? Attributes { get; set; }
    }
}
