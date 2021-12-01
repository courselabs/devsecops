# Lab Solution

The default registry is Docker Hub - using the domain `docker.io`, and the default image tag is `latest`.

So `kiamol/ch05-pi` is the short form of `docker.io/kiamol/ch05-pi:latest`:

```
docker pull kiamol/ch05-pi

docker pull docker.io/kiamol/ch05-pi:latest
```

Check the image list and you'll only see one - these aren't aliases, they're just different froms of the same name:

```
docker image ls kiamol/ch05-pi
```

> Be wary of using `latest` images - it's a confusing name because it might not be the latest version.

For other registries you need to include the domain in the reference - so images on MCR need to be prefixed with `mcr.microsoft.com/`:

```
docker pull mcr.microsoft.com/dotnet/runtime:5.0
```
