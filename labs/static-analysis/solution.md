# Lab Solution

The Dockerfile uses the `SONAR_TOKEN` argument as a flag - if its present then the analysis gets executed after compilation:

```
docker build -t hello-world-java --build-arg SONAR_TOKEN=$SONAR_TOKEN ./labs/static-analysis/hello-world-java
```

When it completes the build will pass and you can open the project at http://localhost:9000/dashboard?id=hhello-world-java

Click on _Project Settings_ then _Quality Gates_ and set it to use your custom gate.

Now run a build with the `SONAR_ENFORCE_GATE` argument set, so the quality checks are enforced:

```
docker build -t hello-world-java --build-arg SONAR_TOKEN=$SONAR_TOKEN --build-arg SONAR_ENFORCE_GATE=true  ./labs/static-analysis/hello-world-java
```

Open the project and you'll see there's a critical security issue in the code - http://localhost:9000/project/issues?id=hhello-world-java&resolved=false&severities=BLOCKER%2CCRITICAL%2CMAJOR%2CMINOR&types=VULNERABILITY

___
> Back to the [exercises](README.md).