
begin tran t1
----alter table tblUserGroupRoles drop constraint [GroupId]

----select * from sys.foreign_keys
----rollback tran t1

if(
		select 
			count(
			tc.constraint_name
			--,tc.table_name
			--,kcu.column_name, 
			--ccu.table_name as foreign_table_name,
			--ccu.column_name as foreign_column_name
			) 
		from 
			information_schema.table_constraints as tc 
			JOIN information_schema.key_column_usage as kcu on tc.constraint_name = kcu.constraint_name
			JOIN information_schema.constraint_column_usage as ccu on ccu.constraint_name = tc.constraint_name
		where constraint_type = 'FOREIGN KEY' AND tc.table_name='tblUserGroupRoles'
)> 1
begin
		declare @tempTable table(id int identity(1,1), foreign_key nvarchar(100))
		insert into @tempTable(foreign_key)
		select 
			tc.constraint_name
		from 
			information_schema.table_constraints as tc 
			JOIN information_schema.key_column_usage as kcu on tc.constraint_name = kcu.constraint_name
			JOIN information_schema.constraint_column_usage as ccu on ccu.constraint_name = tc.constraint_name
		where constraint_type = 'FOREIGN KEY' AND tc.table_name='tblUserGroupRoles'
		
		select		* from @tempTable
		declare		@countr int;
		set			@countr = (select min(id) from @tempTable)+ 1;
		select		@countr
		while(@countr <= (select max(id) from @tempTable))
			begin
				declare		@foreignKey nvarchar(100);
				set			@foreignKey = (select foreign_key from @tempTable where id = @countr);
				print		@foreignKey;
				
				select		constraint_name from information_schema.table_constraints where constraint_name = @foreignKey
				alter		table	tblUserGroupRoles drop constraint (select constraint_name from information_schema.table_constraints where constraint_name = @foreignKey)
							
				set			@countr = @countr+1;
			end
end


--SELECT
--K_Table = FK.TABLE_NAME,
--FK_Column = CU.COLUMN_NAME,
--PK_Table = PK.TABLE_NAME,
--PK_Column = PT.COLUMN_NAME,
--Constraint_Name = C.CONSTRAINT_NAME
--FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS C
--INNER JOIN INFORMATION_SCHEMA.TABLE_CONSTRAINTS FK ON C.CONSTRAINT_NAME = FK.CONSTRAINT_NAME
--INNER JOIN INFORMATION_SCHEMA.TABLE_CONSTRAINTS PK ON C.UNIQUE_CONSTRAINT_NAME = PK.CONSTRAINT_NAME
--INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE CU ON C.CONSTRAINT_NAME = CU.CONSTRAINT_NAME
--INNER JOIN (
--SELECT i1.TABLE_NAME, i2.COLUMN_NAME
--FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS i1
--INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE i2 ON i1.CONSTRAINT_NAME = i2.CONSTRAINT_NAME
--WHERE i1.CONSTRAINT_TYPE = 'PRIMARY KEY'
--) PT ON PT.TABLE_NAME = PK.TABLE_NAME
--where fk.table_name='tblUserGroupRoles'

select 
			--count(
			tc.constraint_name
			,tc.table_name
			,kcu.column_name, 
			ccu.table_name as foreign_table_name,
			ccu.column_name as foreign_column_name
			--) 
		from 
			information_schema.table_constraints as tc 
			JOIN information_schema.key_column_usage as kcu on tc.constraint_name = kcu.constraint_name
			JOIN information_schema.constraint_column_usage as ccu on ccu.constraint_name = tc.constraint_name
		where constraint_type = 'FOREIGN KEY' AND tc.table_name='tblUserGroupRoles'