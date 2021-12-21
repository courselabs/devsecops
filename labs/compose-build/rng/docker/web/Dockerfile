FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine AS builder
ARG BUILD_VERSION=1.0.0

WORKDIR /src
COPY src/Numbers.Web/Numbers.Web.csproj .
RUN dotnet restore

COPY src/Numbers.Web/ .
RUN dotnet publish -c Release /p:Version=$BUILD_VERSION -o /out Numbers.Web.csproj

# app image
FROM mcr.microsoft.com/dotnet/aspnet:6.0-alpine

ENV RngApi__Url=http://numbers-api/rng

ENTRYPOINT ["dotnet", "/app/Numbers.Web.dll"]

WORKDIR /app
COPY --from=builder /out/ .

ARG BUILD_TAG=local
ARG COMMIT_SHA=local
LABEL build_tag=${BUILD_TAG}
LABEL commit_sha=${COMMIT_SHA}