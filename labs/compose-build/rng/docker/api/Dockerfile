FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine AS builder
ARG BUILD_VERSION=1.0.0

WORKDIR /src
COPY src/Numbers.Api/Numbers.Api.csproj .
RUN dotnet restore

COPY src/Numbers.Api/ .
RUN dotnet publish -c Release /p:Version=$BUILD_VERSION -o /out Numbers.Api.csproj

# app image
FROM mcr.microsoft.com/dotnet/aspnet:6.0-alpine

ENTRYPOINT ["dotnet", "/app/Numbers.Api.dll"]

WORKDIR /app
COPY --from=builder /out/ .

ARG BUILD_TAG=local
ARG COMMIT_SHA=local
LABEL build_tag=${BUILD_TAG}
LABEL commit_sha=${COMMIT_SHA}