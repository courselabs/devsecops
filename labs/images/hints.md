# Lab Hints

Don't worry about installing Java, remember the official [OpenJDK](https://hub.docker.com/_/openjdk) image does that for you. The app is built for Java 8, but it runs fine on newer versions.

The Dockerfile should be straightforward, but you need to think about the paths. The Java class file may be in a different directory from your Dockerfile, and Docker needs to access both from the context.

Also the Java command inside the container needs to use the correct path to find the class file you copy into the image.

> Need more? Here's the [solution](solution.md).