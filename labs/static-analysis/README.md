# Static Code Analysis

Static analysis tools inspect the source code of your applications, looking for known issues. The tools use a database of issues which can include poor coding practices, code which is difficult to maintain, and known security problems. [SonarQube]() is one of the most popular analysers. It supports multiple languages with databases which include best practices and CVEs.

There are commercial versions of SonarQube but we'll run the free community edition. It runs as a server, and you can trigger the analysis when you build your code. A SonarQube client on your dektop or build server sends the code for analysis and prints the results. Any issues are stored in the SonarQube server, and you can configure critical issues to fail the build.

## Reference

- [SonarQube on Docker Hub](https://hub.docker.com/_/sonarqube)

- [Try out SonarQube](https://docs.sonarqube.org/latest/setup/get-started-2-minutes/)

- [SonarQube with .NET apps]()

- [SonarQube with Java apps and Maven]()

## Run Sonarqube

Start by running SonarQube - this [Docker Compose file](./infra/docker-compose.yml) uses the official image on Docker Hub, and publishes to port 9000 on your machine:

```
docker-compose -f labs/static-analysis/infra/docker-compose.yml up -d
```

SonarQube can take a minute or two to spin up. When the container is running, log in to the web UI at http://localhost:9000 using System Administrator credentials:

- login: `admin`
- password: `admin`

It will ask you to set new password, `admin2` is fine.

This is a new SonarQube server with no projects set up, but you can browse the rules it uses:

- http://localhost:9000/coding_rules - coding rules for multiple languages and analysis types

- http://localhost:9000/coding_rules?languages=cs&types=VULNERABILITY - security vulnerabilities for C# apps

- http://localhost:9000/coding_rules?languages=cs&open=csharpsquid%3AS5445&types=VULNERABILITY - a known OWASP vulnerabilty for C#

Just browsing the rules for your language can help you write better and more secure code :)


## Create a SonarQube Project

We'll create a new project we can use to analyse some .NET Code.

Browse to http://localhost:9000/projects/create and configure the new project:

- select _Manually_ (SonarQube can integrate with other services, but we'll set it up manually)

- set the _Project key_ to be `hello-world-cs`

- click _Set Up_

- create a token, call it anything e.g. `devops`

- SonarQube displays the token - we'll use this later for authentication, so copy the token (it looks like `d19405a6925267bbcd52221c407bc01c2010c82b` but yours will be different)

- click _Continue_

- choose .NET for the application type, then .NET Core

The project UI shows you how you can run a local build and have your source analysed in the SonarQube server.

## Run Analysis in Docker Build

We'll run our local build using Docker, so we don't need to install any SDKs.

Start by saving your SonarQube token in a variable in your terminal session (if you don't have it, you can create a new one from your [security page](http://localhost:9000/account/security/)).

_Be sure to use your actual token in this command:_

```
# on macOS or Linux:
SONAR_TOKEN='<your-token>'

# or on Windows PowerShell:
$SONAR_TOKEN='<your-token>'
```

Now we can pass the token as a build argument for this [.NET Dockerfile](.\hello-world-cs\Dockerfile):

- this builds a simple Hello World app, with optional SonarQube analysis

- the build runs inside Docker, so we can access SonarQube via the host using the special `host.docker.internal` address

- you'll see the `dotnet sonarscanner` commands in the Dockerfile, which are the same ones SonarQube printed on the project page

```
docker build -t hello-world-cs --build-arg SONAR_TOKEN=$SONAR_TOKEN ./labs/static-analysis/hello-world-cs

£ if you're using Linux then the `host.docker.internal` address may not be set
£ you can run this instead:
export DOCKER_BUILDKIT=0

docker build -t hello-world-cs --build-arg SONAR_TOKEN=$SONAR_TOKEN --build-arg SONAR_URL=http://sonarqube:9000 --network infra_default ./labs/static-analysis/hello-world-cs
```

When the build completes, it sends the results to the SonarQube server where they get stored for the project.

Check the project in the UI - how does it look?

- overall project status - http://localhost:9000/dashboard?id=hello-world-cs
- issues - http://localhost:9000/project/issues?id=hello-world-cs&resolved=false
- security hotspots - http://localhost:9000/security_hotspots?id=hello-world-cs


## Failing Builds with Quality Gates

http://localhost:9000/quality_gates/

Copy the default _Sonar way_ gate; call the new gate `courselabs`

Default conditions are only for new code;

click _Add Condition_
select _On Overall Code_
dropdown choose _Security Rating_
operator select worse than _A_
click _Add Condition_

Brose back to cs proj http://localhost:9000/dashboard?id=hello-world

Click _Project Settings_ dropdown in top-right and select _Quality Gate_
Select _Always use a Specific Quality Gate_ and select your new `courselabs` gate

Now repeat the build with gate enforcement:

```
docker build -t hello-world-cs --build-arg SONAR_TOKEN=$SONAR_TOKEN --build-arg SONAR_ENFORCE_GATE=true ./labs/static-analysis/hello-world-cs
```

> Build fails; downside - rules are outside of SCM so you could fetch an old version and build and not get the same output if the rules have changed

## Lab

Java Hello World

- don't need to create a project

docker build -t hello-world-java --build-arg SONAR_TOKEN=$SONAR_TOKEN ./labs/static-analysis/hello-world-java

- configure project to use the custom gate

- run build so gate is enforced

docker build -t hello-world-java --build-arg SONAR_TOKEN=$SONAR_TOKEN --build-arg SONAR_ENFORCE_GATE=true  ./labs/static-analysis/hello-world-java

