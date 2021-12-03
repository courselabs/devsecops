# Lab Solution

In my solution I've set the replica count and container image here: [lab/deployment.yaml](./lab/deployment.yaml).

You can copy the contents of that file into [project/whoami/deployment.yaml](./project/whoami/deployment.yaml) and then push your changes:

```
git add labs/argo/project/whoami/deployment.yaml

git commit -m 'Bump to build -4'

git push labs-argo main:master
```

If you quickly browse back to the ArgoCD web page at https://localhost:30881/applications/whoami you'll see the two old Pods being removed and three new Pods being started.

To test the self-healing feature, you can delete the Deployment, which will delete all the Pods:

```
kubectl delete deploy whoami
```

Check the web UI again and you'll see new Pods being created to replace the deleted ones. 

Test the app at http://localhost:30010 and you'll see its still working.
