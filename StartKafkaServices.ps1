# Remove old Kafka and ZooKeeper logs
$kafkaLogPath = "C:\tmp\kafka-logs"
$zookeeperLogPath = "C:\tmp\zookeeper"

if (Test-Path $kafkaLogPath) {
    Remove-Item -Recurse -Force $kafkaLogPath
    Write-Host "Deleted old Kafka logs at $kafkaLogPath"
}

if (Test-Path $zookeeperLogPath) {
    Remove-Item -Recurse -Force $zookeeperLogPath
    Write-Host "Deleted old ZooKeeper logs at $zookeeperLogPath"
}

# Start ZooKeeper
Start-Process powershell -ArgumentList @(
    "-NoExit",
    "-Command",
    "`$env:JAVA_HOME='C:\Program Files\Java\jdk-11.0.9'; " +
    "`$env:PATH='C:\Program Files\Java\jdk-11.0.9\bin;' + `$env:PATH; " +
    "cd 'C:\Kafka\kafka\bin\windows'; " +
    ".\zookeeper-server-start.bat '..\..\config\zookeeper.properties'"
)

# Small delay to ensure ZooKeeper starts before Kafka
Start-Sleep -Seconds 5

# Start Kafka broker
Start-Process powershell -ArgumentList @(
    "-NoExit",
    "-Command",
    "`$env:JAVA_HOME='C:\Program Files\Java\jdk-11.0.9'; " +
    "`$env:PATH='C:\Program Files\Java\jdk-11.0.9\bin;' + `$env:PATH; " +
    "cd 'C:\Kafka\kafka\bin\windows'; " +
    ".\kafka-server-start.bat '..\..\config\server.properties'"
)
