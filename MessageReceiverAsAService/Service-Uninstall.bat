@echo off
SET mypath=%~dp0
SET MessageReceiverAsAServiceInstallPath=C:\Services\MessageReceiverAsAService\

sc stop MessageReceiverAsAService
SLEEP 1
sc delete MessageReceiverAsAService binPath= "%MessageReceiverAsAServiceInstallPath%MessageReceiverAsAService.exe"

rmdir /S /Q "%MessageReceiverAsAServiceInstallPath%"
