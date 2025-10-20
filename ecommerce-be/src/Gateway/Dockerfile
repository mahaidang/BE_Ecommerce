# ========== BUILD STAGE ==========
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ../../Ecommerce.sln .
COPY ../../src/Gateway/Gateway.Api ./Gateway/Gateway.Api

WORKDIR /src/Gateway/Gateway.Api
RUN dotnet restore Gateway.Api.csproj
RUN dotnet publish Gateway.Api.csproj -c Release -o /app/publish /p:UseAppHost=false

# ========== RUNTIME STAGE ==========
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "Gateway.Api.dll"]
