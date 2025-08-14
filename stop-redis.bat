@echo off
REM Stop Redis server
echo Stopping Redis server...

REM Using redis-cli to send SHUTDOWN command
cd C:\Redis
redis-cli.exe shutdown

echo Redis server stopped.
pause
