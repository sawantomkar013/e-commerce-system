# OrderMicroservices (Layered)

Structure: Domain → Infrastructure → Interface.

## Run infra
```bash
docker compose up -d
```

## Run services
```bash
dotnet restore
dotnet run --project "src/Notification Service/NotificationService"
dotnet run --project "src/Order Service/Interface/OrderService.Interface.API"
```

Order API: https://localhost:5001/swagger
Notification API: https://localhost:6001/swagger
