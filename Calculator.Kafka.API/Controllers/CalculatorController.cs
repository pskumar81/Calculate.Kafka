using Calculator.Kafka.Infrastructure.Interfaces;
using Calculator.Kafka.Models;
using Microsoft.AspNetCore.Mvc;

namespace Calculator.Kafka.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CalculatorController : ControllerBase
{
    private readonly IKafkaProducer _kafkaProducer;
    private readonly ILogger<CalculatorController> _logger;

    public CalculatorController(IKafkaProducer kafkaProducer, ILogger<CalculatorController> logger)
    {
        _kafkaProducer = kafkaProducer;
        _logger = logger;
    }

    [HttpPost("calculate")]
    public async Task<IActionResult> Calculate([FromBody] CalculationRequest request)
    {
        try
        {
            if (request == null)
            {
                return BadRequest("Request cannot be null");
            }

            if (string.IsNullOrEmpty(request.Operation))
            {
                return BadRequest("Operation is required");
            }

            var validOperations = new[] { "add", "subtract", "multiply", "divide" };
            if (!validOperations.Contains(request.Operation.ToLower()))
            {
                return BadRequest("Invalid operation. Supported operations: add, subtract, multiply, divide");
            }

            if (request.Operation.ToLower() == "divide" && request.SecondNumber == 0)
            {
                return BadRequest("Division by zero is not allowed");
            }

            request.Id = Guid.NewGuid();
            request.RequestTime = DateTime.UtcNow;

            await _kafkaProducer.SendCalculationRequestAsync(request);

            _logger.LogInformation("Calculation request {Id} sent for processing", request.Id);

            return Accepted(new { RequestId = request.Id, Message = "Calculation request submitted for processing" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing calculation request");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("health")]
    public IActionResult Health()
    {
        return Ok(new { Status = "Healthy", Timestamp = DateTime.UtcNow });
    }
}