@echo off

dotnet publish -c Release -p:DebugSymbols=false
iscc .\bin\Release\net8.0\win-x64\publish\setupScript.iss