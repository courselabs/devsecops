FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build

WORKDIR /src
COPY ["hello-world.csproj", "."]
RUN dotnet restore "./hello-world.csproj"

COPY . .
RUN dotnet publish "hello-world.csproj" -c Release -o /out

# app image
FROM mcr.microsoft.com/dotnet/runtime:6.0 AS app
WORKDIR /app
COPY --from=build /out .
ENTRYPOINT ["dotnet", "hello-world.dll"]

# CVE scanner
FROM app AS scan
COPY --from=aquasec/trivy:0.21.1 /usr/local/bin/trivy /usr/local/bin/trivy
RUN trivy --version > /scanner.txt && \
    trivy rootfs -s HIGH --exit-code 1 --no-progress /

# final image
FROM app as final
COPY --from=scan /scanner.txt .