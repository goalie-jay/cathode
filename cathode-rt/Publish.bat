@echo off

dotnet publish -c Release
iscc .\bin\Release\net8.0\win-x64\publish\setupScript.iss