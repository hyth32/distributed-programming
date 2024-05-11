@echo off
redis-cli shutdown
taskkill /IM dotnet.exe /F
taskkill /IM nginx.exe /F
