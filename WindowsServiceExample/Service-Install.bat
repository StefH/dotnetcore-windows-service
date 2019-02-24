@echo off
call Service-Uninstall.bat

call Publish.bat

SET targetpath=C:\Services\WindowsServiceExample\

sc create WindowsServiceExample binPath= "%targetpath%WindowsServiceExample.exe"