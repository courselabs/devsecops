# Exporting Metrics

In a distributed architecture, each of your components exposes an HTTP endpoint, publishing all the metrics relevant to that component. The metrics format is standard, and you'll want to capture metrics at different levels of detail.

## Reference

- [Prometheus exposition format](https://prometheus.io/docs/instrumenting/exposition_formats/) - the most common metrics format
- [Metric types](https://prometheus.io/docs/concepts/metric_types/) - the four metric types in Prometheus
- [Client libraries](https://prometheus.io/docs/instrumenting/clientlibs/) - adding instrumentation to apps
- [Exporters](https://prometheus.io/docs/instrumenting/exporters/) - instrumentation for third-party systems 

## Application metrics

Start by running a simple server application. It's a mock document processor, and it records metrics about the work it's doing:

```
docker run -d -p 9110:9110 --name processor courselabs/fulfilment-processor
```

> Browse to the metrics endpoint at http://localhost:9110/metrics

You'll see a whole lot of text, including lines like this:

```
# HELP fulfilment_requests_total Fulfilment requests received
# TYPE fulfilment_requests_total counter
fulfilment_requests_total{status="failed"} 0
fulfilment_requests_total{status="processed"} 142

# HELP fulfilment_in_flight_total Fulfilment requests in progress
# TYPE fulfilment_in_flight_total gauge
fulfilment_in_flight_total 142
```

This data records the number of documents the processor is currently working on, and the number it has processed.

📋 Make a note of the numbers and refresh the page a few times. What's the difference between _gauge_ and _counter_ metrics?

<details>
  <summary>Need some help?</summary>

Counter values can increase or stay the same, they don't decrease. They're typically used to count things that have been done, which doesn't go down.

After some refreshes my processed counter increased from 142 to 4403:

```
# HELP fulfilment_requests_total Fulfilment requests received
# TYPE fulfilment_requests_total counter
fulfilment_requests_total{status="failed"} 180
fulfilment_requests_total{status="processed"} 4403
```

Gauges can go up or down (or stay the same). They're used to record a snapshot of the number of things happening right now.

After refreshing my in-flight counter went from 142 to 46:

```
# HELP fulfilment_in_flight_total Fulfilment requests in progress
# TYPE fulfilment_in_flight_total gauge
fulfilment_in_flight_total 46
```

</details><br/>

Prometheus uses labels to record different aspects of the same type of data, so the `fulfilment_requests_total` metric includes counters for the number of documents processed and the number which have failed.

📋 Why doesn't the app also record a count of sucessfully processed documents?

<details>
  <summary>Need some help?</summary>

More labels means more data to record, so you shouldn't produce metrics which can be calculated reliably from other metrics.

You can always determine the succesful count by subtracting the number of failures from the total:

```
fulfilment_requests_total{status="failed"} 180
fulfilment_requests_total{status="processed"} 4403
```

=> succesfully processed = 4403-180 = 4223

</details><br/>

Right now this app just shows the custom metrics. Prometheus client libraries can also produce common metrics about the application platform.

Remove the processor container and start a new one, configured to expose platform runtime metrics (this is a .NET app using the [prometheus-net client library](https://github.com/prometheus-net/prometheus-net)):

```
docker rm -f processor

docker run -d -p 9110:9110 --name processor -e Observability__Metrics__IncludeRuntime=true courselabs/fulfilment-processor
```

> Browse to the metrics endpoint again at http://localhost:9110/metrics

The application metrics are there, together with standard metrics like the number of threads in use and the amount of processor time used:

```
# HELP process_num_threads Total number of threads
# TYPE process_num_threads gauge
process_num_threads 16

# HELP process_cpu_seconds_total Total user and system CPU time spent in seconds.
# TYPE process_cpu_seconds_total counter
process_cpu_seconds_total 0.47
```

📋 What has happened to the document processed counter?

<details>
  <summary>Need some help?</summary>

It's been reset - this is a new instance of the app with a new set of metrics.

Prometheus understands about resets, so if you query this metric it would add the current counter values to the data from the previous instance of the app.

</details><br/>

Application metrics tell you what your app is doing and how hard it is working. You also want metrics at infrastructure level to show what your servers are doing.

## Server metrics

The [node exporter](https://github.com/prometheus/node_exporter) publishes standard server details. You would normally run it as a background service on Linux machines (there's also a [Windows exporter](https://github.com/prometheus-community/windows_exporter) for Windows machines).

We can see the metrics it publishes by running it in a container. 

```
docker run -d -p 9100:9100 --name node courselabs/node-exporter
```

> Browse these metrics at http://localhost:9100/metrics

You'll see a lot more metrics, recording system information and details about the application platform of the node exporter itself.

📋 What language is the exporter written in? Are any metrics the same as the document processor?

<details>
  <summary>Need some help?</summary>

The exporter is written in Go - you'll see lots of metrics about the Go runtime:

```
# HELP go_goroutines Number of goroutines that currently exist.
# TYPE go_goroutines gauge
go_goroutines 8

# HELP go_info Information about the Go environment.
# TYPE go_info gauge
go_info{version="go1.15.8"} 1
```

Library authors are encouraged to publish standard metrics where possible. There isn't much instrumentation in common with a Linux server and the document processor app, but you'll see `process_cpu_seconds_total` and `process_start_time_seconds` in both.

</details><br/>

Having the same type of metric published in different systems lets you compare between them. There's one more level of detail you'll want to see, which fits in between the server and your custom app metrics - instrumentation from the runtime which is hosting your app.

## Runtime metrics

Application servers like Tomcat and ASP.NET should publish usage metrics, like the number of HTTP requests processed. These are usually recorded and published as standard in the client library.

Run a Java REST API which uses Tomcat and the [micrometer client library](https://micrometer.io):

```
docker run -d -p 8080:80 courselabs/fulfilment-api
```

> Browse to the metrics at http://localhost:8080/actuator/prometheus

These metrics include a lot of detail about the JVM (the Java runtime), and the performance of Tomcat (the HTTP server). There's no code in the app to record these, they're all part of [Spring Boot's metrics collection](https://docs.spring.io/spring-boot/docs/current/reference/html/actuator.html#actuator.metrics.supported).

Try browsing to some application endpoints:

- http://localhost:8080/documents - shows a mock list of documents being processed
- http://localhost:8080/notfound - returns a 404 not-found page

📋 Refresh the metrics. What can you learn about the HTTP requests the app has served?

<details>
  <summary>Need some help?</summary>

There's a metric called `http_server_requests_seconds` which records the count of requests and the time taken to process them, split by the requested URL and the response code:

```
# HELP http_server_requests_seconds  
# TYPE http_server_requests_seconds summary
http_server_requests_seconds_count{exception="None",method="GET",outcome="SUCCESS",status="200",uri="/actuator/prometheus",} 18.0
http_server_requests_seconds_sum{exception="None",method="GET",outcome="SUCCESS",status="200",uri="/actuator/prometheus",} 0.1702426
http_server_requests_seconds_count{exception="None",method="GET",outcome="SUCCESS",status="200",uri="/documents",} 2.0
http_server_requests_seconds_sum{exception="None",method="GET",outcome="SUCCESS",status="200",uri="/documents",} 0.0521596
http_server_requests_seconds_count{exception="None",method="GET",outcome="CLIENT_ERROR",status="404",uri="/**",} 3.0
http_server_requests_seconds_sum{exception="None",method="GET",outcome="CLIENT_ERROR",status="404",uri="/**",} 0.258512
```

This is a summary gauge, you can use it to show average processing time for requests, e.g for the /documents endpoint:

```
http_server_requests_seconds_count=3.0
http_server_requests_seconds_sum=0.258512
```

=> average processing time = 0.258512 / 3.0 = 0.086 seconds per request

</details><br/>

Summaries record two pieces of data for each metric - how often it has happened, and the total amount of time it took for all occurences. That lets you calculate averages without storing too much data. 

## Lab

We've looked at counters, gauges and summaries. There's one other metric type - the histogram.

Look at the metrics from the document processor at http://localhost:9110/metrics again and you'll see a histogram metric. What is it recording, and how could you use the data?

> Stuck? Try [hints](hints.md) or check the [solution](solution.md).

___
## Cleanup

Cleanup by removing all containers:

```
docker rm -f $(docker ps -aq)
```