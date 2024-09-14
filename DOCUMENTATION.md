# Cathode Standard Library And Types Documentation

## Namespaces

*	[core](https://github.com/rocky-horror/cathode/blob/master/DOCUMENTATION-CORE.md)
*	[system](https://github.com/rocky-horror/cathode/blob/master/DOCUMENTATION-SYSTEM.md)
*	[conio](https://github.com/rocky-horror/cathode/blob/master/DOCUMENTATION-CONIO.md)
*	[fileio](https://github.com/rocky-horror/cathode/blob/master/DOCUMENTATION-FILEIO.md)
*	[array](https://github.com/rocky-horror/cathode/blob/master/DOCUMENTATION-ARRAY.md)

## Data types

*	[string](https://github.com/rocky-horror/cathode/blob/master/DOCUMENTATION-DATATYPE-STRING.md)
*	[float](https://github.com/rocky-horror/cathode/blob/master/DOCUMENTATION-DATATYPE-FLOAT.md)
*	[integer](https://github.com/rocky-horror/cathode/blob/master/DOCUMENTATION-DATATYPE-INTEGER.md)
*	[byte](https://github.com/rocky-horror/cathode/blob/master/DOCUMENTATION-DATATYPE-BYTE.md)
*	[filehandle](https://github.com/rocky-horror/cathode/blob/master/DOCUMENTATION-DATATYPE-FILEHANDLE.md)
*	[void](https://github.com/rocky-horror/cathode/blob/master/DOCUMENTATION-DATATYPE-VOID.md)

## Operators

### typeof(obj)

*	Returns the type of that object at the time of calling
	
### import(str)

*	Imports the given namespace

### !

*	Inverts the logical expression in front of it

### -(value)

*	Flips the sign of the number type inside the parentheses

### [] (array[index])

*	Produces the value at the given index of the array to be used in an expression

### &&

*	Produces a nonzero value if the expressions on both sides evaluate to true. Otheriwse, zero is returned
*	The expressions on both sides are always evaluated

### ||

*	Produces a nonzero value if either of the expressions on both sides evaluate to true. Otherwise, zero is returned
*	The expressions on both sides are always evaluated