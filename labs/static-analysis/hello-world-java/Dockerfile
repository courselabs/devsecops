FROM maven:3.6.3-jdk-11 AS builder
ARG SONAR_PROJECT='hello-world-java'
ARG SONAR_URL='http://host.docker.internal:9000'
ARG SONAR_TOKEN=''
ARG SONAR_ENFORCE_GATE='false'

WORKDIR /usr/src/helloworld
COPY pom.xml .
RUN mvn -B dependency:go-offline

COPY . .
RUN mvn package

RUN if [ "$SONAR_TOKEN" != "" ] ; then \
        mvn sonar:sonar \
        -Dsonar.projectKey=h$SONAR_PROJECT \
        -Dsonar.host.url=$SONAR_URL \
        -Dsonar.login=$SONAR_TOKEN \
        -Dsonar.qualitygate.wait=$SONAR_ENFORCE_GATE ; \
    fi

# app
FROM openjdk:11.0.12-jre-slim-buster

WORKDIR /app
COPY --from=builder /usr/src/helloworld/target/helloworld-0.1.0.jar .

ENTRYPOINT ["java", "-jar", "/app/helloworld-0.1.0.jar"]
