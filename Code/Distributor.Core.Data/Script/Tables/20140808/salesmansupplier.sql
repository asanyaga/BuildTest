IF (NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE   TABLE_NAME = 'tblSalesmanSupplier'))
	BEGIN
		CREATE TABLE [tblSalesmanSupplier](
		[id] [uniqueidentifier] NOT NULL primary Key ,
		[SupplierId] [uniqueidentifier] NOT NULL,
		[CostCentreId] [uniqueidentifier] NOT NULL,		
		[IM_DateCreated] [datetime] NOT NULL,
		[IM_DateLastUpdated] [datetime] NOT NULL,
		[IM_Status] [int] NOT NULL
		) ; 
		 ALTER TABLE tblSalesmanSupplier ADD CONSTRAINT SalesmanSupplier_Supplier_FK FOREIGN KEY ([SupplierId]) REFERENCES [tblSupplier](Id);
		  ALTER TABLE tblSalesmanSupplier ADD CONSTRAINT SalesmanSupplier_CostCentre_FK FOREIGN KEY ([CostCentreId]) REFERENCES [tblCostCentre](Id);
END




