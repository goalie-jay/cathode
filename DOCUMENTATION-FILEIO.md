# Cathode Standard Library Documentation

## Namespace "fileio"

### Functions

#### Getcwd()

*	Arguments: none
*	Returns: string
*	Description: Returns the current working directory

#### Chdir(name)

*	Arguments: name (string)
*	Returns: integer
*	Description: Tries to change the current working directory to the supplied directory and returns a nonzero value if successful. Otherwise, zero is returned

#### dEnumDirectories(name)

*	Arguments: name (string)
*	Returns: array, void
*	Description: Tries to enumerate the directories inside the given directory and returns them as an array if successful. Otherwise, void is returned

#### dEnumFiles(name)

*	Arguments: name (string)
*	Returns: array, void
*	Description: Tries to enumerate the files inside the given directory and returns them as an array if successful. Otherwise, void is returned

#### dExists(dirName)

*	Arguments: dirName (string)
*	Returns: integer
*	Description: Checks if a given directory exists and returns a nonzero value if it does. Otherwise, zero is returned

#### dCreate(dirName)

*	Arguments: dirName (string)
*	Returns: integer
*	Description: Creates a directory with the given name and returns a nonzero value if successful. Otherwise, zero is returned

#### dUnlink(dirName)

*	Arguments: dirName (string)
*	Returns: integer
*	Description: Deletes a directory with the given name and returns a nonzero value if successful. Otherwise, zero is returned

#### fUnlink(name)

*	Arguments: name (string)
*	Returns: integer
*	Description: Deletes a file with the given name and returns a nonzero value if successful. Otherwise, zero is returned

#### fOpen(name, perms)

*	Arguments: name (string), perms (string)
*	Returns: filehandle, void
*	Description: Tries to open a file with the given name and perms ("r", "rw", or "w") and returns a handle if successful. Otherwise, void is returned

#### fExists(name)

*	Arguments: name (string)
*	Returns: integer
*	Description: Checks if a file with the given name exists and returns a nonzero value if successful. Otherwise, zero is returned

#### fCreate(name)

*	Arguments: name (string)
*	Returns: filehandle, void
*	Description: Tries to create a file with the given name and returns a handle if successful. Otherwise, void is returned

#### fGetPath(name)

*	Arguments: name (string)
*	Returns: string, void
*	Description: Tries to get the full path of a file with the given name and returns that path if successful. Otherwise, void is returned

#### fClose(handle)

*	Arguments: handle (filehandle)
*	Returns: void
*	Description: Closes the given handle

#### fReadLine(handle)

*	Arguments: handle (filehandle)
*	Returns: void
*	Description: Tries to read a line from the file pointed to by the given handle and returns that line if successful. Otherwise, void is returned

#### fLen(handle)

*	Arguments: handle (filehandle)
*	Returns: integer, void
*	Description: Tries to get the length of the file pointed to by the given handle and returns the length if successful. Otherwise, void is returned

#### fGetPos(handle)

*	Arguments: handle (filehandle)
*	Returns: integer
*	Description: Returns the current position in the file pointed to by the given handle

#### fSetPos(handle, position)

*	Arguments: handle (filehandle), position (integer)
*	Returns: void
*	Description: Sets the current position in the file pointed to by the given handle to the supplied position

#### fRead(handle, count)

*	Arguments: handle (filehandle), count (integer)
*	Returns: array, void
*	Description: Tries to read the given count of bytes from the file pointed to by the given handle and returns them as an array. If there are not enough bytes remaining, it will return as many as it can read. If the function fails, void is returned

#### fWrite(handle, arr)

*	Arguments: handle (filehande), arr (array)
*	Returns: integer
*	Description: Tries to write the given array of bytes to the file pointed to by the given handle and returns a nonzero value if successful. Otherwise, zero is returned

#### fWriteLine(handle, line, encoding)

*	Arguments: handle (filehandle), line (string), encoding (string)
*	Returns: integer
*	Description: Tries to write the given line of text with the given encoding ("ascii", "unicode", or "utf8") to the file pointed to by the given handle and returns a nonzero value if successful. Otherwise, zero is returned