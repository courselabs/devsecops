# Lab Solution

Build the image and it will fail with a bunch of CVEs:

```
docker build -t hello-world-cs labs/scanning/hello-world-cs
```

This is a simple fix when you know what you're looking for :)

The application image is based on a .NET runtime image which uses Debian as the OS layer:

```
FROM mcr.microsoft.com/dotnet/runtime:6.0 AS app
```

All the issues Trivy finds are in OS tools and packages. Debian is the default OS for some runtimes because it has the widest compatibility - but it has a lot more packages with potential issues.

The Alpine OS is a better option, but you'll need to test your apps work correctly as there are some underlying differences between Alpine and other Linux distros.

Change the Dockerfile to use the Alpine runtime image - as in [Dockerfile.solution](./hello-world-cs/Dockerfile.solution):

```
FROM mcr.microsoft.com/dotnet/runtime:6.0-alpine AS app
```

Rebuild and there are no HIGH severity issues so the image gets built, and you can test it by running a container:

```
docker build -t hello-world-cs -f labs/scanning/hello-world-cs/Dockerfile.solution labs/scanning/hello-world-cs

docker run --rm hello-world-cs
```

Looks good :)

And we can check the version of Trivy to confirm this image has been scanned:

```
docker run --rm --entrypoint sh -it hello-world-cs

cat /app/scanner.txt

exit
```

___
> Back to the [exercises](README.md).