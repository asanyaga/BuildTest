SET ECHO OFF
ECHO RESTARTING SQL SERVER
c:\windows\system32\windowspowershell\v1.0\powershell.exe Get-ExecutionPolicy
c:\windows\system32\windowspowershell\v1.0\powershell.exe -file configs\buildserverrestartsql.ps1