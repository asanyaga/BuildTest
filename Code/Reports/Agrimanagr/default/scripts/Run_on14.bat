@Echo Off

FOR /f %%i IN ('DIR *.Sql /B') do call :RunScript %%i

GOTO :END

 

:RunScript

Echo Executing %1

SQLCMD -S 10.0.0.14 -d ag3 -U sa -P P@ssw0rd -i %1

Echo Completed %1

 

:END