using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Nodes;

namespace UndergroundBank.MonitoringService.Domain.Entities;

public class LogEntry
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    public DateTime Timestamp { get; set; }

    public string? Level { get; set; }

    public string? Message { get; set; }

    public string? TraceId { get; set; }

    public string? SpanId { get; set; }

    public string? ServiceName { get; set; }

    [Column(TypeName = "jsonb")]
    public JsonObject? Attributes { get; set; }
}

