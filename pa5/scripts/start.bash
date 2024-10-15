cd ../nats-server/
./nats-server &

cd ../RankCalculator/
export DB_RUS="localhost:6000"
export DB_EU="localhost:6001"
export DB_OTHER="localhost:6002"
dotnet run &

cd ../EventsLogger/
dotnet run &
dotnet run &

cd ../Valuator/
export DB_RUS="localhost:6000"
export DB_EU="localhost:6001"
export DB_OTHER="localhost:6002"
dotnet run --urls "http://0.0.0.0:5001" &
dotnet run --urls "http://0.0.0.0:5002" &

cd ../../../src/nginx-1.25.4
./nginx -c ../../../src/nginx-1.25.4/conf/nginx.conf