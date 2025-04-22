using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using UndergroundBank.MonitoringService.Application.Dto;
using UndergroundBank.MonitoringService.Application.Interfaces;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace UndergroundBank.MonitoringService.Web.Controllers;

[ApiController]
[Route("api/monitoring")]
[ProducesResponseType(typeof(Error), 400)]
[ProducesResponseType(typeof(Error), 500)]
public class MonitoringController : ControllerBase
{
    private readonly IMonitoringService _monitoringService;

    public MonitoringController(IMonitoringService monitoringService)
    {
        _monitoringService = monitoringService;
    }

    [HttpPost("traces")]
    public async Task<IActionResult> ReceiveTraces()
    {
        using var reader = new StreamReader(Request.Body);
        var traceBody = await reader.ReadToEndAsync();
        var traceDto = JsonSerializer.Deserialize<LogsDto>(traceBody);
        await _monitoringService.AddLogs(traceDto);
        Console.WriteLine("Trace received:");
        Console.WriteLine(traceBody);
        return Ok();
    }

    [HttpPost("logs")]
    public async Task<IActionResult> ReceiveLogs()
    {
        using var reader = new StreamReader(Request.Body);
        var logsBody = await reader.ReadToEndAsync();
        var logsDto = JsonSerializer.Deserialize<LogsDto>(logsBody);
        await _monitoringService.AddLogs(logsDto);
        Console.WriteLine("Trace received:");
        Console.WriteLine(logsBody);
        return Ok();
    }
}
