# GitOps with ArgoCD

GitOps turns the CI/CD approach on its head - instead of an external automation server pushing changes into the cluster, a tool inside the cluster monitors a Git repo and pulls in any changes.

It means your cluster can be secured more cleanly, because you don't need a cluster admin user for the automation server. It also means your app definitions - and even the infrastructure setup for cloud services - can be definitively stored in Git repos. You can recreate your whole deployment from scratch.

## Reference

- [GitOps](https://www.gitops.tech) - describing the approach and its adoption

- [ArgoCD](https://argo-cd.readthedocs.io/en/stable/) - the GitOps project in the CNCF

- [ArgoCD command line](https://argoproj.github.io/argo-cd/user-guide/commands/argocd/)

- [Application CRD spec](https://argoproj.github.io/argo-cd/operator-manual/application.yaml)

- [GitOps with Kubernetes and Argo](https://eltons.show/episodes/ecs-c3/) - my YouTube walkthrough

## Install ArgoCD

ArgoCD has a server-side component which runs in the cluster and watches the configured Git repos for your projects. There's also a command line tool which you can use to set up and manage projects.

Start by installing the CLI - use the [full install docs](https://argo-cd.readthedocs.io/en/stable/cli_installation/) or one of these quick options:

```
# Windows:
choco install argocd-cli

# Mac:
brew install argocd

# Linux:
curl -sSL -o /usr/local/bin/argocd https://github.com/argoproj/argo-cd/releases/latest/download/argocd-linux-amd64
chmod +x /usr/local/bin/argocd
```

Check your installation with:

```
argocd version
```

> You'll see the client version and then a server error - the CLI can't connect to the ArgoCD server, because we haven't deployed it yet.

There's a local copy of the Argo CD spec here:

- [argocd/2.1.2.yaml](./specs/argocd/2.1.2.yaml) - it installs a lot of resources, including CustomResourceDefinitions - CRDs - which extend the functionality of Kubernetes. ArgoCD adds an `Application` resource to the cluster.

Deploy the server components:

```
kubectl apply -n argocd -f labs/argo/specs/argocd
```

Argo installs a new custom object type called _Application_.

📋 List all of the application objects in the cluster.

<details>
  <summary>Not sure how?</summary>

Custom objects can be used in Kubectl like ordinary objects:

```
kubectl get applications
```

</details><br/>

This installation of ArgoCD includes a web UI. The initial admin password is stored in a Secret - run this to view the password in plain text:

```
kubectl -n argocd get secret argocd-initial-admin-secret -o go-template="{{.data.password | base64decode}}"
```

Open the UI at http://localhost:30881, log in with username `admin` and the password from your Secret.

> You'll be redirected to HTTPS with a self-signed certificate, so you'll need to accept the security warning in your browser.

Open https://localhost:30881/settings/clusters - ArgoCD is registered with the local cluster so it can manage applications, but there are no apps yet.

## Configure the Git server

ArgoCD deploys apps as units which are configured with a source code repo to watch. The contents of the repo can be standard Kubernetes YAML, Helm charts or Kustomize. 

ArgoCD monitors the repo, and whenever there is a change - so the running app is out of sync with the specifications in the source repo - it fetches the changes and updates the app.

We'll run a local Git server to make the deployment simple:

- [gogs/gogs.yaml](./specs/gogs/gogs.yaml) is the same Gogs server we've used before, but configured to run in Kubernetes 

Deploy the Git server:

```
kubectl apply -f labs/argo/specs/gogs
```

Browse to http://localhost:30300/ and when the site is ready, sign in with username `courselabs` and password `student`.

Add the new Git server as a remote for this repo and push a copy of the content:

```
# add the local Git server:
git remote add labs-argo http://localhost:30300/courselabs/labs.git

# push to the expected branch name:
git push labs-argo main:master
```

> This version of ArgoCD expects to find a branch named `master` in the Git repo. This repo uses `main` as the branch name, so the push command uses the expected name in the Git server.

Now connect the ArgoCD CLI to the ArgoCD server, **using your password from the Secret**:

```
argocd login localhost:30881 --insecure --username admin --password <your-password>

argocd cluster list
```

> You can add new clusters to deploy to a remote Kubernetes cluster. Apps can be managed with the CLI or with the UI.

## Deploy an application

Create an application for the app in the `labs/argo/project/whoami` folder - that folder contains a simple whoami app spec:

```
argocd app create whoami --repo http://gogs.infra.svc.cluster.local:3000/courselabs/labs.git --path labs/argo/project/whoami --dest-server https://kubernetes.default.svc --dest-namespace default --sync-policy auto --self-heal
```

> Creating the app stores the definition in Kubernetes, and we've set the sync policy to _auto_ - so it will be deployed straight away.

📋 Check the details of the new application with the Argo CLI.

<details>
  <summary>Not sure how?</summary>

Applications are just Kubernetes objects - you can query them with Kubectl. But you get the key information in a readable format from the Argo CLI:

```
argocd app list

argocd app get whoami
```

</details><br/>


You can also see the new application in the UI at https://localhost:30881/applications. You'll see the status is _Synced_ and if you open the app you'll see all the Kubernetes resources.

ArgoCD deploys the app from the YAML specs in the repo - with no pipeline or scripts to maintain. You can test the app at http://localhost:30010/.


## Lab

Now let's see GitOps in action, making a change to the Docker image tag for the app, so it triggers a rollout with new Pods.

Edit the Kubernetes [deployment.yaml](./project/whoami/deployment.yaml) file and make two changes:

- set the image tag to `courselabs/whoami-lab:21.09-4`
- set the replica count to 3

Trigger the update by pushing your changes to the local Git server.

This ArgoCD application is set to self-heal, which means if an administrator removes any resources, ArgoCD will recreate them. Confirm that by deleting the Kubernetes Deployment object.


> Stuck? Try [hints](hints.md) or check the [solution](solution.md).

___

## Cleanup

Deleting apps in ArgoCD doesn't remove the running application - unless you use the cascade flag:

```
argocd app delete lab whoami --cascade
```

> ArgoCD asks you to confirm you really want to do this :)

Then delete the lab namespaces, which will remove ArgoCD and Gogs:

```
kubectl delete ns -l kubernetes.courselabs.co=argo
```