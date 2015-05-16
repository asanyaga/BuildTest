for %%G in (*.sql) do sqlcmd /S . /d DistributrLocal    -E -i "%%G"
pause