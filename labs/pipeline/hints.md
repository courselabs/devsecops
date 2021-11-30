# Lab Hints

Look closely at the Docker Compose commands in the Jenkinsfile and you'll see that they already load multiple Compose files to build up the full model.

You'll need **additional** build and push commands using the extra override files, because the images need to be built with the correct tag.

When you pull the `latest` image tag then you can inspect it to read the metadata. You'll find some useful information in the labels.

> Need more? Here's the [solution](solution.md).