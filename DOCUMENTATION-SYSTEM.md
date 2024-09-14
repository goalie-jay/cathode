# Cathode Standard Library Documentation

## Namespace "system"

### Functions

#### Sys(command)

*	Arguments: command (string)
*	Returns: integer
*	Description: Runs the given command with the default shell of the system and returns the return value

#### Sleep(time)

*	Arguments: time (integer)
*	Returns: void
*	Description: Sleeps for the given amount of time in milliseconds

#### Env(name)

*	Arguments: name (string)
*	Returns: string, void
*	Description: Tries to get the value of an environment variable with the given name and returns it if successful. Otherwise, void is returned