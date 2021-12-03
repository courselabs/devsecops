# Container Security

It's fairly straightforward to model your apps in Kubernetes and get them running, but there's more work to do before you get to production - and security is one of the main areas.

Application security in Kubernetes is mostly concerned with securing containers, which is mostly about apply Linux security measures. There are lots of controls you can apply, but not all applications support them.

You'll need to understand the OS features your apps need because applying controls can break them - Go applications might run fine on a read-only filesystem whereas Java apps will fail.

## References

- [Kubernetes Security guidance](https://kubernetes.io/docs/concepts/security/)

- [Managing resources for Pod containers](https://kubernetes.io/docs/concepts/configuration/manage-resources-containers/)

- [OAWSP cheat sheet for Kubernetes](https://cheatsheetseries.owasp.org/cheatsheets/Kubernetes_Security_Cheat_Sheet.html)

- [CKS - Certified Kubernetes Security Specialist certification](https://www.cncf.io/certification/cks/)

- [Cluster hardening guidance from NSA & CISA (pdf)](https://media.defense.gov/2021/Aug/03/2002820425/-1/-1/1/CTR_KUBERNETES%20HARDENING%20GUIDANCE.PDF)

## Removing access to the Kubernetes API server

The default Kubernetes security controls are not encouraging. 

Start by running the Pi web application and we'll use it to check for security issues:

```
kubectl apply -f labs/container-security/specs/pi
```

The Pi app runs in a single Pod, and you can check the site via the Service at http://localhost:30020.

There's nothing special in the [Deployment spec](./specs/pi/deployment.yaml) relating to security. 

By default Kubernetes mounts a volume in every Pod which contain an authentication token, so applications inside the Pod can use the Kubernetes API server. Applications like Prometheus and Fluentd need access to the API so they can find Pods and other objects - but almost no business apps need to do that.

The token is there in the Pi app:

```
kubectl exec deploy/pi-web -- cat /var/run/secrets/kubernetes.io/serviceaccount/token
```

That's all you need to authenticate with the Kubernetes cluster (you can use tokens like this with Kubectl), and then you'd have all the same permissions as the Pod.

This Pod is using the default [Service Account](https://kubernetes.io/docs/tasks/configure-pod-container/configure-service-account/) - which is a Kubernetes identity system Pods to access the API. Other users may have given that account extra permissions for their requirements, and if an attacker compromises the Pi app then they could have free access to read Secrets or delete running applications.

[fix-1.yaml](./specs/pi/fixes/fix-1.yaml) creates a custom ServiceAccount which is set to not mount the authentication token, and the Pod spec uses that account.

📋 Apply the fix. Can you read the token in the new Pod?

<details>
  <summary>Not sure how?</summary>

Send in the changes:

```
kubectl apply -f labs/container-security/specs/pi/fixes/fix-1.yaml
```

Wait for the new Pod to start:

```
kubectl get po -l app=pi-web --watch
```

And try to print the token:

```
# you'll see an error - No such file or directory
kubectl exec deploy/pi-web -- cat /var/run/secrets/kubernetes.io/serviceaccount/token
```

</details>

This closes one security hole. It won't stop determined attackers, but it doesn't make it so easy for them.

## Restricting Compute Resources

Container processes run directly on the OS of the host machine. By default they can access all the CPU cores and all the memory on that machine. In Kubernetes clusters you typically have lots of Pods running on each node, and if you don't restrict the compute resources then you're setting yourself up for a denial-of-service attack.

The Pi application is running without any compute restrictions. Try computing Pi at a high level of decimal places - **open two tabs for this URL** http://localhost:30020/pi?dp=100000. Check your machine's CPU - you might see it spike because computing Pi is hard work.

[fix-2.yaml](./specs/pi/fixes/fix-2.yaml) applies resource constraints:

- `requests` set the amount of CPU and memory your app needs to run; Kubernetes uses this when it decides which node should run the Pod, based on the amount of resources available on the nodes

- `limits` set the hard limits of CPU and memory the app can use. CPU is restricted, so if you set 0.5 cores then the app can run at 100% CPU and only max out 0.5 cores on the server. Memory is restricted to, and if the app tries to use more than the limit then the Pod can be killed with an out-of-memory error.

📋 Apply the fix. Does calculating Pi to 100K dp still cause a CPU spike?

<details>
  <summary>Not sure how?</summary>
  
Send in the changes:

```
kubectl apply -f labs/container-security/specs/pi/fixes/fix-2.yaml
```

Wait for the new Pod to start:

```
kubectl get po -l app=pi-web --watch
```

Browse to http://localhost:30020/pi?dp=100000 - you shouldn't see any impact on your machine's CPU but the page will take **much** longer to respond.

</details>

Resource limits are a simple way of preventing a denial-of-service attack, but you'll need to keep evaluating them over time to be sure your app has enough resources to work effectively (hopefully your app will get more performant over time and you can reduce limits with each release).

## Restricting OS capabilities

Container processes run on the server with an identity which maps to a user on the server. Lots of container images - even official images - use the `root` user for the application process.

This is very bad. If an attacker manages to break out of the container (exploits like this have been discovered), then they're the admin user on the server. That means they can attack other containers on the server, or access other machines on the network.

Check the user account for the Pi app:

```
kubectl exec deploy/pi-web -- whoami
```

It's `root` - and this image isn't built to be deliberately insecure. Check the [Dockerfile](https://github.com/sixeyed/kiamol/blob/master/ch05/docker-images/pi/Dockerfile) - it uses Microsoft's official ASP.NET runtime image with no special setup.

Container processes also have more [Linux capabilities](https://kubernetes.io/docs/tasks/configure-pod-container/security-context/) than they need, with the standard Kubernetes configuration. `chown` is a Linux command which lets you change the ownership of files - it's available in the Pi container:

```
kubectl exec deploy/pi-web -- chown root:root /app/Pi.Web.dll
```

[fix-3.yaml](./specs/pi/fixes/fix-3.yaml) sets the container to run as a non-root user, drops all the extra Linux capabilities (the app doesn't need them), and stops processes asking for extra privileges.

📋 Apply the fix. How is the app looking?

<details>
  <summary>Not sure how?</summary>
  
Send in the changes:

```
kubectl apply -f labs/container-security/specs/pi/fixes/fix-3.yaml
```

Wait for the new Pod to start:

```
kubectl get po -l app=pi-web --watch
```

Browse to http://localhost:30020/ - the app still works.

</details>

This app continues to run as expected, but for some applications removing permissions means they won't start - listening on port 80 might fail with the restricted capabilities.

[fix-4.yaml](./specs/pi/fixes/fix-4.yaml) configures the app to listen on port 5001 inside the container, so that removes the need for privileged access to port 80 - the application needs to support this.

___
## Lab

This is not the end of security - it's only the beginning. Securing containers is a multi-layered approach which starts with your securing your images, but the hardened Pi app is a good step up from the default Pod security.

Not every app will work with those controls applied though. Here's a [Deployment for the whoami app](./specs/whoami/deployment.yaml) with all the security controls commented out.

Your job is to work out which set of controls can be applied without breaking the app. Start by deploying it as-is:

```
k apply -f labs/container-security/specs/whoami
```

Verify you can use the app at http://localhost:30022/. Then add in the security controls until you have as many enabled as you can, with the app still working.

> Stuck? Try [hints](hints.md) or check the [solution](solution.md).

___
## Cleanup

```
kubectl delete all -l kubernetes.courselabs.co=container-security
```