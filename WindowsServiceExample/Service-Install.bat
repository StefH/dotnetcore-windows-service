@echo off
call Service-Uninstall.bat

call Publish.bat

SET ServiceInstallPath=C:\Services\WindowsServiceExample\

sc create WindowsServiceExample binPath= "%ServiceInstallPath%WindowsServiceExample.exe"