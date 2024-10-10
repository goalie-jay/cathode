# Cathode Standard Library And Types Documentation

## Namespaces

*	[core](https://github.com/rocky-horror/cathode/blob/master/DOCUMENTATION-CORE.md)
*	[system](https://github.com/rocky-horror/cathode/blob/master/DOCUMENTATION-SYSTEM.md)
*	[conio](https://github.com/rocky-horror/cathode/blob/master/DOCUMENTATION-CONIO.md)
*	[fileio](https://github.com/rocky-horror/cathode/blob/master/DOCUMENTATION-FILEIO.md)
*	[array](https://github.com/rocky-horror/cathode/blob/master/DOCUMENTATION-ARRAY.md)
*	[network](https://github.com/rocky-horror/cathode/blob/master/DOCUMENTATION-NETWORK.md)
*	[tcpio](https://github.com/rocky-horror/cathode/blob/master/DOCUMENTATION-TCPIO.md)
*	[string](https://github.com/rocky-horror/cathode/blob/master/DOCUMENTATION-STRING.md)
*	[random](https://github.com/rocky-horror/cathode/blob/master/DOCUMENTATION-RANDOM.md)
*	[process](https://github.com/rocky-horror/cathode/blob/master/DOCUMENTATION-PROCESS.md)
*	[serialize](https://github.com/rocky-horror/cathode/blob/master/DOCUMENTATION-SERIALIZE.md)

## Data types

*	[string](https://github.com/rocky-horror/cathode/blob/master/DOCUMENTATION-DATATYPE-STRING.md)
*	[float](https://github.com/rocky-horror/cathode/blob/master/DOCUMENTATION-DATATYPE-FLOAT.md)
*	[integer](https://github.com/rocky-horror/cathode/blob/master/DOCUMENTATION-DATATYPE-INTEGER.md)
*	[byte](https://github.com/rocky-horror/cathode/blob/master/DOCUMENTATION-DATATYPE-BYTE.md)
*	[filehandle](https://github.com/rocky-horror/cathode/blob/master/DOCUMENTATION-DATATYPE-FILEHANDLE.md)
*	[void](https://github.com/rocky-horror/cathode/blob/master/DOCUMENTATION-DATATYPE-VOID.md)

## Operators

### Important Note

Expressions are evaluated from right to left. Keep that in mind when writing expressions.

### typeof( obj )

*	Returns the type of that object at the time of calling
	
### import( str )

*	Imports the given namespace

### ( expr )

*	Isolates the given expression, useful for ensuring expressions are evaluated in the correct order

### !

*	Inverts the logical expression in front of it

### -value

*	Flips the sign of the number type supplied

### [] (array[index])

*	Produces the value at the given index of the array to be used in an expression

### &&

*	Produces a nonzero value if the expressions on both sides evaluate to true. Otheriwse, zero is returned
*	The expressions on both sides are always evaluated

### ||

*	Produces a nonzero value if either of the expressions on both sides evaluate to true. Otherwise, zero is returned
*	The expressions on both sides are always evaluated

#### <

*	Produces a nonzero value if the expression on the left side is less than the expression on the right side. Otherwise, zero is returned

#### >

*	Produces a nonzero value if the expression on the left side is greater than the expression on the right side. Otherwise, zero is returned

#### ==

*	Produces a nonzero value if the expression on the left side is equal to the expression on the right side. Otherwise, zero is returned

#### !=

*	Produces a nonzero value if the expression on the left side is not equal to the expression on the right side. Otherwise, zero is returned

#### inc( identifier )

*	Increments the given identifier by one. The identifier must be a single variable
*	If you're doing a loop where the increment is one (or a constant that can be achieved through multiplying the iterator) and speed is a necessity, this is the fastest way to increment your index variable
*	This is really only a useful optimization if you're looping thousands of times

#### mkref( functionIdentifier )

*	Produces a reference to the given function, which must be passed as an identifier constant
*	References produced with mkref( ) do not need to be closed, disposed, or cleaned up in any way

#### deref( ref )

*	Produces the function pointed to by the given reference