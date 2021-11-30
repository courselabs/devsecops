

££ Reference

- [SonarQube on Docker Hub](https://hub.docker.com/_/sonarqube)

- [Try out SonarQube](https://docs.sonarqube.org/latest/setup/get-started-2-minutes/)

££ Run Sonarqube

```
docker run -d --name sonarqube -e SONAR_ES_BOOTSTRAP_CHECKS_DISABLE=true -p 9000:9000 sonarqube:8.9.3-community
```

 Log in to http://localhost:9000 using System Administrator credentials:

login: admin
password: admin

.net app - docker build



dotnet sonarscanner begin /k:"labs" /d:sonar.host.url="http://localhost:9000"  /d:sonar.login="93f7a5d00ca1a7f8b937bd1856bd87404e717b22"
dotnet build
dotnet sonarscanner end /d:sonar.login="93f7a5d00ca1a7f8b937bd1856bd87404e717b22"



