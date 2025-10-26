# Calculator.Kafka

A microservices calculator application using Apache Kafka for message-driven architecture. This project demonstrates async processing patterns with Kafka producers and consumers for mathematical operations.

## Architecture

The solution follows a microservices architecture with the following components:

- **Calculator.Kafka.API** - Web API service that receives calculation requests and publishes them to Kafka
- **Calculator.Kafka.Worker** - Background worker service that consumes calculation requests, performs operations, and publishes results
- **Calculator.Kafka.Models** - Shared data models and DTOs
- **Calculator.Kafka.Infrastructure** - Kafka messaging infrastructure and configuration
- **Calculator.Kafka.Tests** - Unit tests

## Features

- RESTful API for submitting calculation requests
- Asynchronous processing using Kafka message queues
- Support for basic arithmetic operations (add, subtract, multiply, divide)
- Docker support with docker-compose orchestration
- Comprehensive logging and error handling
- Kafka UI for monitoring message flows

## Prerequisites

- .NET 9.0 SDK
- Docker and Docker Compose
- Apache Kafka (included in docker-compose)

## Getting Started

### 1. Clone the Repository

```bash
git clone <repository-url>
cd Calculator.Kafka
```

### 2. Start Kafka Infrastructure

```bash
docker-compose up -d zookeeper kafka kafka-ui
```

### 3. Run the Application

#### Option A: Using Docker Compose (Recommended)

```bash
docker-compose up
```

#### Option B: Run Locally

Start the API:
```bash
dotnet run --project Calculator.Kafka.API
```

Start the Worker (in another terminal):
```bash
dotnet run --project Calculator.Kafka.Worker
```

### 4. Test the API

#### Using Swagger UI
Navigate to: `http://localhost:5000/swagger` (or `http://localhost:8080` if running via Docker)

#### Using curl
```bash
# Submit a calculation request
curl -X POST "http://localhost:5000/api/calculator/calculate" \
  -H "Content-Type: application/json" \
  -d '{
    "firstNumber": 10,
    "secondNumber": 5,
    "operation": "add"
  }'
```

### 5. Monitor Kafka

Access Kafka UI at: `http://localhost:8080` (if using Docker Compose)

## Configuration

### Kafka Settings

The application uses the following Kafka configuration (configurable in `appsettings.json`):

```json
{
  "Kafka": {
    "BootstrapServers": "localhost:9092",
    "RequestTopic": "calculation-requests",
    "ResponseTopic": "calculation-responses",
    "GroupId": "calculator-workers",
    "RetryCount": 3,
    "RetryDelayMs": 1000
  }
}
```

## API Endpoints

### POST /api/calculator/calculate

Submit a calculation request for asynchronous processing.

**Request Body:**
```json
{
  "firstNumber": 10,
  "secondNumber": 5,
  "operation": "add"
}
```

**Supported Operations:**
- `add` - Addition
- `subtract` - Subtraction
- `multiply` - Multiplication
- `divide` - Division

**Response:**
```json
{
  "requestId": "123e4567-e89b-12d3-a456-426614174000",
  "message": "Calculation request submitted for processing"
}
```

### GET /api/calculator/health

Health check endpoint.

## Message Flow

1. Client submits calculation request to API
2. API publishes request to `calculation-requests` topic
3. Worker consumes request from topic
4. Worker performs calculation
5. Worker publishes result to `calculation-responses` topic
6. Results can be consumed by other services or stored

## Development

### Building the Solution

```bash
dotnet build
```

### Running Tests

```bash
dotnet test
```

### Building Docker Images

```bash
# Build API image
docker build -t calculator-kafka-api -f Calculator.Kafka.API/Dockerfile .

# Build Worker image
docker build -t calculator-kafka-worker -f Calculator.Kafka.Worker/Dockerfile .
```

## Troubleshooting

### Common Issues

1. **Kafka Connection Issues**
   - Ensure Kafka is running: `docker-compose ps`
   - Check Kafka logs: `docker-compose logs kafka`

2. **Port Conflicts**
   - API runs on port 5000 (or 8080 in Docker)
   - Kafka UI runs on port 8080
   - Kafka broker runs on port 9092

3. **Message Processing Issues**
   - Check worker logs for processing errors
   - Verify topic creation in Kafka UI
   - Ensure proper serialization/deserialization

## Related Projects

This project is part of a series demonstrating different messaging technologies:
- [Calculator.IBMMQ](https://github.com/pskumar81/Calculator.IBMMQ) - IBM MQ implementation
- [Calculator.RabbitMQ](https://github.com/pskumar81/Calculator.RabbitMQ) - RabbitMQ implementation

## License

This project is licensed under the MIT License.