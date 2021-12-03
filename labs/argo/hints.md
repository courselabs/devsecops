# Lab Hints

You'll need to edit the deployment.yaml file to update the settings. ArgoCD is watching the Gogs server for changes, so you'll need to commit your change and push it to the Gogs remote.

Deleting objects is a standard command in Kubectl - you can use `get` to list the objects first and find the name of the one you need to delete.

> Need more? Here's the [solution](solution.md).