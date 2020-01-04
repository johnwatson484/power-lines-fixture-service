# BASE
FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS base
WORKDIR /app
ENV ASPNETCORE_ENVIRONMENT=production

# DEVELOPMENT
FROM base AS development-env
WORKDIR /PowerLinesFixtureService
RUN apt-get update \
 && apt-get install -y --no-install-recommends unzip \
 && curl -sSL https://aka.ms/getvsdbgsh | bash /dev/stdin -v latest -l /vsdbg
COPY ./PowerLinesFixtureService/*.csproj ./
RUN dotnet restore
COPY ./PowerLinesFixtureService ./
ENTRYPOINT [ "dotnet", "watch", "run", "--urls", "http://0.0.0.0:5000" ]

# TEST
FROM development-env AS test-env
WORKDIR /PowerLinesFixtureService.Tests
COPY ./PowerLinesFixtureService.Tests/*.csproj ./
RUN dotnet restore
COPY ./PowerLinesFixtureService.Tests ./
ENTRYPOINT [ "dotnet", "test" ]

# PRODUCTION
FROM base AS build-env
COPY ./PowerLinesFixtureService/*.csproj ./
RUN dotnet restore
COPY ./PowerLinesFixtureService ./
RUN dotnet publish -c Release -o out

# RUNTIME
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS production-env
WORKDIR /app
COPY --from=build-env /app/out .
RUN chown -R www-data:www-data /app
USER www-data
ENTRYPOINT ["dotnet", "PowerLinesFixtureService.dll"]
