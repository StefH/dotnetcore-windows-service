@echo off

SET ServiceInstallPath=C:\Services\WindowsServiceExample

dotnet clean --configuration Release --runtime win7-x64
dotnet publish --configuration Release --runtime win7-x64 -o %ServiceInstallPath%
