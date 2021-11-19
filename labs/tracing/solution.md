# Lab Solution

The `traceparent` header has quite a complex format, and if you use an invalid ID then it won't get propagated.

There are some [example headers in the W3C spec](https://www.w3.org/TR/trace-context-1/#examples-of-http-traceparent-headers) which have valid IDs, so you can use one of those:

```
curl -H "traceparent: 00-0af7651916cd43dd8448eb211c80319c-b7ad6b7169203331-01" localhost:8070/index?all
```

> You'll see an HTML response containing the document IDs - be sure to use the correct URL so the web app calls into the document API.

Check the logs for the web app:

```
docker logs obsfun_fulfilment-web_1 -n 22
```

You'll see a final log like this, which is the server span for the curl call:

```
Activity.Id:          00-0af7651916cd43dd8448eb211c80319c-6f70aaf42a3e1842-01
Activity.ParentId:    00-0af7651916cd43dd8448eb211c80319c-b7ad6b7169203331-01
Activity.ActivitySourceName: OpenTelemetry.Instrumentation.AspNetCore
Activity.DisplayName: Index
Activity.Kind:        Server
Activity.StartTime:   2021-07-19T16:02:30.7422710Z
Activity.Duration:    00:00:00.0193704
Activity.TagObjects:
    http.host: localhost:8070
    http.method: GET
    http.path: /index
    http.url: http://localhost:8070/index?all
    http.user_agent: curl/7.55.1
    http.route: Index
    http.status_code: 200
```

> The trace ID is shown in the `Activity.ParentId` field. This is the first call in the chain and because we set a trace ID, the web app uses that as the parent.

The span ID starts with `00-0af7651916cd43dd8448eb211c80319c`, which is the trace ID.

Check the API logs:

```
docker logs obsfun_fulfilment-api_1 -n 1
```

You'll see a final log entry like this:

```
2021-07-19 16:02:30.754  INFO 1 --- [p-nio-80-exec-7] i.j.internal.reporters.LoggingReporter   : Span reported: af7651916cd43dd8448eb211c80319c:9ee709025ddee655:2f0d964815ca6846:1 - get
```

The ID of the span begins with the trace ID we set in the curl header (minus the initial `00-` which is the trace format version number).

