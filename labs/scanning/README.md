
References

- [Trivy documentation](https://aquasecurity.github.io/trivy/v0.21.1/)

- [Trivy - on GitHub](https://github.com/aquasecurity/trivy) - open source scanning engine

- [Trivy policies for misconfiguration](https://aquasecurity.github.io/trivy/v0.21.1/misconfiguration/policy/builtin/)



££ Image Scanning with Trivy

Run a Trivy container:

```
docker run -it --entrypoint sh -v C:\scm\github\courselabs\devsecops:/src aquasec/trivy:0.21.1
```

Scan an image on Docker Hub:

```
trivy

trivy image sixeyed/whoami:21.04
```

> downloads vulnerabilities db first time round

no vulns in this image!


How about this one:

```
trivy image python:3.4-alpine
```

Lots! Including this [buffer over-read vulnerability](https://avd.aquasec.com/nvd/cve-2019-15903) which doesn't sound good.



££ Library Scanning

Public repos only:

```
trivy repository https://github.com/sixeyed/kiamol
```

> Critical issue with the AWS SDK...

££ Infrastructure-as-Code Scanning

```
trivy config /src/labs/scanning/docker
```

> Dockerfile best practices

```
trivy config /src/labs/scanning/kubernetes
```


££ Running Trivy in Jenkins Pipelines


pipeline stage:

1- scan docker before build

```
trivy config --exit-code 1 --severity HIGH /src/labs/scanning
```

2 - scan image after build

3 - scan k8s before deploy

££ Lab

Fix the build...