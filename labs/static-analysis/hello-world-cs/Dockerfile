
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS sdk

# Sonar Scanner CLI needs JRE:
RUN apt-get update && \
    apt-get install -y --no-install-recommends \
    openjdk-11-jre

# Install the scanner:
RUN dotnet tool install --tool-path /usr/bin dotnet-sonarscanner

FROM sdk AS build
ARG SONAR_PROJECT='hello-world-cs'
ARG SONAR_URL='http://host.docker.internal:9000'
ARG SONAR_TOKEN=''
ARG SONAR_ENFORCE_GATE='false'

WORKDIR /src
COPY ["hello-world.csproj", "."]
RUN dotnet restore "./hello-world.csproj"

COPY . .
RUN if [ "$SONAR_TOKEN" != "" ] ; then dotnet sonarscanner begin /k:$SONAR_PROJECT /d:sonar.host.url=$SONAR_URL /d:sonar.login=$SONAR_TOKEN /d:sonar.qualitygate.wait=$SONAR_ENFORCE_GATE ; fi
RUN dotnet build "hello-world.csproj" -c Release
RUN if [ "$SONAR_TOKEN" != "" ] ; then dotnet sonarscanner end /d:sonar.login=$SONAR_TOKEN ; fi

RUN dotnet publish "hello-world.csproj" -c Release -o /out

FROM mcr.microsoft.com/dotnet/runtime:6.0
WORKDIR /app
COPY --from=build /out .
ENTRYPOINT ["dotnet", "hello-world.dll"]