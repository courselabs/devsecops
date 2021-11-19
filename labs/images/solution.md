# Lab Solution

Here's a sample solution:

- [lab/Dockerfile](./lab/Dockerfile)

It just creates a working directory called `/app` in the container filesystem, and copies in the Java class file.

The class file and the Dockerfile are in different directories, so you need to use a context where Docker can access both files:

```
- labs
|- images   <- this is the context
 |-- java   <- so Docker can get the class file from here
 |-- lab    <- and the Dockerfile from here
```
Build the image using that context and specifying the path to the Dockerfile:

```
docker build -t java-hello-world -f labs/images/lab/Dockerfile labs/images
```

Run a container from the image:

```
docker run java-hello-world
```

> The output should say `Hello, World`
