**README**

# Cathode

A dynamically-typed, interpreted programming language that believes in using functions for most operations

## Features

*   **Dynamic typing**: No need to declare variable types beforehand
*	**Variable clarity**: While types are dynamic, variables must be dimensioned to be used, eliminating ambiguity over whether a variable is being created or simply assigned
*   **Everything is a function**: Operators are rarely used, and functions are favored
*   **Unique syntax**: Blend elements from various influences for a fun and readable coding experience
*	**Imports on a functional basis**: Namespaces cannot be accessed by dot notation, and imports are done in each function at runtime. It is impossible to pollute the global namespace
*	**Return value posting**: Return values of functions can be set without returning, saving boilerplate

## Getting Started

### Prerequisites

*   Just install the runtime

### Installing the Language Runtime

To get started, download the latest release from the [releases page](https://github.com/[rocky-horror]/cathode/releases) and follow the installation instructions.

## Code Examples

## Preferred Code Style

*	Keywords and operators are lowercase
*	Standard library functions are PascalCase, with an optional lowercase single-character prefix to indicate the specialty of the function
*	Variables are recommended to be PascalCase or camelCase

```
namespace OurNamespace ; this is for determining the namespace for files designed to be imported. It is ignored in the main file

import("conio") ; import is used like a function, but it is viewed as a keyword and is lowercase
dim ourStruct = Struct() ; dim is a keyword for dimensioning variables

PrintLn(typeof(ourStruct)) ; outputs "struct". typeof is also a keyword used like a function

```

### Main Function Format

```
fndef Main accepts args
	
fnret
```

### Basic Arithmetic

```
; Import console I/O namespace
import("conio")

; Add two numbers together
dim x = 5 + 3
PrintLn(x)
```

### Function Definition

```
fndef PrintNameAndAge accepts name, age
	import("conio")
	PrintLn(Format("$0 is $1 years old!", { name, age }))
fnret

PrintNameAndAge("John", 27)
```

### Struct Use 

```
fndef CreateStruct
	Dim ourStruct = Struct()
	
	; Struct fields are created upon their use if they do not already exist
	ourStruct.Numbers = { 13, 29, 42, 86 }
fnret

Dim result = CreateStruct()
import("conio")
PrintLn(result)
```