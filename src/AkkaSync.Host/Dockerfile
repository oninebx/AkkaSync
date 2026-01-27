# Build the standalone AkkaSync Host Docker image with:
# docker build -f src/AkkaSync.Host/Dockerfile -t akkasync-host .

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY . .

# compile mvp plugins
# copy plugins source code
COPY src/AkkaSync.Plugins.* ./AkkaSync.Plugins.*

# build & publish to /plugins
ARG PLUGINS="Source.File Sink.Sqlite Transformer.DbTable HistoryStore.InMemory"
RUN set -eux; \
    for name in $PLUGINS; do \
        proj="src/AkkaSync.Plugins.$name/AkkaSync.Plugins.$name.csproj"; \
        echo "Building plugin $proj"; \
        dotnet restore "$proj"; \
        dotnet publish "$proj" -c Release -o /app/plugins /p:UseAppHost=false; \
    done

RUN dotnet restore src/AkkaSync.Host/AkkaSync.Host.csproj
RUN dotnet publish src/AkkaSync.Host/AkkaSync.Host.csproj \
    -c Release \
    -o /app/publish \
    /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

COPY --from=build /app/publish .
COPY --from=build /app/plugins ./plugins

ARG DATA_SCENARIOS="Csv2Sqlite"
COPY examples/AkkaSync.Examples /examples-data

RUN set -eux; \
    mkdir -p /app/data/pipelines; \
    for scenario in $DATA_SCENARIOS; do \
        echo "Processing scenario: $scenario"; \
        \
        # input / output copy by senarios
        for dir in input output; do \
            src="/examples-data/$scenario/$dir"; \
            dst="/app/data/$scenario/$dir"; \
            mkdir -p "$dst"; \
            cp -r "$src/." "$dst/"; \
        done; \
        \
        # configs merge into pipelines
        cfg="/examples-data/$scenario/configs"; \
        if [ -d "$cfg" ]; then \
            cp -r "$cfg/." /app/pipelines/; \
        fi; \
    done

EXPOSE 5000

ENTRYPOINT [ "dotnet", "AkkaSync.Host.dll" ]