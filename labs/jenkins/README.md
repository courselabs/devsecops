

## Running Jenkins

docker-compose -f labs\jenkins\docker-compose.yml up -d

Browse to Jenkins at http://localhost:8080

Sign in with:

- courselabs
- student

## Creating a Pipeline

Click _Create a job_ on home screen
call it lab-1
Select Freestyle Project
Click OK

Scroll down to build
Click _Add build step_
Select _Execute shell_

Paste sample script:

```
echo "This is build number: $BUILD_NUMBER of job: $JOB_NAME"
```

Click _Save_

In job window click _Build Now_

When the build finishes check the output http://localhost:8080/job/lab-1/1/console

Build again - how is the output different?

Same job name, new number = build number

There are other tools installed in Jenkins. What happens if you print the version of the Java compiler, the Docker command line or the Kubernetes command line in the script?

Back to project
_Configure_

```
echo "This is build number: $BUILD_NUMBER of job: $JOB_NAME"

docker version

javac --version

kubectl version
```

_Save_ and _Build Now_

Job fails - kubectl command errors because there is no server. If a command fails then the build exits.

> This is the old-school way of using Jenkins. There are better options

## Plugins and the New UI

Go to Jenkins homepage & select _Manage Jenkins_ then _Manage Plugins_

- Updates probably listed :)
- Click _Installed_ - some already in this install
- In search look for `blueocean`
- Tick `Blue Ocean`
- Click _Install without restart_

> Lots and lots of plugins get installed

When finished click _Go back to the top page_

New menu item - _Open Blue Ocean_; click
Browse to lab-1 and open a build; new UI for same job

Click back to main Blue Ocean UI http://localhost:8080/blue

Click _New pipeline_

Select Git

Repo URL http://gogs:3000/courselabs/labs.git

Creds: courselabs/student

_Create Credential_
_Create Pipeline_

Click _Create Pipeline_ again :)

Click the Plus icon next to the _Start_ block to add a new build stage

Name the stage test
Click _Add step_

Choose _Sell Script_ and paste:

```
echo "This is build number: $BUILD_NUMBER of job: $JOB_NAME"

javac --version
```

Click _Save_ and then _Save & run_

Click _Show branches_ and fine the build in master

The build should succeed - check the output to see the messages

> The difference is this build is stored in a text file in the git repo: http://localhost:3000/courselabs/labs/src/master/Jenkinsfile

Jenkins created that file. You can build it in the web UI, but more likely you will edit the text file directly.

## Storing Pipelines in Source Code

Open Gogs at http://localhost:3000

Sign in with courselabs/student

You'll see an existing repo called labs

Create a new repo called devsecops; leave all other options

```
git remote add jabs-jenkins http://localhost:3000/courselabs/devsecops.git

git push -u origin main
```

 - clone repo; edit pipeline

## Lab

create new repo - devsecops
- add remote
- push to gogs

- create new pipeline to build java app (fix path in jenkinsfile)

- could you build e.g. .net app?

