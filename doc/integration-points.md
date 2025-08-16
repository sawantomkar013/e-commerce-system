# Integration Points

This project integrates with several external services and systems to fulfill its functionality.

1.  **NotificationService Integration (HTTP)**
    * **Mechanism**: The `OrderService` uses the `NotificationServiceClients.cs` from OrderService.Infrastructure.ServiceClients project to send an HTTP POST request to the `NotificationService.API`.
    * **Retry Policy**: Robust retry logic is implemented using **Polly** within the client, with status-specific policies (e.g., exponential backoff) to handle transient failures.

2.  **Kafka Integration**
    * **Library**: `Confluent.Kafka` is used within the `KafkaProducer` from OrderService.Infrastructure.BackgroundJobs project to publish order creation events, enabling asynchronous, event-driven communication. The project also includes a PowerShell script to start Kafka consumers that can be used to read messages from the Kafka topics.
    * **Mechanism**: A MediatR handler invokes the `KafkaProducer` to publish a new order event, enabling asynchronous, event-driven communication.

3.  **Redis Caching**
    * **Library**: `StackExchange.Redis` is used for caching.
    * **Mechanism**: The response of the `GET /orders/{id}` query is cached using a MediatR pipeline behavior, improving performance by reducing repeated database calls.