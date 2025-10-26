using System.Text.Json;
using Calculator.Kafka.Infrastructure.Configuration;
using Calculator.Kafka.Infrastructure.Interfaces;
using Calculator.Kafka.Models;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Calculator.Kafka.Infrastructure.Services;

public class KafkaProducer : IKafkaProducer
{
    private readonly IProducer<string, string> _producer;
    private readonly KafkaSettings _settings;
    private readonly ILogger<KafkaProducer> _logger;

    public KafkaProducer(IOptions<KafkaSettings> settings, ILogger<KafkaProducer> logger)
    {
        _settings = settings.Value;
        _logger = logger;

        var config = new ProducerConfig
        {
            BootstrapServers = _settings.BootstrapServers,
            Acks = Acks.All,
            MessageSendMaxRetries = _settings.RetryCount,
            RetryBackoffMs = _settings.RetryDelayMs
        };

        _producer = new ProducerBuilder<string, string>(config).Build();
    }

    public async Task SendCalculationRequestAsync(CalculationRequest request)
    {
        try
        {
            var message = new Message<string, string>
            {
                Key = request.Id.ToString(),
                Value = JsonSerializer.Serialize(request)
            };

            var result = await _producer.ProduceAsync(_settings.RequestTopic, message);
            _logger.LogInformation("Sent calculation request {Id} to topic {Topic}", request.Id, _settings.RequestTopic);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send calculation request {Id}", request.Id);
            throw;
        }
    }

    public async Task SendCalculationResponseAsync(CalculationResponse response)
    {
        try
        {
            var message = new Message<string, string>
            {
                Key = response.Id.ToString(),
                Value = JsonSerializer.Serialize(response)
            };

            var result = await _producer.ProduceAsync(_settings.ResponseTopic, message);
            _logger.LogInformation("Sent calculation response {Id} to topic {Topic}", response.Id, _settings.ResponseTopic);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send calculation response {Id}", response.Id);
            throw;
        }
    }

    public void Dispose()
    {
        _producer?.Dispose();
    }
}

public class KafkaConsumer : IKafkaConsumer
{
    private readonly KafkaSettings _settings;
    private readonly ILogger<KafkaConsumer> _logger;

    public KafkaConsumer(IOptions<KafkaSettings> settings, ILogger<KafkaConsumer> logger)
    {
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task StartConsumingRequestsAsync(Func<CalculationRequest, Task> messageHandler, CancellationToken cancellationToken)
    {
        var config = new ConsumerConfig
        {
            BootstrapServers = _settings.BootstrapServers,
            GroupId = _settings.GroupId,
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = false
        };

        using var consumer = new ConsumerBuilder<string, string>(config).Build();
        consumer.Subscribe(_settings.RequestTopic);

        _logger.LogInformation("Started consuming from topic {Topic}", _settings.RequestTopic);

        await Task.Run(async () =>
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var consumeResult = consumer.Consume(cancellationToken);
                    if (consumeResult?.Message?.Value != null)
                    {
                        var request = JsonSerializer.Deserialize<CalculationRequest>(consumeResult.Message.Value);
                        if (request != null)
                        {
                            await messageHandler(request);
                            consumer.Commit(consumeResult);
                            _logger.LogInformation("Processed calculation request {Id}", request.Id);
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error consuming message from topic {Topic}", _settings.RequestTopic);
                }
            }
        }, cancellationToken);
    }

    public async Task StartConsumingResponsesAsync(Func<CalculationResponse, Task> messageHandler, CancellationToken cancellationToken)
    {
        var config = new ConsumerConfig
        {
            BootstrapServers = _settings.BootstrapServers,
            GroupId = $"{_settings.GroupId}-responses",
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = false
        };

        using var consumer = new ConsumerBuilder<string, string>(config).Build();
        consumer.Subscribe(_settings.ResponseTopic);

        _logger.LogInformation("Started consuming from topic {Topic}", _settings.ResponseTopic);

        await Task.Run(async () =>
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var consumeResult = consumer.Consume(cancellationToken);
                    if (consumeResult?.Message?.Value != null)
                    {
                        var response = JsonSerializer.Deserialize<CalculationResponse>(consumeResult.Message.Value);
                        if (response != null)
                        {
                            await messageHandler(response);
                            consumer.Commit(consumeResult);
                            _logger.LogInformation("Processed calculation response {Id}", response.Id);
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error consuming message from topic {Topic}", _settings.ResponseTopic);
                }
            }
        }, cancellationToken);
    }
}
