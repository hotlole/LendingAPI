#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["LendingAPI/LendingAPI.csproj", "LendingAPI/"]
COPY ["Landing.Application/Landing.Application.csproj", "Landing.Application/"]
COPY ["Landing.Core/Landing.Core.csproj", "Landing.Core/"]
COPY ["Landing.Infrastructure/Landing.Infrastructure.csproj", "Landing.Infrastructure/"]
RUN dotnet restore "./LendingAPI/LendingAPI.csproj"
COPY . .
WORKDIR "/src/LendingAPI"
RUN dotnet build "./LendingAPI.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./LendingAPI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LendingAPI.dll"]