# Cathode Standard Library Documentation

## Namespace "array"

*	While the array datatype and the ArrayLen() function are available in namespace "core", the "array" namespace provides extended array manipulation functionality

### Functions

#### aIdxOf(arr, obj)

*	Arguments: arr (array), obj (object)
*	Returns: integer
*	Description: Finds the index of the first occurrence of the object in the array and returns it. If the value is not present in the array, -1 is returned

#### aCount(arr, obj)

*	Arguments: arr (array), obj (object)
*	Returns: integer
*	Description: Returns the amount of times the object occurs in the array

#### aRemoveAll(arr, obj)

*	Arguments: arr (array), obj (object)
*	Returns: array
*	Description: Returns a new array with the contents of the supplied array where all instances of the object were removed

#### aRemoveIdx(arr, idx)

*	Arguments: arr (array), idx (integer)
*	Returns: array
*	Description: Returns a new array with the contents of the supplied array where the element at the given index has been removed

#### aAppend(arr, obj)

*	Arguments: arr (array), obj (object)
*	Returns: array
*	Description: Returns a new array with the contents of the supplied array where the supplied object has been added to the end of the array

#### aSection(arr, start, length)

*	Arguments: arr (array), start (integer), length (integer)
*	Returns: array
*	Description: Returns a new array with the contents of the supplied array starting at the given start index and continuing for the given length

#### aConcat(arr1, arr2)

*	Arguments: arr1 (array), arr2 (array)
*	Returns: array
*	Description: Returns a new array with the contents of the first supplied array followed by the contents of the second supplied array