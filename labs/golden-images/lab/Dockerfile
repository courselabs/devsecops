FROM maven:3.6.3-jdk-11 AS builder

WORKDIR /usr/src/helloworld
COPY pom.xml .
RUN mvn -B dependency:go-offline

COPY . .
RUN mvn package

# app
FROM courselabs/java:jre-21.12 

WORKDIR /app
COPY --from=builder /usr/src/helloworld/target/helloworld-0.1.0.jar .

ENTRYPOINT ["java", "-jar", "/app/helloworld-0.1.0.jar"]
