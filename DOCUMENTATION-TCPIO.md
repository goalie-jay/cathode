# Cathode Standard Library Documentation

## Namespace "fileio"

### Functions

#### nTCPSetReadTimeout(idx, timeout)

*	Arguments: idx (integer), timeout (integer)
*	Returns: void
*	Description: Sets the read timeout in milliseconds of the connection specified by idx

#### nTCPSetWriteTimeout(idx, timeout)

*	Arguments: idx (integer), timeout (integer)
*	Returns: void
*	Description: Sets the write timeout in milliseconds of the connection specified by idx

#### nTCPWrite(idx, arr)

*	Arguments: idx (integer), arr (array)
*	Returns: integer
*	Description: Tries to write the given byte array to the connection specified by idx and returns 1 if successful. Otherwise, 0 is returned

#### nTCPRead(idx)

*	Arguments: idx (integer), count (integer)
*	Returns: array, void
*	Description: Tries to read the given count of bytes from the connection specified by idx and returns them as a byte array if successful. Otherwise, void is returned

#### nTCPIsOpen(idx)

*	Arguments: idx (integer)
*	Returns: integer
*	Description: Checks if the connection specified by idx is open and returns 1 if it is. Otherwise, 0 is returned

#### nTCPCanRead(idx)

*	Arguments: idx (integer)
*	Returns: integer
*	Description: Checks if the connection specified by idx has data available to read and returns 1 if it does. Otherwise, 0 is returned
*	Additional notes: When writing your result checking, keep in mind that this function may be amended to return negative values for errors in the future

#### nTCPClose(idx)

*	Arguments: idx (integer)
*	Returns: void
*	Description: Closes the connection at the index supplied by idx

#### nTCPConnect(addr, port)

*	Arguments: addr (string), port (integer)
*	Returns: integer
*	Description: Attempts to connect to the remote host specified by addr:port and returns the corresponding connection index if successful. Otherwise, -1 is returned
*	Additional notes: Be wary when writing your error checking; this function *can* return zero from a successful call