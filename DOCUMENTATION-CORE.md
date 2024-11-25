# Cathode Standard Library Documentation

## Namespace "core"

*	The core namespace is included automatically in every context. It contains the essential functions of the language, along with the ability to import more namespaces

### Functions

#### PtrZero(lp)

*	Arguments: lp (longpointer)
*	Returns: integer
*	Description: Returns 1 if the pointer's value is zero. Otherwise, 0 is returned

#### PtrAddOffs(lp, offs)

*	Arguments: lp (longpointer), offs (integer)
*	Returns: longpointer
*	Description: Adds the given offset offs to the given longpointer lp and returns a new longpointer with the resulting value

#### RuntimeInfo()

*	Arguments: none
*	Returns: struct
*	Description: Returns a struct containing information about the interpreter and the operating system in the format { ExecutingFile (string, location of the currently executing program), ProcessId (integer), NameOfUser (string), NameOfMachine (string), X64 (integer), InterpreterInfo (struct) { MajorVersionNumber (integer), MinorVersionNumber (integer), IncrementVersionNumber (integer) } }

#### Arr(len)

*	Arguments: len (integer)
*	Returns: array
*	Description: Returns a new array of the given length populated with values of void

#### fAbs(value)

*	Arguments: value (float)
*	Returns: float
*	Description: Returns the absolute value of the given float

#### Abs(value)

*	Arguments: value (integer)
*	Returns: integer
*	Description: Returns the absolute value of the given integer
	
#### BytesToStr(bytes, encoding)

*	Arguments: bytes (array), encoding (string)
*	Returns: string, void
*	Description: Converts the given binary data to a string with the given encoding and returns the resulting string. Encoding value must be "ascii", "unicode", or "utf8". This function will return void if unsuccessful
	
#### StrToBytes(str, encoding)

*	Arguments: str (string), encoding (string)
*	Returns: array, void
*	Description: Converts the given string to a byte array with the given encoding and returns the resulting array. Encoding value must be "ascii", "unicode", or "utf8". This function will return void if unsuccessful
	
#### Arraylen(arr)

*	Arguments: arr (array)
*	Returns: integer
*	Description: Returns the length of the given array
	
#### Strlen(str)

*	Arguments: str (string)
*	Returns: integer
*	Description: Returns the length of the given string
	
#### Format(str, objs)

*	Arguments: str (string), objs (array)
*	Returns: string
*	Description: Fills a formatted string of the format "Item 0: $0, Item 1: $1, [etc...]" with the objects supplied and returns the result. If a variable number in the string does not correspond to an array element, it will be left unchanged

#### Strcat(arr, separator)

*	Arguments: arr (array), separator (string or void)
*	Returns: string
*	Description: Concatenates the strings in the array with each other and returns the result. The separator parameter can be used for its nominal purpose or will be ignored if void is passed
	
#### EnumFields(strct)

*	Arguments: strct (struct)
*	Returns: array
*	Description: Returns an array containing the names of the fields of the given struct
	
#### HasField(strct, name)

*	Arguments: strct (struct), name (string)
*	Returns: integer
*	Description: Checks if the supplied struct has a field of the given name. A nonzero value is returned if the field is present, and zero is returned if it is not
	
#### Field(strct, name)

*	Arguments: strct (struct), name (string)
*	Returns: anything
*	Description: Returns the value of a given struct's field of the given name
	
#### SetField(strct, name, obj)

*	Arguments: strct (struct), name (string), obj (anything)
*	Returns: void
*	Description: Sets the value of a given struct's field of the given name
	
#### Struct()

*	Arguments: None
*	Returns: struct
*	Description: Returns a new empty struct

#### CloneStruct(other)

*	Arguments: other (struct)
*	Returns: struct
*	Description: Returns a new struct identical to the given struct

#### CloneString(other)

*	Arguments: other (string)
*	Returns: string
*	Description: Returns a new string identical to the given string

#### CloneArray(other)

*	Arguments: other (array)
*	Returns: array
*	Description: Returns a new array identical to the given array
	
#### Assert(test, failureMsg)

*	Arguments: test (integer), failureMsg (string)
*	Returns: void
*	Description: Ends the program with an error message if the logical test test fails
	
#### Except(msg)

*	Arguments: msg (string)
*	Returns: void
*	Description: Ends the program with the supplied error message
	
#### Negate(value)

*	Arguments: value (integer)
*	Returns: integer
*	Description: Negates the given logical value and returns the result
	
#### Both(first, second)

*	Arguments: first (integer), second (integer)
*	Returns: integer
*	Description: Returns a nonzero value if both arguments are nonzero. Otherwise, zero is returned
	
#### Either(first, second)

*	Arguments: first (integer), second (integer)
*	Returns: integer
*	Description: Returns a nonzero value if either argument is nonzero. Otherwise, zero is returned
	
#### LessThan(first, second)

*	Arguments: first (integer, float, or byte), second (integer, float, or byte)
*	Returns: integer
*	Description: Returns a nonzero value if the first argument is less than the second. Otherwise, zero is returned
	
#### GreaterThan(first, second)

*	Arguments: first (integer, float, or byte), second (integer, float, or byte)
*	Returns: integer
*	Description: Returns a nonzero value if the first argument is greater than the second. Otherwise, zero is returned
	
#### Equal(first, second)

*	Arguments: first (anything), second (anything)
*	Returns: integer
*	Description: Returns a nonzero value if the arguments are equal. Otherwise, zero is returned
	
#### NotEqual(first, second)

*	Arguments: first (anything), second (anything)
*	Returns: integer
*	Description: Returns the negated result of Equal(first, second)
	
#### Exit(code)

*	Arguments: code (integer)
*	Returns: does not return
*	Description: Immediately exits the program with the given code
	
#### Byte(obj)

*	Arguments: obj (any)
*	Returns: byte, void
*	Description: Converts the supplied object to a byte and returns the result. If conversion is impossible, void is returned
*	Additional notes: Conversion is defined for the following types: integer, float, byte, filehandle, and alphanumeric string
	
#### ByteArrSmall(obj)

*	Arguments: obj (any)
*	Returns: array, void
*	Description: Converts the supplied object into its 32-bit binary representation. If conversion is impossible, void is returned
*	Additional notes: ByteArrSmall() is currently only supported for integers

#### ByteArr(obj)

*	Arguments: obj (any)
*	Returns: array, void
*	Description: Converts the supplied object into its 64-bit binary representation. If conversion is impossible, void is returned
*	Additional notes: ByteArr() is currently only supported for integers and floats
	
#### Integer(obj)

*	Arguments: obj (any)
*	Returns: integer, void
*	Description: Converts the supplied object to an integer and returns the result. If conversion is impossible, void is returned
*	Additional notes: Conversion is defined for the following types: integer, float, byte, filehandle, and alphanumeric string

#### String(obj)

*	Arguments: obj (any)
*	Returns: string
*	Description: Converts the supplied object a string and returns the result
	
#### Float(obj)

*	Arguments: obj (any)
*	Returns: float, void
*	Description: Converts the supplied object to a float and returns the result. If conversion is impossible, void is returned
*	Additional notes: Conversion is defined for the following types: integer, float, byte, and alphanumeric string
	
#### Strcmp(str1, str2)

*	Arguments: str1 (string), str2 (string)
*	Returns: integer
*	Description: Compared the two supplied strings and returns zero if they are equal. Otherwise, a nonzero value is returned
*	Additional notes: Be careful using this function in comparisons since it follows the opposite pattern of normal comparison. Remember to compare the result to zero

#### Uppercase(str)

*	Arguments: str (string)
*	Returns: string
*	Description: Returns a new string equivalent to the given string in uppercase

#### Lowercase(str)

*	Arguments: str (string)
*	Returns: string
*	Description: Returns a new string equivalent to the given string in lowercase