@echo off
start cmd /k redis-server
start cmd /k "cd ..\nats-server\ & nats-server.exe"
start cmd /k "cd ..\Valuator\bin\Debug\net8.0\ & dotnet lw1.dll --urls="http://localhost:5001""
start cmd /k "cd ..\Valuator\bin\Debug\net8.0\ & dotnet lw1.dll --urls="http://localhost:5002""
start cmd /k "cd ..\nginx\ & nginx.exe -c .\conf\nginx.conf"