using Calculator.Kafka.Infrastructure.Interfaces;
using Calculator.Kafka.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Concurrent;

namespace Calculator.Kafka.API.Controllers;

// Example of how to handle responses in a real system
[ApiController]
[Route("api/[controller]")]
public class ResultsController : ControllerBase
{
    // In a real system, this would be a database or cache
    private static readonly ConcurrentDictionary<Guid, CalculationResponse> _results = new();
    private readonly ILogger<ResultsController> _logger;

    public ResultsController(ILogger<ResultsController> logger)
    {
        _logger = logger;
    }

    [HttpGet("{requestId}")]
    public async Task<IActionResult> GetResult(Guid requestId)
    {
        if (_results.TryGetValue(requestId, out var result))
        {
            return Ok(result);
        }

        return NotFound(new { Message = "Result not ready yet or request not found" });
    }

    [HttpGet("{requestId}/wait")]
    public async Task<IActionResult> WaitForResult(Guid requestId, CancellationToken cancellationToken)
    {
        // Long polling - wait up to 30 seconds for result
        var timeout = TimeSpan.FromSeconds(30);
        var start = DateTime.UtcNow;

        while (DateTime.UtcNow - start < timeout && !cancellationToken.IsCancellationRequested)
        {
            if (_results.TryGetValue(requestId, out var result))
            {
                return Ok(result);
            }

            await Task.Delay(1000, cancellationToken); // Check every second
        }

        return Accepted(new { Message = "Result not ready yet, please try again" });
    }

    // This would be called by a background service that consumes from calculation-responses topic
    public static void StoreResult(CalculationResponse response)
    {
        _results.TryAdd(response.Id, response);
    }
}