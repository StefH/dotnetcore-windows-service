@echo off
SET mypath=%~dp0
SET ServiceInstallPath=C:\Services\WindowsServiceExample\

sc stop WindowsServiceExample
rem C:\Windows\Microsoft.NET\Framework\v4.0.30319\InstallUtil.exe /u "%mypath%WindowsServiceExample.exe"
sc delete WindowsServiceExample binPath= "%ServiceInstallPath%WindowsServiceExample.exe"

rmdir /S /Q "%ServiceInstallPath%"
