
simplest is to copy the original job

http://localhost:8080/view/all/newJob

- name= hello-world
- type=pipeline
- copy from=manual-gate

Under _Build triggers_ select _Poll SCM_ and enter this in the _Schedule_ box:

```
* * * * * 
```

> This means Jenkins will check the Git repo every minute, and if there have been any changes since the last build then a new one is triggered

change script path to `labs/jenkins/hello-world/Jenkinsfile`

save & build

Check the logs - the test stage fails:

_Error: Could not find or load main class HelloWorkd.java_

It's a typo :)

Change `HelloWorkd.java` to `HelloWorld.java` in the file labs/jenkins/hello-world/Jenkinsfile 

then commit and push your changes:

```
git add labs/jenkins/hello-world/Jenkinsfile

git commit -m 'Lab solution'

git push labs-jenkins
```

Back in Jenkins, wait for the build to trigger from the SCM change (or click _Build Now_). All will be well. Check the output page for the build and you'll see a list of archived artifacts, containing the Java class file.