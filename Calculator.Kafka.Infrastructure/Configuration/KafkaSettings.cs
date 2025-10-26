namespace Calculator.Kafka.Infrastructure.Configuration;

public class KafkaSettings
{
    public string BootstrapServers { get; set; } = "localhost:9092";
    public string RequestTopic { get; set; } = "calculation-requests";
    public string ResponseTopic { get; set; } = "calculation-responses";
    public string GroupId { get; set; } = "calculator-workers";
    public int RetryCount { get; set; } = 3;
    public int RetryDelayMs { get; set; } = 1000;
}