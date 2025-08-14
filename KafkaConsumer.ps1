# Set Java environment (required for Kafka scripts)
$env:JAVA_HOME = "C:\Program Files\Java\jdk-11.0.9"
$env:PATH = "$env:JAVA_HOME\bin;$env:PATH"

# Path to Kafka bin directory
$kafkaBinPath = "C:\Kafka\kafka\bin\windows"

# List of topics
$topics = @(
    "orders.created.pending",
    "orders.created.confirmed",
    "orders.created.shipped"
)

# Bootstrap server
$bootstrap = "localhost:9092"

foreach ($topic in $topics) {
    Write-Host "`nStarting consumer for topic: $topic"
    
    Start-Process powershell -ArgumentList @(
        "-NoExit",
        "-Command",
        "cd `"$kafkaBinPath`"; .\kafka-console-consumer.bat --bootstrap-server $bootstrap --topic $topic --from-beginning"
    )
}
