

## Reference

- [SonarQube on Docker Hub](https://hub.docker.com/_/sonarqube)

- [Try out SonarQube](https://docs.sonarqube.org/latest/setup/get-started-2-minutes/)

## Run Sonarqube

```
docker-compose -f labs\static-analysis\infra\docker-compose.yml up -d
```

 Log in to http://localhost:9000 using System Administrator credentials:

login: admin
password: admin

set new password, admin2 is fine
 explore:

- http://localhost:9000/coding_rules - multiple languages & analysis types

- http://localhost:9000/coding_rules?languages=cs&types=VULNERABILITY - security vulns for C#

- http://localhost:9000/coding_rules?languages=cs&open=csharpsquid%3AS5445&types=VULNERABILITY - OWASP vuln


Create a project

- http://localhost:9000/projects/create

- select _Manually_

- enter key `hello-world-cs`

- create a token, call it anything :)

- copy the token, e.g. `a73c619c269c872d19234bde0c744ded62eb6c76` <- yours will be different

- choose .NET app, then .NET Core

- docs show what to run; we'll use that with a docker build

## Run Analysis in Docker Build

Save token in variable:

```
#nix
SONAR_TOKEN='<your-token>'

#ps
$SONAR_TOKEN='<your-token>'
```

```
docker build -t hello-world-cs --build-arg SONAR_TOKEN=$SONAR_TOKEN .\labs\static-analysis\hello-world-cs
```

Check the project - how does it look?

Issues: http://localhost:9000/project/issues?id=hello-world-cs&resolved=false
Security hotspots: http://localhost:9000/security_hotspots?id=hello-world-cs


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
docker build -t hello-world-cs --build-arg SONAR_TOKEN=$SONAR_TOKEN --build-arg SONAR_ENFORCE_GATE=true .\labs\static-analysis\hello-world-cs
```

> Build fails; downside - rules are outside of SCM so you could fetch an old version and build and not get the same output if the rules have changed

## Lab

Java Hello World

- don't need to create a project

docker build -t hello-world-java --build-arg SONAR_TOKEN=$SONAR_TOKEN .\labs\static-analysis\hello-world-java

- configure project to use the custom gate

- run build so gate is enforced

docker build -t hello-world-java --build-arg SONAR_TOKEN=$SONAR_TOKEN --build-arg SONAR_ENFORCE_GATE=true  .\labs\static-analysis\hello-world-java

