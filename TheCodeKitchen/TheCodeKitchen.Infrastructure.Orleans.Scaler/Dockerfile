FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["TheCodeKitchen.Infrastructure.Orleans.Scaler/TheCodeKitchen.Infrastructure.Orleans.Scaler.csproj", "TheCodeKitchen.Infrastructure.Orleans.Scaler/"]
COPY ["TheCodeKitchen.Infrastructure.Orleans/TheCodeKitchen.Infrastructure.Orleans.csproj", "TheCodeKitchen.Infrastructure.Orleans/"]
COPY ["TheCodeKitchen.Infrastructure/TheCodeKitchen.Infrastructure.csproj", "TheCodeKitchen.Infrastructure/"]
COPY ["TheCodeKitchen.Application.Constants/TheCodeKitchen.Application.Constants.csproj", "TheCodeKitchen.Application.Constants/"]
COPY ["TheCodeKitchen.Infrastructure.Orleans.Client/TheCodeKitchen.Infrastructure.Orleans.Client.csproj", "TheCodeKitchen.Infrastructure.Orleans.Client/"]
COPY ["TheCodeKitchen.Infrastructure.Logging.Serilog/TheCodeKitchen.Infrastructure.Logging.Serilog.csproj", "TheCodeKitchen.Infrastructure.Logging.Serilog/"]
RUN dotnet restore "TheCodeKitchen.Infrastructure.Orleans.Scaler/TheCodeKitchen.Infrastructure.Orleans.Scaler.csproj"
COPY . .
WORKDIR "/src/TheCodeKitchen.Infrastructure.Orleans.Scaler"
RUN dotnet build "./TheCodeKitchen.Infrastructure.Orleans.Scaler.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./TheCodeKitchen.Infrastructure.Orleans.Scaler.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TheCodeKitchen.Infrastructure.Orleans.Scaler.dll"]
