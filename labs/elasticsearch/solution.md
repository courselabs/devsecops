# Lab Solution

## Bulk load the logs

Make a POST request to the index's `_bulk` endpoint:

```
curl -H 'Content-Type: application/json' -XPOST 'localhost:9200/logs/_bulk' --data-binary '@data/logs.json'
```

## Count of successful requests

Successful requests have a log message containing the phrase `Fulfilment completed`.

You can match just on the word `completed` as it isn't used in any other log entries:

- [match-completed.json](\lab\queries\match-completed.json) - shows the query
- add `size=0` to the request to see the query results without the actual documents

```
curl -H 'Content-Type: application/json' 'localhost:9200/logs/_search?size=0&pretty' --data-binary '@labs/elasticsearch/lab/queries/match-completed.json'
```

> There are 30 matches (in the field `hits.total.value`)

## Request ID for the one error which was not a document service problem

Logs with the error level mostly have a message containing the phrase `document service unavailable`:

- [match-error.json](\lab\queries\match-error.json) - uses a boolean query where the level is error and the message must not contain the document service phrase

```
curl -H 'Content-Type: application/json' http://localhost:9200/logs/_search?pretty --data-binary '@labs/elasticsearch/lab/queries/match-error.json'
```

> The request ID is 32441751.


___
> Back to the [exercises](README.md).