# Preparing for Production

It's straightforward to model your apps in Kubernetes and get them running, but there's more work to do before you get to production.

Kubernetes can fix apps which have temporary failures, automatically scale up apps which are under load and add security controls around containers.

These are the things you'll add to your application models to get ready for production.

## References


## Restricting Compute Resources

TODO


## Container security 

Security is a very large topic in containers, but there are a few features you should aim to include in all your specs:

- changing the user to ensure the container process doesn't run as `root`
- don't mount the Service Account API token unless your app needs it
- add a [Security Context](https://kubernetes.io/docs/concepts/security/pod-security-standards/) to limit the OS capabilities the app can use

Kubernetes doesn't apply these by default, because they can cause breaking changes in your app.

```
kubectl exec deploy/pi-web -- whoami

kubectl exec deploy/pi-web -- cat /var/run/secrets/kubernetes.io/serviceaccount/token

kubectl exec deploy/pi-web -- chown root:root /app/Pi.Web.dll
```

> The app runs as root, has a token to use the Kubernetes API server and has powerful OS permissions

This alternative spec fixes those security issues:

- [pi-secure/deployment.yaml](labs/container-security/specs/pi-secure/deployment.yaml) - sets a non-root user, doesn't mount the SA token and drops Linux capabilities

```
kubectl apply -f labs/container-security/specs/pi-secure/

kubectl get pod -l app=pi-secure-web --watch
```

> The spec is more secure, but the app fails. Check the logs and you'll see it doesn't have permission to listen on the port.

Port 80 is privileged inside the container, so apps can't listen on it as a least-privilege user with no Linux capabilities. This is a .NET app which can use a custom port:

- [deployment-custom-port.yaml](specs/pi-secure/update/deployment-custom-port.yaml) - configures the app to listen on non-privileged port 5001

📋 Deploy the update and check it  fixes those security holes.

<details>
  <summary>Not sure how?</summary>

```
kubectl apply -f labs/container-security/specs/pi-secure/update

kubectl wait --for=condition=Ready pod -l app=pi-secure-web,update=ports
```

The Pod container is running, so the app is listening, and now it's more secure:

```
kubectl exec deploy/pi-secure-web -- whoami

kubectl exec deploy/pi-secure-web -- cat /var/run/secrets/kubernetes.io/serviceaccount/token

kubectl exec deploy/pi-secure-web -- chown root:root /app/Pi.Web.dll
```

</details><br/>

This is not the end of security - it's only the beginning. Securing containers is a multi-layered approach which starts with your securing your images, but this is a good step up from the default Pod security.



___
## Lab

Adding production concerns is often something you'll do after you've done the initial modelling and got your app running. 

So your task is to add container probes and security settings to the configurable app. Start by running it with a basic spec:

```
kubectl apply -f labs/container-security/specs/configurable
```

Try the app and you'll see it fails after 3 refreshes and never comes back online. There's a `/healthz` endpoint you can use to check that. Your goals are:

- run 5 replicas and ensure traffic only gets sent to healthy Pods
- restart Pods if the app in the container fails
- add an HPA as a backup, scaling up to 10 if Pods use more than 50% CPU.

This app isn't CPU intensive so you won't be able to trigger the HPA by making HTTP calls. How else can you test the HPA scales up and down correctly? 

> Stuck? Try [hints](hints.md) or check the [solution](solution.md).




___
## Cleanup

```
kubectl delete all,hpa -l kubernetes.courselabs.co=container-security
```