

## Developer Free-For-All

```
docker-compose -f labs\golden-images\apps\docker-compose.yml build
```

docker image ls courselabs/app*

docker inspect courselabs/app-1:v1

docker inspect courselabs/app-2:v1

> Layers - lots and completely divergent 

## A Golden Image Library

Debian version - based on official, OS is latest

Apline version - full SBOM

Build:

```
docker-compose -f .\labs\golden-images\library\docker-compose.yml build --pull
```

Check:

```
docker image ls courselabs/java*
```

Alpine marginally smaller:

```
REPOSITORY        TAG                IMAGE ID       CREATED              SIZE
courselabs/java   jdk-21.12          dbe839e1d807   About a minute ago   271MB       
courselabs/java   jre-21.12          ad98b2279e7f   About a minute ago   184MB       
courselabs/java   jre-debian-21.12   79786bc38074   2 weeks ago          221MB       
courselabs/java   jdk-debian-21.12   0d6d8cde8cc0   2 weeks ago          422MB
```

## Using Golden Images


```
docker-compose -f labs\golden-images\apps\docker-compose-v2.yml build
```

docker image ls courselabs/app*

docker inspect courselabs/app-1:v2

docker inspect courselabs/app-2:v2

> Layers - same count, same root layer


## Image Cache

docker image ls

docker system df

> Active images vs all + build cache

docker system prune -f

> safe, just dangling

--all will fully clear out images

## Lab

app 3 - maven; fix to use golden image for runtime; plus sbom for package list

- what version of micrometer is installed?
- is it the latest jar plugin?

docker build -t app-3:v1 labs\golden-images\apps\app-3