FROM mcr.microsoft.com/dotnet/sdk:6.0.401-alpine3.16-amd64 AS build
WORKDIR /app

# Copy fsproj and restore as distinct layers
COPY src/Exercism.Analyzer.FSharp/Exercism.Analyzer.FSharp.fsproj ./
RUN dotnet restore -r linux-musl-x64

# Copy everything else and build
COPY src/Exercism.Analyzer.FSharp/ ./
RUN dotnet publish -r linux-musl-x64 -c Release -o /opt/analyzer --no-restore --self-contained true

# Build runtime image
FROM mcr.microsoft.com/dotnet/runtime-deps:6.0.9-alpine3.16-amd64
WORKDIR /opt/analyzer

COPY --from=build /opt/analyzer/ .
COPY --from=build /usr/local/bin/ /usr/local/bin/

COPY bin/run.sh /opt/analyzer/bin/

ENTRYPOINT ["sh", "/opt/analyzer/bin/run.sh"]
