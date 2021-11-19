# Lab Hints

The document processor metrics are at http://localhost:9110/metrics.

Each metric has some documentation, prefixed with `#`:

```
# HELP process_num_threads Total number of threads
# TYPE process_num_threads gauge
process_num_threads 16
```

The `HELP` text gives you some basic information about the metric. You can search the `TYPE` lines to find the histogram.

> Need more? Here's the [solution](solution.md).