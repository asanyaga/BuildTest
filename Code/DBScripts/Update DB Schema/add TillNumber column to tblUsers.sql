

if not exists (select column_name from INFORMATION_SCHEMA.columns where table_name = 'tblUsers' and column_name = 'TillNumber')
alter table tblUsers add TillNumber int
