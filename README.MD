
Step 1. Backend API (.NET Core):

Start docker by 
docker start 24ca4169cf2d

dotnet run --project Selu383.SP26.Api

2. Mobile App (Expo/React Native):

Open a terminal in the cd Selu383.SP26.Mobile directory.
Run: npm install (first time or after dependency changes)
Then run: npx expo start

3. Web App (Vite/React):

Open a terminal in the 
 cd Selu383.SP26.Web 
Run: npm install (first time or after dependency changes)
Then run: npm run dev






To create new cointainer

docker run -e ACCEPT_EULA=Y -e SA_PASSWORD=YourPassword123! -p 1433:1433 -d mcr.microsoft.com/mssql/server:2022-latest

To check exisiting container 

docker ps -a