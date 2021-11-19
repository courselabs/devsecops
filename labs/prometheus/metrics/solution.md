# Lab Solution

The histogram metric is `fulfilment_processing_seconds`.

It records (fake) data about document processing times:

```
# HELP fulfilment_processing_seconds Fulfilment processing duration
# TYPE fulfilment_processing_seconds histogram
fulfilment_processing_seconds_sum 202315
fulfilment_processing_seconds_count 31131
fulfilment_processing_seconds_bucket{le="0"} 0
fulfilment_processing_seconds_bucket{le="2"} 3049
fulfilment_processing_seconds_bucket{le="4"} 9270
fulfilment_processing_seconds_bucket{le="6"} 15623
fulfilment_processing_seconds_bucket{le="8"} 21837
fulfilment_processing_seconds_bucket{le="10"} 28067
fulfilment_processing_seconds_bucket{le="12"} 31131
fulfilment_processing_seconds_bucket{le="+Inf"} 31131
```

Histograms record several metrics, grouped into buckets. In this case each bucket records the number of documents processed in different timespans, e.g:

| Within _x_ seconds | Document count |
|-|-|
| 2 | 3049|
| 6 | 15623|

Buckets are cumulative, so they can be used for calculations, e.g:

```
fulfilment_processing_seconds_bucket{le="4"} 9270
fulfilment_processing_seconds_bucket{le="6"} 15623
```

=> number of documents which took between 4 and 6 seconds to process = 15623-9270 = 6353

There's enough information in a histogram to calculate percentiles, e.g. 95% of requests were processed within _y_ seconds.