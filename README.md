**README**

# Cathode

A dynamically-typed, interpreted programming language

## ######
## HEY
## ######
* Cathode has been superseded. It is no longer in active development as I have created another language that better suits my needs. I will be making that language public shortly.

## Features

*   **Dynamic typing**: Variables can hold any type of data at any time
*	**Variable clarity**: While types are dynamic and struct fields can be assigned at will, freestanding variables must be dimensioned to be used, eliminating ambiguity over whether a variable is being created or simply assigned
*   **Unique syntax**: Blend elements from various influences for a fun (?) programming experience
*	**Imports on a functional basis**: Namespaces cannot be accessed by dot notation, and imports are done in each function at runtime. It is impossible to pollute the global namespace
*	**Return value posting**: Return values of functions can be set without returning, saving boilerplate

## Getting Started

### Prerequisites

*   Basically just download and compile the runtime, or download a release

### Installing the Language Runtime

To get started, download the latest release from the [releases page](https://github.com/rocky-horror/cathode/releases)

*	^ The language is incomplete and highly unstable, so I recommend compiling the interpreter from source so you have access to a debugger

## Documentation

Documentation is available [here on github](https://github.com/rocky-horror/cathode/blob/master/DOCUMENTATION.md)

## Code Examples

### Preferred Code Style

*	Keywords and operators are lowercase
*	Standard library functions are PascalCase, with an optional lowercase single-character prefix to indicate the specialty of the function
*	Variables are recommended to be PascalCase or camelCase
*	Spaces on the inside (but not the outside) of parentheses are encouraged but not required

```
namespace OurNamespace ; this is for determining the namespace for files designed to be imported. It is ignored in the main file

import( "conio" ) ; import is used like a function, but it is viewed as a keyword and is lowercase
dim ourStruct = Struct( ) ; dim is a keyword for dimensioning variables. It can be used with or without assignment; in this case ourStruct is being assigned a blank struct

PrintLn( typeof( ourStruct ) ) ; outputs "struct". typeof is also a keyword used like a function

```

### Main Function Format

```
fndef Main accepts args
	
fnret
```

### Basic Arithmetic

```
; Import console I/O namespace
import( "conio" )

; Add two numbers together
dim x = 5 + 3
PrintLn( x )
```

### Function Definition

```
fndef PrintNameAndAge accepts name, age
	import( "conio" )
	PrintLn( Format( "$0 is $1 years old!", { name, age } ) )
fnret

PrintNameAndAge( "John", 27 )
```

### Struct Use 

```
fndef CreateStruct
	Dim ourStruct = Struct( )
	
	; Struct fields are created upon their use if they do not already exist
	ourStruct.Numbers = { 13, 29, 42, 86 }
fnret

Dim result = CreateStruct( )
import( "conio" )
PrintLn( result.Numbers )
```

### Reading a File Line-By-Line

```
fndef Main
	post 0 ; This sets a default return value of 0

	import( "conio" ) ; Import the PrintLn( ), Print( ), and ReadLn( ) functions
	import( "fileio" ) ; Import the fOpen( ), fGetPos( ), fLen( ), and fReadLn( ) functions

	Print( "Enter a filename to read it line-by-line: " )
	dim filename = ReadLn( ) ; Accept filename from stdin
	
	dim handle = fOpen( filename, "r" ) ; Open the file with read perms
	
	if ( handle == void ) ; Ensure the handle is valid
		PrintLn( "Failed to open file." )
		ret 1 ; We have failed, so return an error code
	then
	
	PrintLn( "Press enter after each line to get the next one." )
	PrintLn( fReadLn( handle ) ) ; Print first line
	while ( fGetPos( handle ) < fLen( handle ) )
		Title( Format( "($0%)", { Integer( ( Float( fGetPos( handle ) ) / Float( fLen( handle ) ) ) * Float( 100 ) ) } ) ) ; Set title to percentage read. It's converted back to integer in order to not be an ugly decimal
		ReadLn( )
		Print( fReadLn( handle ) )
	loop
	PrintLn( "" )
	
	fClose( handle ) ; Close our handle
	Title( "(100%)" )
	Print( "Done (press enter to quit)." )
	ReadLn( )
fnret
```

## Bugs

If you find something that doesn't work, *please* let me know, either through a Discord message or through a GitHub issue, and I'll get it fixed as soon as I possibly can
