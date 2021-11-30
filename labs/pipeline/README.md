# Building Docker Images with Jenkins

You can run containers which have access to the Docker engine - so the app inside the container can use the Docker CLI to run other containers or build images. That makes for a nice clean CI/CD setup, where you run Jenkins in a container using pipelines which build your applications with multi-stage Dockerfiles. The pipelines can also run containers from the new images to smoke test the functionality.

With this approach your whole build infrastructure is portable. You can run a Jenkins server on any Docker engine, load in your pipelines and build any type of software without installing any other tools. You can also save credentials in Jenkins so you can authenticate with other services - then you can push images to Docker Hub or deploy to your production environment from your pipeline.

## References

- [Accessing the Docker Engine from inside a container]()

- [Saving credentials in Jenkins]()

- [Working with credentials in pipelines]()

## Build a Simple Java Image

We'll start by building [this Dockerfile](labs\pipeline\docker\Dockerfile), using [this Jenkinsfile](labs\pipeline\docker\Jenkinsfile). That will package a pre-built Java app into a Docker image and run a container from the image. If the build or the test stages fail then the pipeline fails.

Start by running Jenkins and Gogs using this [Docker Compose spec](labs\pipeline\infra\docker-compose.yml):

```
docker-compose -f labs\pipeline\infra\docker-compose.yml up -d
```

> The Jenkins service is configured to mount the Docker socket, which is how the container can talk to the Docker Engine where it's running.

This is a new Gogs server, so we'll need to set it up for our repo:

- browse to http://localhost:3000

- sign in with username `courselabs` and password `student`

- click the plus icon `+` in the _My Repositories_ section to create a new repo

- call the repo `devsecops` and click _Create Repository_

Add your Gogs server as a remote and push the repo to it:

```
# you may already have this set up from the Jenkins lab - ignore any errors you see:
git remote add gogs http://localhost:3000/courselabs/devsecops.git

git push gogs main
```

Now we can create a pipeline in Jenkins with:

- job name = `docker`
- Git URL = `http://gogs:3000/courselabs/devsecops.git`
- branch specifier = `refs/heads/main`
- Jenkinsfile path = `labs/pipeline/docker/Jenkinsfile`

📋 Open the Jenkins UI and create the new pipeline job.

<details>
  <summary>Not sure how?</summary>

- browse to http://localhost:8080
- log in with username `courselabs` and password `student`
- click on _New item_ in the left nav
- select _Pipeline_ as the project type
- in the pipeline window select _Pipeline script from SCM_ in the dropdown
- enter the pipeline details

</details>

Save and run the build.

📋 Check the build output. What is the name of the Docker image that it built? And what is the output from the test container?

<details>
  <summary>Not sure?</summary>

Open the logs from the _Build_ stage and you'll see the image tag is `docker.io/courselabs/hello-world:1`.

In the _Test_ stage logs you'll see the container output is the string `Hello, World`.

</details>

This is a simple example, but we don't want to just package binaries into images - we want to build them from source code, which we can do with a multi-stage builds.

## Multi-stage Builds in Jenkins

There's nothing special about multi-stage builds - any Docker builds we can run on the laptop work just the same in the Jenkins container:

- [multi-stage/Dockerfile](.\multi-stage\Dockerfile) - builds the same Java app from source, so we no longer need a separate compilation step

- [multi-stage/Jenkinsfile](.\multi-stage\Jenkinsfile) - builds the image using environment variables to generate the tag, then runs a test container

Those files are already in your Gogs server, so you can create a new pipeline in Jenkins to run the script in `labs/pipeline/multi-stage/Jenkinsfile`.

📋 Create and run a new pipeline called `multi-stage`.

<details>
  <summary>Not sure how?</summary>

Create a new item in the Jenkins UI at http://localhost:8080/view/all/newJob, call it `multi-stage`, set the type to be _Pipeline_ and enter `docker` in the box to _Copy from_ an existing pipeline.

Change the script path to `labs/pipeline/multi-stage/Jenkinsfile`.

Click _Save_ and then _Build Now_.

</details>

The build runs with the familiar stages we've been using. Remember that Jenkins is sharing your Docker Engine, so when it builds images they're available for you to use in `docker` commands.

📋 What is the size of the new image from the build? How does it compare to the SDK?

<details>
  <summary>Not sure?</summary>

You can list images on your machine to see the details:

```
# image built from pipeline:
docker image ls courselabs/multi-stage:1

# java sdk image from Docker Hub:
docker pull openjdk:11-jdk-slim
docker image ls openjdk:11-jdk-slim 
```

</details>

> You should see your application image is about half the size of the SDK image.

## Building with Docker Compose

This Jenkins container image also has the `docker compose` command installed, so we can build multiple images from one command.

We'll be building the random number app from source:

- [compose/Jenkinsfile](.\compose\Jenkinsfile) - builds the app in the `labs/compose-build/rng` folder; the _Push_ stage is commented out so only the _Build_ stage will run

- [the API Dockerfile](../compose-build\rng\docker\api\Dockerfile) - is a multi-stage build using the .NET SDK and runtime images

So this is a .NET app. To be sure we're not cheating, run this command inside the Jenkins container to confirm that .NET is not installed:

```
# will fail
docker exec infra_jenkins_1 dotnet --version
```

> You'll see a `not found in $PATH` error because there is no .NET in the container. That's OK though because it's a multi-stage build and the compilation will run inside containers.

📋 Create and run a new pipeline called `compose` from the script path `labs/pipeline/compose/Jenkinsfile`.

<details>
  <summary>Not sure how?</summary>

Create a new item in the Jenkins UI at http://localhost:8080/view/all/newJob, call it `compose`, set the type to be _Pipeline_ and enter `docker` in the box to _Copy from_ an existing pipeline.

Change the script path to `labs/pipeline/compose/Jenkinsfile`.

Click _Save_ and then _Build Now_.

</details>

Check the output - the build should run successfully and generate images.

📋 How many images are built? Where does the `21.12` part of the image tag come from? 

<details>
  <summary>Not sure?</summary>

The logs from the _Build_ stage will show two images being built: `courselabs/rng-api:21.12-1` and `courselabs/rng-web:21.12-1`.

`21.12` is the release cycle, which is set in an environment variable in the Jenkinsfile.

</details>

> Environment variables are used for the registry and repository names in the image tag, so they can be easily edited to change where the images get pushed.

## Pushing to Docker Hub

The publish part of our pipeline will push images to Docker Hub. For that you'll need three things:

- a [Docker Hub account](https://hub.docker.com) - create one if you don't already have one; the free tier is fine
- your Docker Hub credentials stored in Jenkins
- a change to the Jenkinsfile so the image tags have your Docker Hub username

When you have a Docker Hub account, you can browse to [your settings](https://hub.docker.com/settings/security) and generate an access token. Copy the token to your clipboard - you can use it to log in so you don't store your actual password in Jenkins.

Now create a credential in Jenkins:

- browse to http://localhost:8080/credentials/store/system/domain/_/
- click _Add Credentials_ and make sure the _Kind_ is _Username with password_
- enter your Docker Hub username (**not** your email address) in the _Username_ field
- enter your access token in the _Password_ field
- call the credential `docker-hub` in the _ID_ field

Next edit the [Jenkinsfile](compose/Jenkinsfile):

- replace the value of the environment variable `REPOSITORY="courselabs"` with your own Docker Hub ID
- e.g. my Hub ID is `sixeyed` so my updated setting will read `REPOSITORY="sixeyed"`
- uncomment the _Push_ stage - delete the start `/*` and end `*/` comments

Push your changes:

```
git add labs/pipeline/compose/Jenkinsfile        
git commit -m 'Added push'
git push gogs main
```

📋 Build the `compose` job again and verify the images are built and pushed to Docker Hub under your username.

<details>
  <summary>Not sure how?</summary>

Check all the build stages. If the _Push_ stage fails, check the logs. The issue will be a problem authenticating to Docker Hub:

- check your `docker-id` credentials in Jenkins are correct
- check your `REPOSITORY` environment variable matches the Docker Hub ID in the credentials
- verify the images are being built with the expected image name, containing your Docker Hub ID

If you get an error message it should be clear what the issue is. When you get the build working you'll see images being pushed to Docker Hub, like mine at https://hub.docker.com/r/sixeyed/rng-api/tags

</details>

> You probably don't have Jenkins, Gogs or .NET installed on your machine. But now you can build and push Docker images for a .NET app with a fully automated pipeline, which brings in all the dependencies it needs.


## Lab

Docker image tags are typically used for versioning. Our images contain a release cycle and a build number as the image version. We also want to provide less specific versions of the image, e.g. 

- `courselabs/rng-api:21.12-3` - is build 3 of the 21.12 release
- `courselabs/rng-api:21.12` - is the latest build of the 21.12 release
- `courselabs/rng-api:latest` - is the latest build of the latest release

`latest` is the default tag, so users can run a container from `courselabs/rng-api` to get the latest version, or `courselabs/rng-api:21.12` to get the latest build for the release, or `courselabs/rng-api:21.12-3` to run a specific build.

There are some additional Compose overrides we can use to build images with the extra tags:

- [labs\compose-build\rng\release.yml](..\compose-build\rng\release.yml)
- [labs\compose-build\rng\latest.yml](..\compose-build\rng\latest.yml)

Extend the [Jenkinsfile](compose/Jenkinsfile) to add those tags and push them to Docker Hub as part of the build. The goal is for each build to push the specific image tag and update the other tags, so they work as aliases for the same image digest:

![](/img/pipeline-lab.png)

You'll need to push your Jenkinsfile changes to Gogs and run a new build in Jenkins. When you have it all working, pull the `latest` image using Docker on your machine. Can you find the build information and Git hash from the metadata?

> Stuck? Try [hints](hints.md) or check the [solution](solution.md).

___
## Cleanup

Cleanup by removing all containers:

```
docker rm -f $(docker ps -aq)
```