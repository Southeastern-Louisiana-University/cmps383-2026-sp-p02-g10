

STEP 1
docker run -e ACCEPT_EULA=Y -e SA_PASSWORD=YourPassword123! -p 1433:1433 -d mcr.microsoft.com/mssql/server:2022-latest

STEP 2

cd /Users/shivpatel/Desktop/cmps383-2026-sp-p02-g10
dotnet run --project Selu383.SP26.Api

STEP 3 (OPTIONAL TO STOP)

docker stop <container_id>