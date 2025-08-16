# Tech Stack

* **.NET 8+**: The core framework for building both microservices.
* **ASP.NET Core**: For building REST APIs in the `OrderService` and `NotificationService`.
* **MediatR**: To implement the **CQRS pattern** and enforce separation of concerns.
* **Polly**: For implementing robust, status-specific retry policies for HTTP calls.
* **Confluent.Kafka**: For producing and publishing events to Kafka.
* **StackExchange.Redis**: For integrating with Redis for caching.
* **Entity Framework Core**: For data persistence with a SQLite database.
* **xUnit**: The testing framework used for unit tests.
* **Moq**: The mocking library used for creating mock objects in tests.