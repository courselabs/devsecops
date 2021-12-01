FROM openjdk:11-jdk-slim AS builder

WORKDIR /src
COPY HelloWorld.java .

RUN mkdir -p /out/bin && \
    javac -d /out/bin HelloWorld.java

# app image
FROM openjdk:11-jre-slim

WORKDIR /app
COPY --from=builder /out/bin/HelloWorld.class .

CMD java HelloWorld