# Cathode Standard Library Documentation

## Namespace "string"

*	While the string datatype and the Strlen() and Strcat() functions are available in namespace "core", the "string" namespace provides extended string manipulation functionality

### Functions

#### sIdxOf(str, substr)

*	Arguments: str (string), substr (string)
*	Returns: integer
*	Description: Finds the index of the first occurrence of the substring in the string and returns it. If the substring is not present in the string, -1 is returned

#### sSplit(str, delim)

*	Arguments: str (string), delim (string)
*	Returns: array
*	Description: Splits the given string by the given delimiter and then returns an array of the split pieces

#### sEmpty(str)

*	Arguments: str (string)
*	Returns: integer
*	Description: Checks if the given string is empty and returns 1 if it is. Otherwise, 0 is returned

#### sTrim(str)

*	Arguments: str (string)
*	Returns: string
*	Description: Removes whitespace from the beginning and end of the string