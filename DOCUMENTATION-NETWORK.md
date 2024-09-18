# Cathode Standard Library Documentation

## Namespace "network"

### Functions

#### ResolveHostname(hostname)

*	Arguments: hostname (string)
*	Returns: string, void
*	Description: Returns the IP associated with the given hostname if it can be resolved. Otherwise, void is returned

#### Ping(addr)

*	Arguments: addr (string)
*	Returns: integer
*	Description: Pings the given address and returns the length of the round trip in milliseconds if ping was possible. Otherwise, -1 is returned