TODO - add to Jenkins Dockerfile:


```

mkdir -p ~/.docker/cli-plugins/ && \

wget -O ~/.docker/cli-plugins/docker-compose https://github.com/docker/compose/releases/download/v2.2.0/docker-compose-linux-x86_64   && \

 chmod +x ~/.docker/cli-plugins/docker-compose
```

## Building Docker Images with Jenkins

docker-compose -f labs\pipeline\infra\docker-compose.yml up -d

- docker pipe

http://localhost:3000

create repo devsecops

git remote add gogs http://localhost:3000/courselabs/devsecops.git

git push gogs main


- jenkinsfile

http://localhost:8080

Create a new pipeline

- call it docker
- select _Pipeline script from SCM_ in the dropdown
- SCM = _Git_
- Repository URL = `http://gogs:3000/courselabs/devsecops.git`
- Branch Specifier = `refs/heads/main`
- Script path = `labs/pipeline/docker/Jenkinsfile`

## Multistage Docker Builds

- docker build for ^^

- new pipeline called multi-stage
- copy from `docker`
- script path = `labs/pipeline/multi-stage/Jenkinsfile`


## Building with Docker Compose

- rng app - compose file

- new pipeline called compose
- copy from `docker`
- script path = `labs/pipeline/compose/Jenkinsfile`


## Pushing to Docker Hub

- open global credentials store http://localhost:8080/credentials/store/system/domain/_/
- add credential, type _Username with password_
-- yoour username and password
-- id `docker-hub`

- uncomment push in jenkinsfile

## Lab

- add `latest` tag and push