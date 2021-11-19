# Lab Hints

You can load the log file in the same way as you did in the [Logstash exercises](../logstash/README.md) - the destination folder this time is the data directory in the `labs/kibana` folder.

Use the Kibana Console to query the API and find the name of the new index - that's what you'll use for the index pattern.

Then in the Discover tab, make use of the field list on the left to build your filters. This data doesn't include a numeric request ID, so you'll need to get creative with your search terms.

> Need more? Here's the [solution](solution.md).