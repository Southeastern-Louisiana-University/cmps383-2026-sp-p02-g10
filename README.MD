
# Project Setup Instructions

Follow these steps to set up and run the project on your machine. All commands are copy-paste ready.

---

## Prerequisites

- [Docker](https://www.docker.com/products/docker-desktop/)
- [.NET 10 SDK](https://dotnet.microsoft.com/en-us/download)
- [Node.js & npm](https://nodejs.org/) (v18+ recommended)

---

## 1. Start SQL Server with Docker

If you don't have a container yet, run:

```sh
docker run -e ACCEPT_EULA=Y -e SA_PASSWORD=Password123! -p 1433:1433 -d mcr.microsoft.com/mssql/server:2022-latest
```

If you already have a container, start it (replace <container_id> as needed):

```sh
docker ps -a # Find your container id
docker start <container_id>
```

---

## 2. Backend API (.NET Core)

Open a terminal in the project root and run:

```sh
dotnet restore Selu383.SP26.Api/Selu383.SP26.Api.csproj
dotnet ef database update --project Selu383.SP26.Api/Selu383.SP26.Api.csproj --startup-project Selu383.SP26.Api/Selu383.SP26.Api.csproj
dotnet run --project Selu383.SP26.Api/Selu383.SP26.Api.csproj
```

To run backend tests:

```sh
dotnet test Selu383.SP26.Tests/Selu383.SP26.Tests.csproj
```

---

## 3. Mobile App (Expo/React Native)

```sh
cd Selu383.SP26.Mobile
npm install
npx expo start
```

---

## 4. Web App (Vite/React)

```sh
cd Selu383.SP26.Web
npm install
npm run dev
```

---

## Useful Docker Commands

- List all containers:
	```sh
	docker ps -a
	```
- Stop a container:
	```sh
	docker stop <container_id>
	```

---

## Notes

- Default SQL password is `Password123!` (change in `appsettings.json` if needed)
- API runs at https://localhost:7116 (see launchSettings.json)
- Mobile and web apps may need to be configured to point to your API URL

---

You're ready to go!