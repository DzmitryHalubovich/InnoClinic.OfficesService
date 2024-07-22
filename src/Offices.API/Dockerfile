#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["/src/Offices.API/Offices.API.csproj", "src/Offices.API/"]
COPY ["/src/Offices.Contracts/Offices.Contracts.csproj", "src/Offices.Contracts/"]
COPY ["/src/Offices.Domain/Offices.Domain.csproj", "src/Offices.Domain/"]
COPY ["/src/Offices.Infrastructure/Offices.Infrastructure.csproj", "src/Offices.Infrastructure/"]
COPY ["/src/Offices.Presentation/Offices.Presentation.csproj", "src/Offices.Presentation/"]
COPY ["/src/Offices.Services/Offices.Services.csproj", "src/Offices.Services/"]
COPY ["/src/Offices.Services.Abstractions/Offices.Services.Abstractions.csproj", "src/Offices.Services.Abstractions/"]
RUN dotnet restore "./src/Offices.API/Offices.API.csproj"
COPY . .
WORKDIR "/src/src/Offices.API"
RUN dotnet build "./Offices.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Offices.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Offices.API.dll"]