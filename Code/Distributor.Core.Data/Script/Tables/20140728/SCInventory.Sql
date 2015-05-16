
IF (NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE   TABLE_NAME = 'tblCommoditySupplierInventory'))
	BEGIN
		CREATE TABLE [dbo].[tblCommoditySupplierInventory](
	[id] [uniqueidentifier] NOT NULL primary key,
	[WareHouseId] [uniqueidentifier] NOT NULL,
	[CommodityId] [uniqueidentifier] NOT NULL,
	[SupplierId] [uniqueidentifier] NOT NULL,
	[GradeId] [uniqueidentifier] NOT NULL,
	[Balance] [decimal](20, 2) NULL,
	[UnavailableBalance] [decimal](20, 2) NOT NULL,
	[IM_DateCreated] [datetime] NOT NULL,
	[IM_DateLastUpdated] [datetime] NOT NULL,
	[IM_Status] [int] NOT NULL
) ;

 ALTER TABLE tblCommoditySupplierInventory ADD CONSTRAINT CommoditySupplierInventory_CostCentre_FK FOREIGN KEY ([SupplierId]) REFERENCES [tblCostCentre](Id);
 ALTER TABLE tblCommoditySupplierInventory ADD CONSTRAINT WareHouseInventory_CostCentre_FK FOREIGN KEY ([WareHouseId]) REFERENCES [tblCostCentre](Id);

 END



