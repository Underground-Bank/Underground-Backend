using System.Text.Json.Nodes;

namespace UndergroundBank.MonitoringService.Application.Dto
{
    public class TraceDto
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string TraceId { get; set; } = default!;

        public string SpanId { get; set; } = default!;

        public string? ParentSpanId { get; set; }

        public string? Name { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public string? ServiceName { get; set; }
        public JsonObject? Attributes { get; set; }
    }
}
