create table tblRecollection(
    [Id] [uniqueidentifier] NOT NULL primary key,
	[CostCentreId] [uniqueidentifier] NOT NULL,
	[FromCostCentreId] [uniqueidentifier] NOT NULL,	
	[Description] [varchar](50) NULL,
	[Amount] decimal(16,2) Not NULL,
	IsReceived bit not null,
	[DateInserted] [datetime] NOT NULL,
	[IM_DateCreated] [datetime] NOT NULL,
	[IM_DateLastUpdated] [datetime] NOT NULL,	
	[IM_Status] [int] NOT NULL,
);
create table tblRecollectionItem(
    [Id] [uniqueidentifier] NOT NULL primary key,	
	RecollectionId [uniqueidentifier] NOT NULL,		
	[Amount] decimal(16,2) Not NULL,
	[DateInserted] [datetime] NOT NULL,
	[IM_DateCreated] [datetime] NOT NULL,
	[IM_DateLastUpdated] [datetime] NOT NULL,	
	[IM_Status] [int] NOT NULL,
);

ALTER TABLE tblRecollectionItem ADD CONSTRAINT RecollectionItem_Recollection_FK FOREIGN KEY (RecollectionId) REFERENCES tblRecollection(Id);