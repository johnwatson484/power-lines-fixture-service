# Development
FROM mcr.microsoft.com/dotnet/core/sdk:3.1-alpine AS development

RUN apk update \
  && apk --no-cache add curl procps unzip \
  && wget -qO- https://aka.ms/getvsdbgsh | /bin/sh /dev/stdin -v latest -l /vsdbg

RUN addgroup -g 1000 dotnet \
    && adduser -u 1000 -G dotnet -s /bin/sh -D dotnet

USER dotnet
WORKDIR /home/dotnet

COPY --chown=dotnet:dotnet ./Directory.Build.props ./Directory.Build.props
RUN mkdir -p /home/dotnet/PowerLinesFixtureService/ /home/dotnet/PowerLinesFixtureService.Tests/
COPY --chown=dotnet:dotnet ./PowerLinesFixtureService.Tests/*.csproj ./PowerLinesFixtureService.Tests/
RUN dotnet restore ./PowerLinesFixtureService.Tests/PowerLinesFixtureService.Tests.csproj
COPY --chown=dotnet:dotnet ./PowerLinesFixtureService/*.csproj ./PowerLinesFixtureService/
RUN dotnet restore ./PowerLinesFixtureService/PowerLinesFixtureService.csproj
COPY --chown=dotnet:dotnet ./PowerLinesFixtureService.Tests/ ./PowerLinesFixtureService.Tests/
RUN true
COPY --chown=dotnet:dotnet ./PowerLinesFixtureService/ ./PowerLinesFixtureService/
RUN true
COPY --chown=dotnet:dotnet ./scripts/ ./scripts/
RUN dotnet publish ./PowerLinesFixtureService/ -c Release -o /home/dotnet/out

ARG PORT=5002
ENV PORT ${PORT}
ENV ASPNETCORE_URLS http://*:5002
ENV ASPNETCORE_ENVIRONMENT development
EXPOSE ${PORT}
# Override entrypoint using shell form so that environment variables are picked up
ENTRYPOINT dotnet watch --project ./PowerLinesFixtureService run

# Production
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-alpine AS production

RUN addgroup -g 1000 dotnet \
    && adduser -u 1000 -G dotnet -s /bin/sh -D dotnet

USER dotnet
WORKDIR /home/dotnet

COPY --from=development /home/dotnet/out/ ./
ARG PORT=5002
ENV ASPNETCORE_URLS http://*:5002
ENV ASPNETCORE_ENVIRONMENT production
EXPOSE ${PORT}
# Override entrypoint using shell form so that environment variables are picked up
ENTRYPOINT dotnet PowerLinesFixtureService.dll
