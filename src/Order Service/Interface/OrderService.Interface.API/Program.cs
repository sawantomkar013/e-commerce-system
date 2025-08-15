using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using OrderService.Domain.DataAccess;
using OrderService.Infrastructure.BackgroundJobs;
using OrderService.Infrastructure.EntityFramework;
using OrderService.Infrastructure.GlobalValidation;
using OrderService.Infrastructure.Logging;
using OrderService.Infrastructure.MediatR;
using OrderService.Infrastructure.NamingPolicy;
using OrderService.Infrastructure.ServiceClients;
using OrderService.Interface.API.Mappers.Orders;

var builder = WebApplication.CreateBuilder(args);

// Serilog
builder.ConfigureSerilog();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<OrderMapper>();

// Db + Redis + MediatR + HttpClient + Kafka
builder.Services.AddScoped<AuditSaveChangesInterceptor>();
var conn = builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=app.db";
builder.Services.AddOrderDb(conn);

var redisConn = builder.Configuration.GetValue<string>("Redis:Connection") ?? "localhost:6379";
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = redisConn;
});

builder.Services.AddMediatRAndBehaviors(typeof(Program).Assembly);

var notifyBase = builder.Configuration.GetValue<string>("NotificationService:BaseUrl") ?? "https://localhost:6001/";
builder.Services.AddHttpClient<INotificationClient, NotificationServiceClient>(client =>
{
    client.BaseAddress = new Uri(notifyBase); // NotificationService URL
});

var kafkaSection = builder.Configuration.GetSection("Kafka");
builder.Services.Configure<KafkaSettings>(kafkaSection);

builder.Services.AddSingleton<IKafkaProducer, KafkaProducer>();

// Configure API behavior with snake_case serialization
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = new SnakeCaseNamingPolicy();
    options.SerializerOptions.DictionaryKeyPolicy = new SnakeCaseNamingPolicy();
    options.SerializerOptions.PropertyNameCaseInsensitive = true;
});


builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = new SnakeCaseNamingPolicy();
    options.JsonSerializerOptions.DictionaryKeyPolicy = new SnakeCaseNamingPolicy();
    options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Order Service API",
        Version = "v1"
    });

    // This ensures snake_case is reflected in schema names
    c.SchemaGeneratorOptions.SchemaIdSelector = (type) => type.Name;
    c.SchemaFilter<SnakeCaseSchemaFilter>();
});

builder.Services.AddCustomValidationResponses();

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
