@echo off
call Service-Uninstall.bat

call Publish.bat

SET MessageReceiverAsAServiceInstallPath=C:\Services\MessageReceiverAsAService\

sc create MessageReceiverAsAService binPath= "%MessageReceiverAsAServiceInstallPath%MessageReceiverAsAService.exe"