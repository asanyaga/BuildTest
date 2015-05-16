
begin tran t1
if exists(select * from sys.foreign_keys
		where	object_id = OBJECT_ID(N'dbo.FK_tblContact_tblCostCentre')
		and		parent_object_id = OBJECT_ID(N'dbo.tblContact'))
			print 'Found ya'
			alter table tblContact drop constraint FK_tblContact_tblCostCentre
commit  tran t1