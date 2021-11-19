# Lab Hints

The `logs.json` file is already formatted correctly to use with the [bulk index API](https://www.elastic.co/guide/en/elasticsearch/reference/current/docs-bulk.html), so you can make a single POST request to the API to load them. 

Your match queries can be similar to the ones in the exercises. First you can query on one field, next you'll need a boolean query where one field needs to match and the other field shouldn't match.

Log searching needs some understanding of the data, so you may need to start by looking at all the logs.

> Need more? Here's the [solution](solution.md).