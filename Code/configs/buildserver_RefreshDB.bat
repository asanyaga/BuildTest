REM THIS FILE SHOULD BE RUN IN ADMIN MODE AND POWERSHELL SHOULD BE INSTALLED

cd C:\Program Files (x86)\Jenkins\workspace\VCBuild1\configs
REM
REM UPDATE CONFIG FILES 
CALL buildserver_setup.bat
cd..
cd rebuilddb
C:\Windows\Microsoft.NET\Framework\v4.0.30319\msbuild /verbosity:m rebuilddb.csproj
cd..
cd configs
@powershell -file "RestartSQL.ps1"
@powershell -file "RestartSQL.ps1"
C:\Windows\Microsoft.NET\Framework\v4.0.30319\msbuild setup/setup.xml 
cd..
cd RebuildDB\bin\Debug
rebuilddb.exe 
cd ..\..\..\configs
PAUSE