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

EXPOSE 5000

ENTRYPOINT [ "dotnet", "AkkaSync.Host.dll" ]