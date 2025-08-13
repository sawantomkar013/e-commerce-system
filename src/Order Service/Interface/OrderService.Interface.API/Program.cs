using Confluent.Kafka;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using OrderService.Domain.DataAccess;
using OrderService.Infrastructure.BackgroundJobs;
using OrderService.Infrastructure.Caching;
using OrderService.Infrastructure.EntityFramework;
using OrderService.Infrastructure.Logging;
using OrderService.Infrastructure.MediatRSetup;
using OrderService.Infrastructure.ServiceClients;

var builder = WebApplication.CreateBuilder(args);

// Serilog
builder.ConfigureSerilog();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Db + Redis + MediatR + HttpClient + Kafka
var conn = builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=app.db";
builder.Services.AddOrderDb(conn);

var redisConn = builder.Configuration.GetValue<string>("Redis:Connection") ?? "localhost:6379";
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
});
builder.Services.AddRedisCache(redisConn);

builder.Services.AddMediatRAndBehaviors(typeof(Program).Assembly);

var notifyBase = builder.Configuration.GetValue<string>("NotificationService:BaseUrl") ?? "https://localhost:6001/";
builder.Services.AddHttpClient<INotificationClient, NotificationServiceClient>(client =>
{
    client.BaseAddress = new Uri(notifyBase); // NotificationService URL
});

var kafkaBootstrap = builder.Configuration.GetValue<string>("Kafka:BootstrapServers") ?? "localhost:9092";
builder.Services.AddSingleton(new ProducerConfig { BootstrapServers = kafkaBootstrap });
builder.Services.AddSingleton<IKafkaProducer, KafkaProducer>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Order Service API",
        Version = "v1"
    });
});

var app = builder.Build();

// Auto-migrate
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<OrderDbContext>();
    db.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Order Service API v1");
        c.RoutePrefix = string.Empty; // Swagger at root URL
    });
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
