IF (NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE   TABLE_NAME = 'tblInventoryImports'))
	BEGIN
		CREATE TABLE [tblInventoryImports](
		[id] [uniqueidentifier] NOT NULL primary Key ,
		[Ref] [varchar](250) NOT NULL,
		[WarehouseCode] [varchar](250) NOT NULL,
		[ProductCode] [varchar](250) NOT NULL,
		[Quantity] decimal(18,2) NOT NULL,
		[IsProcessed] [bit] NOT NULL,
		[Info] [varchar](250)  NULL,
		[IM_DateCreated] [datetime] NOT NULL,
		[IM_DateLastUpdated] [datetime] NOT NULL,
		[IM_Status] [int] NOT NULL
		)
 ; END




