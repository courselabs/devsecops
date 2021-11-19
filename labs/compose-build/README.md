# Building Apps with Compose

Docker Compose models can include build-time details, so you can build multiple container images using one Docker Compose command. Using override files lets you customize the image references and set labels to track the image back to the source code.

## Reference

- [Compose build spec](https://docs.docker.com/compose/compose-file/compose-file-v3/#build)

- [Using environment variables in Compose files](https://docs.docker.com/compose/environment-variables/) 

<details>
  <summary>CLI overview</summary>

Docker Compose has commands to work with images:

```
docker-compose build --help

docker-compose push --help
```

> These support multiple YAML files in the same way as the other commands.

</details><br/>

## Building with Compose

All the source code for the random number app we've been usingis in the `rng` folder, along with the Dockerfile and a Compose file: 

- [rng\docker-compose.yml](.\rng\docker-compose.yml) - includes the context and Dockerfile paths

All paths in Compose are relative to the location of the Compose file. 

📋 Switch to the `rng` folder and build the images.

<details>
  <summary>Not sure how?</summary>

```
cd labs/compose-build/rng

docker-compose build 
```

</details><br/>

> The build command builds all the images with a `build` section in the spec. You'll see the output from your configured build engine - you can use the original or BuildKit for this lab

These images have the tag `21.05-local`. You can use the same Compose file to run the app from your local images.

📋 Run the app using your new images and test it works.

<details>
  <summary>Not sure how?</summary>

```
docker-compose up -d

# try the app at http://localhost:8090
```

</details><br/>

> The Compose spec has all the details to run and build the app. 

## Build arguments and image labels

A single Compose file to build and run your app is very appealing, but run and build options are very different and it's usually easier to split them to keep your Compose specs easier to read and maintain:

- [core.yml](./rng/core.yml) - defines the core services and networks for the random number app

- [build.yml](./rng/build.yml) - defines the build options for the services, including the path to the build context and the path to the Dockerfile

And with some additional config you can add some useful auditing to your images:

- the [rng API Dockerfile](./rng/docker/api/Dockerfile) uses `ARG` instructions - which are values you can set as build arguments - to add metadata to the image, using labels to record the build version and Git commit ID 

- _we haven't covered all the details of how this Dockerfile works - we'll do that in [multi-stage builds](../multi-stage/README.md)_

- [args.yml](./rng/args.yml) overrides the image name and sets default values for the build arguments. All the `${VARIABLES}`  can be overridden by environment variables on the machine running the build.

📋 Join all those files to build the app, then inspect the labels for the API image.

<details>
  <summary>Not sure how?</summary>

```
# join all the files to get the full build spec:
docker-compose -f core.yml -f build.yml -f args.yml build

# this output shows label values:
docker image inspect --format '{{.Config.Labels}}' courselabs/rng-api:21.05-0
```

</details><br/>

> The default values from the Compose file set the label values.

You can set environment variables on your machine which will override the defaults in the Compose files:

```
# macOS or Linux:
export RELEASE=2021.07
export BUILD_NUMBER=121

# OR with PowerShell:
$env:RELEASE='2021.07'
$env:BUILD_NUMBER='121'
```

📋 Repeat the build. What are the new image tags? And the label values in the API image?

<details>
  <summary>Not sure how?</summary>

```
# it's the same set of files:
docker-compose -f core.yml -f build.yml -f args.yml build

# the tag is 2021.07-121

# show the new label values:
docker image inspect --format '{{.Config.Labels}}' courselabs/rng-api:2021.07-121
```

</details><br/>

> Different build argument values break the cache for this build, so you'll see the Dockerfile instructions being run again.

Those labels values aren't very useful when you build locally - but in a Continuous Integration build, they would be set with the correct values by the build service.

## Docker Compose for CI builds

Compose is great for running lots of non-production environments on a single machine, but if you're not planning to use it for that the build feature is perfect for Continuous Integration. You can easily build your apps in Jenkins or GitHub Actions just by running `docker-compose build`.

This repo also has a GitHub Actions workflow to build the RNG images using the same Docker Compose files you've been using locally:

- [rng-build.yml](../../.github/workflows/rng-build.yml) - GitHub workflows use a YAML spec, but if you're not familiar with them you'll see the same `docker-compose` commands being executed.

This is a public repo so you can browse to the workflow output:

https://github.com/courselabs/docker/actions/workflows/rng-build.yml

📋 Drill down into the latest build output and you'll see an image tag being pushed. Pull that image and inspect the labels.

<details>
  <summary>Not sure how?</summary>

```
# the build version is appended to the image tag, e.g for build 33:
docker pull courselabs/rng-api:21.05-33

docker image inspect courselabs/rng-api:21.05-33
```

</details><br/>

You'll see the actual build details stored in the image labels, something like this:

```
"Labels": {
  "build_tag": "RNG App Docker Image Weekly Build-33-refs/heads/main",
  "commit_sha": "dcd4b265f1406182a2e671b574af44100dbdfdab"
}
```

> This is from the same Dockerfiles and Compose files you use for a local build.

Image labels help you track back from a running container to the build pipeline which created it, and the exact version of the source code.

## Lab

Look closely at the GitHub workflow and you'll see it runs two sets of builds and pushes - the second set uses one more Compose override file.

What is the difference when you use the extra Compose file and why does the workflow run this second build?

Pull the API image from the tag in the second build and check if it's the same as the tag `21.05-33`.

> Stuck? Try [hints](hints.md) or check the [solution](solution.md).

___
## Cleanup

Cleanup by removing all containers:

```
docker rm -f $(docker ps -aq)
```
---