
dockerfile

pom.xml


docker build -t app-3:v2 .\labs\golden-images\lab\

docker image ls app-3

docker inspect app-3:v1

- yet another root

docker inspect app-3:v2

- same root as app-1 and app-2

REPOSITORY   TAG       IMAGE ID       CREATED         SIZE
app-3        v1        90f85a6e8be0   2 minutes ago   221MB
app-3        v2        cbc2c31a0685   2 minutes ago   184MB

both work the same :)

docker run app-3:v1

docker run app-3:v2