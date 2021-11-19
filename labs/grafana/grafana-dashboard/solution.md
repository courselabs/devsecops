# Lab Solution

This is quite fiddly, but worth doing because you'll use these a lot.

## Configure the heatmap

Start with a new panel using the query:

```
avg by(le) (rate(fulfilment_processing_seconds_bucket[5m]))
```

Set the format in the query panel to be `Heatmap`:

![](../../img/grafana-dashboard-lab-1.png)

Then in the display options set the visualization type to `Heatmap` and under the _Axes_ section, set the _Data format_ to `Time series buckets`:

![](../../img/grafana-dashboard-lab-2.png)

## Sample solution

You can load the completed dashboard with the heatmap from `lab/dashboard.json`.
