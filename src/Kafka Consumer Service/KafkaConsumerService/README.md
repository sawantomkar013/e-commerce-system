# KafkaConsumerService

Small .NET 8 worker that consumes messages from a Kafka topic (`orders.created`) and logs them.

## Requirements
- .NET 8 SDK
- Kafka broker reachable at the configured `BootstrapServers` (default: localhost:9092). You can use docker-compose to run Kafka+Zookeeper locally.

## Run
1. Start Kafka (docker-compose or local)
2. From this project's folder:
   ```bash
   dotnet restore
   dotnet run --project KafkaConsumerService
   ```

## Configuration
Edit `appsettings.json` to change `Kafka:BootstrapServers`, `Kafka:GroupId`, or `Kafka:Topic`.
