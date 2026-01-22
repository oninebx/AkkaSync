FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY . .
RUN dotnet restore src/AkkaSync.Host/AkkaSync.Host.csproj
RUN dotnet publish src/AkkaSync.Host/AkkaSync.Host.csproj \
    -c Release \
    -o /app/publish \
    /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

COPY --from=build /app/publish .
COPY src/AkkaSync.Host/plugins ./plugins
COPY samples /app/samples

ENV AKKASYNC_SAMPLES_PATH=/app/samples

EXPOSE 5000
EXPOSE 5001

ENTRYPOINT [ "dotnet", "AkkaSync.Host.dll" ]