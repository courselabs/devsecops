# Lab Solution

Range queries need to include `start`, `end`  and `step` parameters in the URL query string.

This example will work, but won't return any data - because you won't have any metrics stored for these dates:

```
curl 'localhost:9090/api/v1/query_range?query=fulfilment_in_flight_total&start=2021-07-11T00:01:30Z&end=2021-07-11T01:00:00Z&step=15s'
```

You'll see:

```
{"status":"success","data":{"resultType":"matrix","result":[]}}
```

You can use an [RFC 3339](https://datatracker.ietf.org/doc/html/rfc3339) date or a Unix timestamp as the start and end values.

In PowerShell you can store the current date and one hour ago in variables like this:

```
$now=Get-Date
$end=(Get-Date -Date $now -UFormat %s)
$start=(Get-Date -Date $now.AddHours(-1) -UFormat %s)
```

OR in Linux/macOS:

```
now=$(date)
end=$(date -d $now +%s)
start="$(date -d "$now - 1 hours" +%s)"
```

Then query the API using the variables in the URL - this works for PowerShell and Bash:

```
curl "localhost:9090/api/v1/query_range?query=fulfilment_in_flight_total&start=$start&end=$end&step=30s"
```