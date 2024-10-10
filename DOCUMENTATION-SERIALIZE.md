# Cathode Standard Library Documentation

## Namespace "serialize"

### Functions

#### CanSerialize(obj)

*	Arguments: obj (anything)
*	Returns: integer
*	Description: Checks if an object can be serialized and returns 1 if it can. Otherwise, 0 is returned

#### SerializeBin(obj)

*	Arguments: obj (anything)
*	Returns: array
*	Description: Serializes the given object into a byte array and returns that array

#### DeserializeBin(arr)

*	Arguments: arr (array)
*	Returns: anything
*	Description: Deserializes the given byte array into its source object