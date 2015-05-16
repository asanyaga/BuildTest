

if not exists(select column_name from information_schema.columns where table_name = 'tblCooler' and column_name = 'Name')
	alter table tblCooler add Name nvarchar(250)