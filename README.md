# ContinuousPing

This is a simple windows service I wrote in ~2014 that parses a list of hosts and dumps out CSV containing their pings.
I usually do traceroute to get the list of hosts that I want to test against, and then put each of those hosts into the `Hosts.txt` file in the root directory of the Windows Service.

Hosts.txt:
````
127.0.0.1
192.168.0.1
192.168.100.1
cpe-74-65-16-1.rochester.res.rr.com
tge0-0-3.hnrtnyaf01h.northeast.rr.com
be46.hnrtnyaf02r.northeast.rr.com
be28.albynyyf01r.northeast.rr.com
google.com
yahoo.com
````

If you leave it running for a while, remember to go back and clear the old logs out of the 'traces' directory.

If you have a network outage while this is running, it will tell you which hop starts failing if you load the CSV in excel (or your favorite visualization tool) and do a quick line chart.  Non-responses are given a value of 1001ms.
