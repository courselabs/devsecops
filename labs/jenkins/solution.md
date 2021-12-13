# Lab Solution

The simplest way is to copy the previous job:

- browse to http://localhost:8080/view/all/newJob
- set the job name to be `hello-world`
- select _Pipeline_ as the job type
- in the _Copy from_ box, enter `manual-gate`

In the build definintion, scroll to _Build triggers_ and select _Poll SCM_. That sets up a schedule to check for changes from the Git server; enter this in the _Schedule_ box:

```
* * * * * 
```

> This means Jenkins will check the Git repo every minute, and if there have been any changes since the last build then a new one is triggered.

Change the script path to `labs/jenkins/hello-world/Jenkinsfile`, then save and build the job - it fails.

Check the logs or the console output - the _Test_ stage fails with this log line:

_Error: Could not find or load main class HelloWorkd.java_

`HelloWorkd`> Looks like a typo :)

Change the line `java HelloWorkd` in the Jenkinsfile to `java HelloWorld` in the file `labs/jenkins/hello-world/Jenkinsfile`. My fixes are in [this Jenkinsfile](./lab\Jenkinsfile)

Then commit and push your changes. Back in Jenkins, wait for the build to trigger from the SCM change (or click _Build Now_). 

All will be well. Check the output page for the build and you'll see a list of archived artifacts, containing the Java class file.

___
> Back to the [exercises](README.md).