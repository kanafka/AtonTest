FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

COPY AtonTest.sln .
COPY Domain/Domain.csproj Domain/
COPY Application/Application.csproj Application/
COPY Infrastructure/Infrastructure.csproj Infrastructure/
COPY API/API.csproj API/

RUN dotnet restore

COPY . .

RUN dotnet build -c Release --no-restore
RUN dotnet publish API/API.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 5000
EXPOSE 5001

ENTRYPOINT ["dotnet", "API.dll"]
