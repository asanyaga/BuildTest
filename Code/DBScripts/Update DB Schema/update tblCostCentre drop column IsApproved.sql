
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF__tblCostCe__IsApp__4CA1FA10]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[tblCostCentre] DROP CONSTRAINT [DF__tblCostCe__IsApp__4CA1FA10]
END

GO


if exists (select column_name from information_schema.columns where table_name = 'tblCostCentre' and column_name = 'IsApproved')
	begin
		print 'found column'
		print 'deleting column'
		alter table tblCostCentre drop column IsApproved
	end
	
