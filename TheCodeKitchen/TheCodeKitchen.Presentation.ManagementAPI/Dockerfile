FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["TheCodeKitchen.Presentation.ManagementAPI/TheCodeKitchen.Presentation.ManagementAPI.csproj", "TheCodeKitchen.Presentation.ManagementAPI/"]
COPY ["TheCodeKitchen.Infrastructure.Security/TheCodeKitchen.Infrastructure.Security.csproj", "TheCodeKitchen.Infrastructure.Security/"]
COPY ["TheCodeKitchen.Infrastructure/TheCodeKitchen.Infrastructure.csproj", "TheCodeKitchen.Infrastructure/"]
COPY ["TheCodeKitchen.Infrastructure.AzureSignalR/TheCodeKitchen.Infrastructure.AzureSignalR.csproj", "TheCodeKitchen.Infrastructure.AzureSignalR/"]
COPY ["TheCodeKitchen.Application.Constants/TheCodeKitchen.Application.Constants.csproj", "TheCodeKitchen.Application.Constants/"]
COPY ["TheCodeKitchen.Application.Contracts/TheCodeKitchen.Application.Contracts.csproj", "TheCodeKitchen.Application.Contracts/"]
COPY ["TheCodeKitchen.Core.Domain/TheCodeKitchen.Core.Domain.csproj", "TheCodeKitchen.Core.Domain/"]
COPY ["TheCodeKitchen.Core.Enums/TheCodeKitchen.Core.Enums.csproj", "TheCodeKitchen.Core.Enums/"]
COPY ["TheCodeKitchen.Presentation/TheCodeKitchen.Presentation.csproj", "TheCodeKitchen.Presentation/"]
COPY ["TheCodeKitchen.Infrastructure.Orleans.Client/TheCodeKitchen.Infrastructure.Orleans.Client.csproj", "TheCodeKitchen.Infrastructure.Orleans.Client/"]
COPY ["TheCodeKitchen.Infrastructure.Orleans/TheCodeKitchen.Infrastructure.Orleans.csproj", "TheCodeKitchen.Infrastructure.Orleans/"]
COPY ["TheCodeKitchen.Infrastructure.Logging.Serilog/TheCodeKitchen.Infrastructure.Logging.Serilog.csproj", "TheCodeKitchen.Infrastructure.Logging.Serilog/"]
COPY ["TheCodeKitchen.Application.Validation/TheCodeKitchen.Application.Validation.csproj", "TheCodeKitchen.Application.Validation/"]
RUN dotnet restore "TheCodeKitchen.Presentation.ManagementAPI/TheCodeKitchen.Presentation.ManagementAPI.csproj"
COPY . .
WORKDIR "/src/TheCodeKitchen.Presentation.ManagementAPI"
RUN dotnet build "./TheCodeKitchen.Presentation.ManagementAPI.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./TheCodeKitchen.Presentation.ManagementAPI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TheCodeKitchen.Presentation.ManagementAPI.dll"]
