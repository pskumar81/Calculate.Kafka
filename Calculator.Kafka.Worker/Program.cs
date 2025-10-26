using Calculator.Kafka.Infrastructure.Configuration;
using Calculator.Kafka.Infrastructure.Interfaces;
using Calculator.Kafka.Infrastructure.Services;
using Calculator.Kafka.Worker;

var builder = Host.CreateApplicationBuilder(args);

// Configure Kafka settings
builder.Services.Configure<KafkaSettings>(
    builder.Configuration.GetSection("Kafka"));

// Register Kafka services
builder.Services.AddSingleton<IKafkaProducer, KafkaProducer>();
builder.Services.AddSingleton<IKafkaConsumer, KafkaConsumer>();

// Register the Worker
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
