

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

- whoami app


## Building with Docker Compose

- rng app

## Pushing to Docker Hub

- save creds
- uncomment push in jenkinsfile

## Lab

- add `latest` tag and push