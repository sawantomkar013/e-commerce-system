using KafkaConsumerService;
using KafkaConsumerService.Services;
using Serilog;

var builder = Host.CreateApplicationBuilder(args);

// Configure logging (Serilog to console)
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .WriteTo.Console()
    .CreateLogger();

// Bind Kafka settings
builder.Services.Configure<KafkaSettings>(builder.Configuration.GetSection("Kafka"));

// Register the background consumer
builder.Services.AddHostedService<KafkaBackgroundConsumer>();

var app = builder.Build();

await app.RunAsync();
