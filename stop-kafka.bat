@echo off
cd /d "%~dp0"
echo Stopping Kafka broker...
taskkill /IM "java.exe" /F
echo Kafka and ZooKeeper processes terminated.
pause
