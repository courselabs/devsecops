# Lab Solution

The second part of the GitHub build uses the `latest` Docker Compose file:

- [release.yml](./rng/release.yml) uses different image tags, with the `RELEASE` environment variable but not the `BUILD_NUMBER` variable the main Compose file uses

When you merge in the latest file it will build images with the tag `21.05`, which is the version for this release of the app.

Consumers can use `21.05` to get the current build for this release, or e.g. `21.05-33` to get a specific build:

```
docker pull courselabs/rng-api:21.05

docker image ls courselabs/rng-api
```

Those two tags are aliases of the same image now, but with the next release the `21.05` tag will advance and will be an alias of the a later build.