# Lab Hints

This is just a standard Docker build. The final app stage copies a text file from the scan stage - that's to ensure the scan runs when you're using BuildKit.

The `FROM` image for the application stage uses the .NET runtime, but's it's not a minimal image. Check the available [.NET runtime images on Docker Hub](https://hub.docker.com/_/microsoft-dotnet-runtime/) to find a 6.0 release based on a minimal OS.

> Need more? Here's the [solution](solution.md).