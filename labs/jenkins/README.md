# Automation with Jenkins

[Jenkins](https://jenkins.io) is probably the most popular automation server around - but popular in terms of being well used rather than being liked. It's been around a long time and one of the main features is its extensibility: it has a plugin framework with over [1800 published plugins](https://plugins.jenkins.io) which you can use to build any type of application, or integrate with any third-party system. There's a web UI you can use to define jobs, which get stored on the server's filesystem.

The UI and the plugins are one of the main reasons people don't like Jenkins. Plugins are prone to security flaws so they need frequent updates, but they can be fiddly to automate. You tend to find Jenkins servers are maintained long after they should have been decomissioned, because no-one's sure if they'd be able to recreate the exact set of plugins and load all the job definitions from the old server. We'll use Jenkins in a different way, with minimal plugins and job definitions stored in source control.

## Running Jenkins

Start by running Jenkins inside a Docker container, along with a local Git server (using Gogs):

```
docker-compose -f labs\jenkins\infra\docker-compose.yml up -d
```

> This is a custom setup of Jenkins with a few plugins already installed. It's built from [this Dockerfile](https://github.com/courselabs/docker-images/blob/main/src/jenkins/Dockerfile) if you want to see how it's automated.

Browse to Jenkins at http://localhost:8080. You may see a page saying "Jenkins is starting" - it can take a few minutes for a new server to come online. When you see the home page, click the log in link and sign in with these admin credentials:

- username: `courselabs`
- password: `student`

Check out the UI - it's slightly "web 1.0". The left nav takes you to the main options, including the menu to manage Jenkins; the central section will show a list of jobs once you have created some. 

## Creating a Freestyle Job

We'll create a classic Jenkins job - using the freestyle type where you build up the steps using the web UI:

- click _Create a job_ on the home screen
- call the new job `lab-1`
- set the job type to be _Freestyle project_
- click _OK_

This creates the new job. There are different sections of the UI which represent typical stages of a build - source code connection details, the build triggers and the build steps.

We'll use this job to run a simple script which prints some text:

- scroll down to the _Build_ section
- click _Add build step_
- select _Execute shell_
- paste this into the _Command_ box:

```
echo "This is build number: $BUILD_NUMBER of job: $JOB_NAME"
```

- click _Save_

> Jenkins populates [a set of environment variables](http://localhost:8080/env-vars.html/) when it runs the job, which are accessible in the job steps. This script prints out the build number - which is an incrementing count of the number of times the job has run - and the job name.

Now you're in the main job window. The left nav lets you configure the job again, and the central section will show the recent runs of the job.

Click _Build Now_ to run the job. When the build finishes check the output in http://localhost:8080/job/lab-1/1/console

Build again - how is the output different?

📋 Build the job again - how is the output different?

<details>
  <summary>Not sure?</summary>

Click _Build Now_ again. When the job completes you can see the output at http://localhost:8080/job/lab-1/2/console

The job name is the same, but the number has incremented.

</details><br/>

There are other tools installed in this Jenkins server, which you would use in a real pipeline. What happens if you print the version of the Java compiler, the Docker command line or the Kubernetes command line in the script?

📋 Edit the job to print the version numbers of `javac`, `docker` and `kubectl`.

<details>
  <summary>Not sure how?</summary>

Click _Configure_ in the job page to edit the job. 

Update the _Command_ box to print version numbers:

```
echo "This is build number: $BUILD_NUMBER of job: $JOB_NAME"

docker version

javac --version

kubectl version
```

Click _Save_.

</details><br/>

When you build the updated job, it will fail. Commands always return with an exit code, usually 0 means OK and non-zero means the command failed. The Kubernetes CLI errors because it can't connect to a Kubernetes environment. The exit code is non-zero so Jenkins takes that as a failure so the job step fails. 

If there were other steps after this one, they wouldn't run because the job exits when a step fails.

> This is the old-school way of using Jenkins. Where does the job definition live? How would you migrate it to a new Jenkins server? The better option is to use [pipelines](https://www.jenkins.io/doc/book/pipeline/), which need a plugin that this server already has.

## Plugins and the New UI

The pipeline feature comes with the `workflow-aggregator` plugin. There's also a nice UI to go with it. That comes in a different plugin which we'll install to get a feel for 
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
git remote add gogs http://localhost:3000/courselabs/devsecops.git

git push gogs main
```

> Log in

Check the repo at http://localhost:3000/courselabs/devsecops, you should see all the lab content

Back to Jenkins, create a new job: http://localhost:8080/view/all/newJob

- call it `manual-gate`
- select _Pipeline_


Under the _Pipeline_ section:

- select _Pipeline script from SCM_ in the dropdown
- SCM = _Git_
- Repository URL = `http://gogs:3000/courselabs/devsecops.git`
- Branch Specifier = `refs/heads/main`
- Script path = `labs/jenkins/manual-gate/Jenkinsfile`

The build is using this [Jenkinsfile](.\manual-gate\Jenkinsfile). It will pause after the _Test_stage and wait for user input.

Save and run the build. In the job UI is it clear how you can progress the _Deploy_stage?

: blue box _paused_; click and select _Do it!_


## Lab

create new pipeline to build a Java app
from the Jenkinsfile in labs\jenkins\hello-world\Jenkinsfile, polling SCM for changes every minute.

When the build runs it will fail. Can you see how to fix it? Where is the compiled app stored after a successful build?

