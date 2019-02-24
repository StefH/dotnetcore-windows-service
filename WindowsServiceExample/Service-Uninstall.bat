@echo off
SET mypath=%~dp0
SET targetpath=C:\Services\WindowsServiceExample\

sc stop WindowsServiceExample
rem C:\Windows\Microsoft.NET\Framework\v4.0.30319\InstallUtil.exe /u "%mypath%WindowsServiceExample.exe"
sc delete WindowsServiceExample binPath= "%targetpath%WindowsServiceExample.exe"

rmdir /S /Q "%targetpath%"