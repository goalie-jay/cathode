# Cathode Standard Library Documentation

## Namespace "conio"

### Functions

#### cClear()

*	Arguments: none
*	Returns: void
*	Description: Clears the console window

#### cMode(w, h)

*	Arguments: w (integer), h (integer)
*	Returns: void
*	Description: Sets the console window size

#### cWidth()

*	Arguments: none
*	Returns: integer
*	Description: Gets the console window width

#### cHeight()

*	Arguments: none
*	Returns: integer
*	Description: Gets the console window height

#### Title(str)

*	Arguments: str (string)
*	Returns: void
*	Description: Sets the console window title to the given string

#### Print(obj)

*	Arguments: obj (anything)
*	Returns: void
*	Description: Prints the given object (with a string conversion if necessary) to the console

#### PrintLn(obj)

*	Arguments: obj (anything)
*	Returns: void
*	Description: Prints the given object (with a string conversion if necessary) to the console followed by a line terminator

#### ReadLn()

*	Arguments: none
*	Returns: string
*	Description: Reads a line from the console and returns it

#### cGetX()

*	Arguments: none
*	Returns: integer
*	Description: Returns the x position of the console cursor

#### cGetY()

*	Arguments: none
*	Returns: integer
*	Description: Returns the y position of the console cursor

#### cSetPos(x, y)

*	Arguments: x (integer), y (integer)
*	Returns: integer
*	Description: Attempts to the set the coordinates of the console cursor to the given x and y and returns 1 if successful. Otherwise, 0 is returned

#### cCharAt(x, y)

*	Arguments: x (integer), y (integer)
*	Returns: integer, void
*	Description: Attempts to get the character at the given coordinates in the console buffer and returns it if successful. Otherwise, void is returned