@echo off

SET targetpath="C:\Services\WindowsServiceExample"

dotnet clean --configuration Release --runtime win7-x64
rem call dotnet restore --force --runtime win7-x64
rem dotnet build --force --configuration Release --runtime win7-x64 --output %targetpath%
dotnet publish --configuration Release --runtime win7-x64 --output %targetpath%
