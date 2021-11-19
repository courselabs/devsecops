# Lab Solution

There's a sample solution in the lab folder: `lab/uptime-dashboard.json`. You can load that into Grafana by clicking the plus icon on the left menu and then _Import_.

### Infrastructure row

Add a new panel and select _Add a new row_. 

Hover your mouse over the row name and a gear icon will appear - click the icon to set the row name to _Infrastructure_.

Hover over your original status table name and an arrow icon will appear - click that and select _Edit_.

Add a label to the PromQL:

```
sum without(job, tier) (up{tier="infrastructure"})
```

Click _Apply_ - and do the same for the timeline panel (both can use the same query).

### Backend row

Add a new row for _Backend_.

Hover over the status table, click the arrow icon and select _More...Duplicate_ from the menu.

Edit the label selector in the new panel to use this PromQL:

```
sum without(job, tier) (up{tier="backend"})
```

Duplicate the timeline panel too, and set the PromQL to the same to filter for the backend tier.

### Web row

Repeat the steps from the backend row for the web tier.

### Save the dashboard

You can drag the panels into the correct rows and resize them. Click the disk icon from the top right menu to save the dashboard.

Click the share icon next to the dahboard name and click _Export_ to save the JSON to a file.
