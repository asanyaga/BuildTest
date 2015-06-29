REM THIS FILE SHOULD BE RUN IN ADMIN MODE AND POWERSHELL SHOULD BE INSTALLED
C:
cd \Users\bmwasi\Documents\GitHub\Agrimanagr-Forked\Code\configs
REM
REM UPDATE CONFIG FILES 
CALL mwasi.bat
cd..
cd rebuilddb
C:\Windows\Microsoft.NET\Framework\v4.0.30319\msbuild /verbosity:m rebuilddb.csproj /p:OutputPath=bin\Debug
cd..
cd configs
C:\Windows\Microsoft.NET\Framework\v4.0.30319\msbuild setup/setup.xml /t:RefreshDatabases /p:Environment=bmwasi
cd..
cd RebuildDB\bin\Debug
 rebuilddb.exe 
cd ..\..\..\configs
PAUSE