using Calculator.Kafka.Infrastructure.Interfaces;
using Calculator.Kafka.Models;

namespace Calculator.Kafka.Worker;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IKafkaConsumer _kafkaConsumer;
    private readonly IKafkaProducer _kafkaProducer;

    public Worker(ILogger<Worker> logger, IKafkaConsumer kafkaConsumer, IKafkaProducer kafkaProducer)
    {
        _logger = logger;
        _kafkaConsumer = kafkaConsumer;
        _kafkaProducer = kafkaProducer;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Calculator Worker started at: {time}", DateTimeOffset.Now);

        try
        {
            await _kafkaConsumer.StartConsumingRequestsAsync(ProcessCalculationRequest, stoppingToken);
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Calculator Worker is stopping");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in Calculator Worker");
        }
    }

    private async Task ProcessCalculationRequest(CalculationRequest request)
    {
        try
        {
            _logger.LogInformation("Processing calculation request {Id}: {FirstNumber} {Operation} {SecondNumber}", 
                request.Id, request.FirstNumber, request.Operation, request.SecondNumber);

            var response = new CalculationResponse
            {
                Id = request.Id,
                Operation = request.Operation,
                ProcessedTime = DateTime.UtcNow
            };

            try
            {
                response.Result = request.Operation.ToLower() switch
                {
                    "add" => request.FirstNumber + request.SecondNumber,
                    "subtract" => request.FirstNumber - request.SecondNumber,
                    "multiply" => request.FirstNumber * request.SecondNumber,
                    "divide" => request.SecondNumber != 0 ? request.FirstNumber / request.SecondNumber : throw new DivideByZeroException(),
                    _ => throw new InvalidOperationException($"Unsupported operation: {request.Operation}")
                };

                response.IsSuccess = true;
                _logger.LogInformation("Calculation completed for request {Id}: Result = {Result}", request.Id, response.Result);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.ErrorMessage = ex.Message;
                _logger.LogError(ex, "Error calculating for request {Id}", request.Id);
            }

            await _kafkaProducer.SendCalculationResponseAsync(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing calculation request {Id}", request.Id);
        }
    }
}
