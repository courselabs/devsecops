# Lab Solution

The Dockerfile is a simple update to use the golden image in the final stage:

- [lab/Dockerfile](lab/Dockerfile) - uses `courselabs/java:jre-21.12 ` for the runtime

The POM should be updated so that Micrometer uses an explicit version instead of a range, and the JAR plugin uses the latest:

- [lab/pom.xml](lab/pom.xml) - uses the latest versions at the time of writing :)

You can build the sample solution with a new tag:

```
docker build -t app-3:v2 ./labs/golden-images/lab/
```

Check the app images:

```
docker image ls app-3
```

On my system the v2 image is about 30% smaller:

```
REPOSITORY   TAG       IMAGE ID       CREATED         SIZE
app-3        v1        90f85a6e8be0   2 minutes ago   221MB
app-3        v2        cbc2c31a0685   2 minutes ago   184MB
````

And if you inspect the images to see the layers:

```
docker inspect app-3:v1

docker inspect app-3:v2
```

You'll see v1 has a different set of layers - no commonality with any of the other Java apps we've built. v2 shares the same base layers as the v2 images from app-1 and app-2, so we're maximizing the layer cache as well as enforcing an approved base image.

___
> Back to the [exercises](README.md).