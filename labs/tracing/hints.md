# Lab Hints

In curl you can add a request header with the format:

```
curl -H "<header-name>: <header-value>" <url>
```

There are different standards for propagating trace headers, but the most common format - which these apps use - is the [W3C `traceparent`](https://www.w3.org/TR/trace-context-1/#traceparent-header).

Set a valid ID in that header and you should see it used in both sets of container logs.

> Need more? Here's the [solution](solution.md).