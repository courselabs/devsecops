
Change pipeline to script in `labs/pipeline/lab/Jenkinsfile`

Build pushes specific build version, release version and latest.


My approach - separate build step, risks being out of date (small risk); means you still get the versioned build if there are other failures (debatable)


```
docker pull courselabs/rng-api:latest

```

>

```
"Labels": {
                "build_tag": "compose-7-origin/main",
                "commit_sha": "e2ebdf2ca8bf55d5f38c1095ffcad55bc7866039"
            }
```

Tag includes the Jenkins job name, build number and git branch 
