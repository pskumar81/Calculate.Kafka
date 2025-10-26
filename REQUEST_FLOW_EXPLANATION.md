# Calculator.Kafka - Request/Response Flow Explanation

## Component Architecture

```
[Client] → [Calculator.API] → [Kafka Broker] → [Calculator.Worker] → [Kafka Broker] → [Response Consumer]
   │              │               │                    │                   │                    │
   │         HTTP Request    Produce Message      Consume Message    Produce Result       Consume Result
   │         (Synchronous)   (Asynchronous)      (Asynchronous)     (Asynchronous)      (Asynchronous)
   │              │               │                    │                   │                    │
   └─── Immediate ack ────────────┘                    │                   │                    │
              │                                        │                   │                    │
              └── Request ID returned                  │                   │                    │
                                                       │                   │                    │
                                            ┌─────────┴─────────┐         │                    │
                                            │   Calculation     │         │                    │
                                            │   Processing      │         │                    │
                                            │   (10 + 5 = 15)   │         │                    │
                                            └─────────┬─────────┘         │                    │
                                                      │                   │                    │
                                                      └───────────────────┘                    │
                                                                                               │
                                                    [Optional: Response Handler Service] ←─────┘
```

## Detailed Flow Breakdown

### 1. CLIENT REQUEST
- **Component**: HTTP Client (curl, browser, etc.)
- **Action**: Sends POST request to Calculator API
- **Data**: JSON with firstNumber, secondNumber, operation
- **Response**: Immediate acknowledgment with RequestID

### 2. CALCULATOR.API (Producer)
- **Component**: ASP.NET Core Web API
- **Role**: Request validator and Kafka producer
- **Actions**:
  - Validates request data
  - Generates unique RequestID
  - Serializes to JSON
  - Publishes to 'calculation-requests' topic
  - Returns immediate response to client

### 3. KAFKA BROKER
- **Component**: Apache Kafka message broker
- **Role**: Message storage and routing
- **Actions**:
  - Stores messages in 'calculation-requests' topic
  - Provides durability and ordering guarantees
  - Enables decoupling between API and Worker

### 4. CALCULATOR.WORKER (Consumer + Producer)
- **Component**: .NET Background Service
- **Role**: Message processor and result producer
- **Actions**:
  - Consumes from 'calculation-requests' topic
  - Performs actual calculation
  - Publishes result to 'calculation-responses' topic

### 5. RESPONSE HANDLING
- **Current**: Results stored in Kafka topic
- **Future**: Response consumer service could notify clients