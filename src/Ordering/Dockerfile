# ========== BUILD STAGE ==========
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ./Ecommerce.sln .
COPY ./src/Ordering ./Ordering


WORKDIR /src/Ordering/Ordering.Api
RUN dotnet restore
RUN dotnet publish Ordering.Api.csproj -c Release -o /app/publish /p:UseAppHost=false

# ========== RUNTIME STAGE ==========
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "Ordering.Api.dll"]
