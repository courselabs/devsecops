# Application Traces

Tracing is the third pillar of observability - metrics show you overall health, logs let you dig into the detail and traces help you understand the communication between components.

For that you need your application components to emit and receive trace data, so a collector can store all the traces and identify the calls which belong to the same client transaction.

## Reference

- [OpenTelemetry specification](https://github.com/open-telemetry/opentelemetry-specification/blob/main/specification/overview.md) - the emerging-but-looking-likely-to-become-standard tracing spec

- [Trace Context](https://www.w3.org/TR/trace-context/) - the W3C standard for propagating trace details in HTTP headers

## Exporting traces

We have a new demo app - a very simple web UI which is part of the (fake) document processing solution:

- [web.yml](./web.yml) - specifies the web application, connected to a Docker network and publishing port 8070

Start by running the app and checking the logs:

```
docker-compose -f labs/tracing/web.yml up -d

docker logs -f obsfun_fulfilment-web_1
```

> This command follows the logs, you can leave it running while we use the app

Browse to the website at http://localhost:8070. It's not much to look at, but it's built with [OpenTelemetry instrumentation](https://opentelemetry.io/docs/concepts/instrumenting/) support, which emits all the tracing details we need.

You'll see an entry like this in the logs:

```
Activity.Id:          00-1cdb0fc1ee7a91448072c6d391106dd9-ab272d7266d63e46-01
Activity.ActivitySourceName: OpenTelemetry.Instrumentation.AspNetCore
Activity.DisplayName: /
Activity.Kind:        Server
Activity.StartTime:   2021-07-16T08:31:17.1688367Z
Activity.Duration:    00:00:00.2235717
Activity.TagObjects:
    http.host: localhost:8070
    http.method: GET
    http.path: /
    http.url: http://localhost:8070/
    http.user_agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:90.0) Gecko/20100101 Firefox/90.0
    http.status_code: 200
    otel.status_code: UNSET
Resource associated with Activity:
    service.name: Fulfilment.Web
    service.instance.id: 4b8a603f-34d3-4611-b01c-9e9c35932170
```

> This application uses the .NET client library. That uses the OpenTelemetry standards but has its own naming convention - it talks about an _activity_ where OpenTelemetry uses the term _span_.

📋 What does the activity represent, and what detail is recorded about it?

<details>
  <summary>Need some help?</summary>

This activity represents a call to the web server.

In each case there's ID to idenfity the span (activity), together with the kind of work, start time and duration.

Tags are used to store key details about the span - these are server calls so they record the HTTP method, URL path, response code etc.

</details><br/>

There's enough information in the span to understand what call was made on the server, when it was initiated, how long it took to run and what the response status was. 

With all your app components emitting logs like this, you can build up a distributed trace showing all the network calls in a transaction.

Hit the _Go_ button on the web page and after a few seconds you'll see an error message `Documents service unavailable!`.

📋 Check the trace logs to see what the problem is.

<details>
  <summary>Need some help?</summary>

Scroll up past the error messages in the logs and you'll see a trace like this:

```
Activity.DisplayName: HTTP GET
Activity.Kind:        Client
Activity.StartTime:   2021-07-19T15:13:36.1289362Z
Activity.Duration:    00:00:10.0518450
Activity.TagObjects:
    http.method: GET
    http.host: fulfilment-api
    http.url: http://fulfilment-api/documents
    otel.status_code: ERROR
    otel.status_description: Resource temporarily unavailable
```

This is a client activity, which means the span records details of the web component making an HTTP call to another component. The tags tell you the web app is trying to call `http://fulfilment-api/documents`, but the response is an error.

</details><br/>

Even with a single component, trace logs are useful. We can go straight from the error a user sees to knowing that there's a problem with the website - it can't access the fulfilment API server.

The situation is more powerful when you have multiple components emitting traces, because the logs help you link together all the network calls. 

## Context propagation

HTTP calls include headers in the client request and the server response. _Context propagation_ is the process of transmitting trace details in HTTP calls by setting values in the headers.

The client includes details like the trace ID in the HTTP request headers. If the server makes any HTTP calls of its own then it propagates the trace information by copying the HTTP headers it received into all outgoing requests.

Start the document fulfilment API server so we can see tracing between components:

- [api.yml](./api.yml) - specifies the API container with a flag to enable tracing, connecting to the same network as the web container and publishing port 8071.

Run the API container and check its logs:

```
docker-compose -f labs/tracing/api.yml up -d

docker logs -f obsfun_fulfilment-api_1
```

> This is a Java Spring Boot application. You'll see a lot of startup logs.

Browse to the application root at http://localhost:8071/documents - this is the document list the website uses.

📋 What span information do you see in these logs?

<details>
  <summary>Need some help?</summary>

This application doesn't print all the trace details in logs, but there is some interesting information there. 

The last few logs will have lines like this:

```
2021-07-19 15:27:52.048 DEBUG 1 --- [p-nio-80-exec-3] fulfilment.api.DocumentsController       : ** GET /documents called in trace id: ab5e7e2261cfc919, with baggage: null
...
2021-07-19 15:27:52.095  INFO 1 --- [p-nio-80-exec-3] i.j.internal.reporters.LoggingReporter   : Span reported: ab5e7e2261cfc919:ab5e7e2261cfc919:0:1 - get
```

The first log prints a trace ID, and the last log states that a span has been reported. 

</details><br/>

The ID of the span starts with the ID of the trace, which is how this HTTP call can be linked to others which are part of the same initial client request.

Browse back to the web app at http://localhost:8070/ and hit _Go_.

Now the web app can reach the API and you'll see a new set of logs for the document API, with a new span ID recorded. The final log will be something like this:

```
2021-07-19 15:38:15.431  INFO 1 --- [p-nio-80-exec-5] i.j.internal.reporters.LoggingReporter   : Span reported: 17d93024304cd248919240491e832317:f9689b5465ee1877:a4e1100c185c5340:1 - get
```

The span ID still starts with a trace ID, but the format of that ID is different from when you called the `/documents` URL directly from your browser

📋 Where did the new trace ID come from?

<details>
  <summary>Need some help?</summary>

This time the API wasn't the first client call - the website is the first call in the chain, and the API is next.

Check the trace logs in the web container to find the trace ID from your API log:

```
# Ctrl-C to exit the logs of the API

docker logs obsfun_fulfilment-web_1
```

Scroll up to the client trace where the web application calls the API, and you'll see an entry like this:

```
Activity.Id:          00-17d93024304cd248919240491e832317-a4e1100c185c5340-01
Activity.ParentId:    00-17d93024304cd248919240491e832317-a4ff2ebe2e663c4c-01
Activity.ActivitySourceName: OpenTelemetry.Instrumentation.Http
Activity.DisplayName: HTTP GET
Activity.Kind:        Client
Activity.StartTime:   2021-07-19T15:38:15.4166205Z
Activity.Duration:    00:00:00.0174145
Activity.TagObjects:
    http.method: GET
    http.host: fulfilment-api
    http.url: http://fulfilment-api/documents
    http.status_code: 200
```

This time the API call has succeeded, and the trace ID component of the activity ID matches the one in the API logs, `17d93024304cd248919240491e832317` in this case.

</details><br/>

If you look at the final log entry for the web application you'll see that it has the same trace ID in its activity (span) ID. Spans are only logged when they've completed, so you see them in reverse order. The actual call chain can be put together from the spans:

1. user makes a POST request to the website; this is a server span with no parent ID because it's the first in the chain, so it generates a trace ID

2. the website makes a client call to the API; this span uses the ID of span #1 as its parent ID, and preserves the trace ID in its own span ID

3. the API logs a span. There's not much detail in the logs, but this is also a server span, it will use the ID from span #2 as its parent, and preserve the trace ID from #1 in its own span ID.

At the simplest level, distributed tracing is about your components recording details of all the HTTP calls they make and receive - the _spans_. Components use an ID in the HTTP header to identify when calls belong to the same transaction - the _trace_.

## Lab

The web application supports OpenTelemetry, so if it gets called by another component using OpenTelemetry then it should propagate the trace ID it receives from that call, instead of generating its own.

You can use curl to call the web app outside of the browser.

_If you're a Windows user run this script to use the correct curl command:_

```
# first enable scripts:
Set-ExecutionPolicy -ExecutionPolicy Unrestricted -Scope Process

# then run:
. ./scripts/windows-tools.ps1
```

Then call the site:

```
curl localhost:8070/index?all
```

Add a trace ID to the curl call and verify that your ID gets used in the spans for the web app and API.

> Stuck? Try [hints](hints.md) or check the [solution](solution.md).

___
## Cleanup

Cleanup by removing all containers:

```
docker rm -f $(docker ps -aq)
```