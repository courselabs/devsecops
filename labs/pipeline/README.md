## Building Docker Images with Jenkins

docker-compose -f labs\pipeline\infra\docker-compose.yml up -d

- docker pipe

http://localhost:3000

create repo devsecops

```
git remote add gogs http://localhost:3000/courselabs/devsecops.git

git push gogs main
```

- jenkinsfile

http://localhost:8080

Create a new pipeline

- call it docker
- select _Pipeline script from SCM_ in the dropdown
- SCM = _Git_
- Repository URL = `http://gogs:3000/courselabs/devsecops.git`
- Branch Specifier = `refs/heads/main`
- Script path = `labs/pipeline/docker/Jenkinsfile`

Click Build Now 

- check output

What is the image tag? Build stage: docker.io/courselabs/hello-world:1 What is the output from running the container? Test stage: Hello, World

## Multistage Docker Builds

- docker build for ^^

dockerfile
jenkinsfile

- new pipeline called multi-stage
- copy from `docker`
- script path = `labs/pipeline/multi-stage/Jenkinsfile`

Build

- check output

What is the size of the new image? How does it compare to the SDK?

```
# image
d image ls courselabs/multi-stage:1

# sdk
d pull openjdk:11-jdk-slim
d image ls openjdk:11-jdk-slim 
```

> About half the size

## Building with Docker Compose

Check .NET version in the container:

```
# will fail
docker exec infra_jenkins_1 dotnet --version
```

> Not installed

- rng app - compose file

- new pipeline called compose
- copy from `docker`
- script path = `labs/pipeline/compose/Jenkinsfile`

Build 

Check output

How many images are built? 2: courselabs/rng-api:21.12-1 and courselabs/rng-web:21.12-1. Where does the 21.12 come from? Release name in Jenkinsfile.

Reg details are parameterised so any registry can be used.


## Pushing to Docker Hub

- open global credentials store http://localhost:8080/credentials/store/system/domain/_/
- add credential, type _Username with password_
-- your username and password
-- id `docker-hub`

- uncomment push in jenkinsfile

```
git add labs/pipeline/compose/Jenkinsfile        
git commit -m 'Added push'
git push gogs main
```

build now

Check the output - if you got the creds right, you should see a new image in Docker Hub with the version number of the build in the tag, e.g. https://hub.docker.com/repository/docker/courselabs/rng-api/tags?page=1&ordering=last_updated

## Lab

- add release and  `latest` tags and push

Goal - multiple tags, same digest:

![](/img/pipeline-lab.png)

Pull the latest iamge. Can you find the build information and Git hash from the metadata?

