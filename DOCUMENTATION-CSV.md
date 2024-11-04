# Cathode Standard Library Documentation

## Namespace "csv"

### Functions

#### ParseCsvFromFile()

*	Arguments: filename (string), sep (void, string)
*	Returns: struct, void
*	Description: Parses CSV data from the supplied file using the first character of the supplied string sep as the separator (or defaulting to "," if void is supplied) and returns it as a struct if successful. Otherwise, void is returned

#### ParseCsv()

*	Arguments: data (string), sep (void, string)
*	Returns: struct, void
*	Description: Parses the supplied CSV data using the first character of the supplied string sep as the separator (or defaulting to "," if void is supplied) and returns it as a struct if successful. Otherwise, void is returned

#### MkCsv()

*	Arguments: header (array)
*	Returns: struct
*	Description: Creates a new CSV struct with blank values and the given header labels and returns that struct