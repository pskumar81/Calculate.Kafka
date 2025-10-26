using Calculator.Kafka.Models;

namespace Calculator.Kafka.Infrastructure.Interfaces;

public interface IKafkaProducer
{
    Task SendCalculationRequestAsync(CalculationRequest request);
    Task SendCalculationResponseAsync(CalculationResponse response);
}

public interface IKafkaConsumer
{
    Task StartConsumingRequestsAsync(Func<CalculationRequest, Task> messageHandler, CancellationToken cancellationToken);
    Task StartConsumingResponsesAsync(Func<CalculationResponse, Task> messageHandler, CancellationToken cancellationToken);
}