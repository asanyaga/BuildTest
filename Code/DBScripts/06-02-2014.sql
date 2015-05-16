

CREATE TABLE [tblSourcingInventory](
	[id] [uniqueidentifier] NOT NULL primary key,
	[WareHouseId] [uniqueidentifier] NOT NULL,
	[CommodityId] [uniqueidentifier] NOT NULL,
	[GradeId] [uniqueidentifier] NOT NULL,
	[Balance] [decimal](20, 2) NULL,
	[UnavailableBalance] [decimal](20, 2) NOT NULL,	
	[IM_DateCreated] [datetime] NOT NULL,
	[IM_DateLastUpdated] [datetime] NOT NULL,	
	[IM_Status] [int] NOT NULL,
 )

