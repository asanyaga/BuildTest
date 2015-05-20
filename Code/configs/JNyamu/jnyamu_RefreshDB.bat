REM THIS FILE SHOULD BE RUN IN ADMIN MODE AND POWERSHELL SHOULD BE INSTALLED
f:
cd\Projects\Distributr-Git\configs\JNyamu
REM
REM UPDATE CONFIG FILES 
CALL jnyamu_setup.bat
cd..
cd..
cd rebuilddb
C:\Windows\Microsoft.NET\Framework\v4.0.30319\msbuild /verbosity:m rebuilddb.csproj /p:OutputPath=bin\Debug
cd..
cd configs
C:\Windows\Microsoft.NET\Framework\v4.0.30319\msbuild setup/setup.xml /t:RefreshDatabases /p:Environment=jnyamu
cd..
cd RebuildDB\bin\Debug
 rebuilddb.exe 
cd ..\..\..\configs
PAUSE