# Lab Hints

You can run a simple Docker build first to check the app compiles correctly and the container runs as expected.

Then you'll need to pass build arguments to run the analysis. Your SonarQube token is for authenticating with the server - you can use your original token for this new project.

After the first analysis run you can find the project in the SonarQube UI to set the quality gate, then you can run a build again with another argument to fail the build if the analysis fails.

> Need more? Here's the [solution](solution.md).