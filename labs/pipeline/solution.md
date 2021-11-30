# Lab Solution

You can see my solution in [lab/Jenkinsfile](./lab/Jenkinsfile):

- it adds a new stage to build and push latest tags
- the new stage has a build step with two Compose commands, to build the release tags and another to build the latest tags
- the second stage has two Compose commands, to push the release tags and the latest tags

It might seem wasteful to build the images again, but you'll see that Docker uses caching extensively and the new builds happen super quickly.

An alternative is to use the `docker image tag` command to add a new tag to an existing image and then pull it, but you can't use Compose with that so you'd need to hard-code the image names.

This approach continues to work even if you add new images to your Compose files.

Try pulling a `latest` image:

```
docker pull courselabs/rng-api:latest
```

Then you can inspect it:

```
docker inspect courselabs/rng-api:latest
```

In the labels you'll see the build tag and commit hash:

```
"Labels": {
                "build_tag": "compose-7-origin/main",
                "commit_sha": "e2ebdf2ca8bf55d5f38c1095ffcad55bc7866039"
            }
```

> The build tag includes the Jenkins job name, build number and git branch
