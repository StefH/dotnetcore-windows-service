@echo off

SET MessageReceiverAsAServiceInstallPath=C:\Services\MessageReceiverAsAService\

dotnet clean --configuration Release --runtime win7-x64
dotnet publish --configuration Release --runtime win7-x64 --output %MessageReceiverAsAServiceInstallPath%
