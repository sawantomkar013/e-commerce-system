@echo off
cd /d "%~dp0"
powershell -NoProfile -ExecutionPolicy Bypass -File ".\StartKafkaServices.ps1"
powershell -NoProfile -ExecutionPolicy Bypass -File ".\KafkaConsumer.ps1"
pause
