- [x] Verify that the copilot-instructions.md file in the .github directory is created.
- [x] Clarify Project Requirements
- [x] Scaffold the Project
- [x] Customize the Project
- [x] Install Required Extensions
- [x] Compile the Project
- [x] Create and Run Task
- [x] Launch the Project
- [x] Ensure Documentation is Complete

## Calculator.Kafka Project

This is a complete Calculator service implementation using Apache Kafka for messaging, following the same architectural patterns as the IBM MQ and RabbitMQ versions.

### Project Structure

- **Calculator.Kafka.API** - Web API service for receiving calculation requests
- **Calculator.Kafka.Worker** - Background worker service for processing calculations
- **Calculator.Kafka.Models** - Shared models and DTOs
- **Calculator.Kafka.Infrastructure** - Kafka messaging infrastructure
- **Calculator.Kafka.Tests** - Unit tests

### Key Features

- RESTful API with Swagger documentation
- Asynchronous message processing with Kafka
- Docker support with docker-compose orchestration
- Comprehensive error handling and logging
- Health check endpoints
- Kafka UI for monitoring

### Getting Started

1. Start Kafka infrastructure: `docker-compose up -d zookeeper kafka kafka-ui`
2. Run the application: `docker-compose up` or use VS Code tasks
3. Access Swagger UI: `http://localhost:5000/swagger`
4. Monitor Kafka: `http://localhost:8080`

Work through each checklist item systematically.
Keep communication concise and focused.
Follow development best practices.