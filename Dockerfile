# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy everything
COPY . .

# Restore & publish using correct path from repo root
RUN dotnet restore QuantityMeasurementApp/QuantityMeasurementWebAPI/QuantityMeasurementWebAPI.csproj

RUN dotnet publish QuantityMeasurementApp/QuantityMeasurementWebAPI/QuantityMeasurementWebAPI.csproj \
    -c Release -o /app/out --no-restore

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

COPY --from=build /app/out .

ENTRYPOINT ["dotnet", "QuantityMeasurementWebAPI.dll"]