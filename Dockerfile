# Use the official .NET 8 runtime as base image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5000

# Use the official .NET 8 SDK for building
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project file and restore dependencies
COPY ["TeamClassification.csproj", "."]
RUN dotnet restore "TeamClassification.csproj"

# Copy source code and build
COPY . .
RUN dotnet build "TeamClassification.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "TeamClassification.csproj" -c Release -o /app/publish

# Create final runtime image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Set environment variables
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://0.0.0.0:5000

ENTRYPOINT ["dotnet", "TeamClassification.dll"]
