cd ../nats-server/
nohup ./nats-server &

cd ../RankCalculator/
nohup dotnet run &

cd ../EventsLogger/
nohup dotnet run &

cd ../Valuator/
nohup dotnet run --urls "http://0.0.0.0:5001" &
nohup dotnet run --urls "http://0.0.0.0:5002" &

cd ../../../src/nginx-1.25.4/
./nginx -c ../../../src/nginx-1.25.4/conf/nginx.conf