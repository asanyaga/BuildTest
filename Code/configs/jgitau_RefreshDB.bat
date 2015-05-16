REM THIS FILE SHOULD BE RUN IN ADMIN MODE AND POWERSHELL SHOULD BE INSTALLED
d:
cd\project\distributr\configs
REM
REM UPDATE CONFIG FILES 
CALL jgitau_setup.bat
cd..
cd rebuilddb
C:\Windows\Microsoft.NET\Framework\v4.0.30319\msbuild /verbosity:m rebuilddb.csproj /p:OutputPath=bin\Debug
cd..
cd configs
@powershell -file "jgitauRestartSQL.ps1"
@powershell -file "jgitauRestartSQL.ps1"
C:\Windows\Microsoft.NET\Framework\v4.0.30319\msbuild setup/setup.xml /t:RefreshDatabases /p:Environment=jgitau
cd..
cd RebuildDB\bin\Debug
 rebuilddb.exe 
cd ..\..\..\configs
PAUSE