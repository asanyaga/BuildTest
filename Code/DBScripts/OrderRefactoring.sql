Alter Table tblDocument Add OrderStatusId int null default 0;
Alter Table tblLineItems Add LineItemStatusId int null default 0;
Alter Table tblLineItems Add ApprovedQuantity decimal(18,2) null default 0;
Alter Table tblLineItems Add LostSaleQuantity decimal(18,2) null default 0;
Alter Table tblLineItems Add BackOrderQuantity decimal(18,2) null default 0;
Alter Table tblDocument Add OrderParentId uniqueidentifier null ;
Alter Table tblLineItems Add DispatchedQuantity decimal(18,2) null default 0;

ALTER TABLE [dbo].[tblContact] DROP CONSTRAINT [FK_tblContact_tblMaritalStatus];
alter table tblContact drop column MaritalStatus;
Alter Table tblContact Add MaritalStatusId int null default 0;

ALTER TABLE [dbo].[tblCentre] DROP CONSTRAINT [Centre_Route_FK];
alter table tblCentre drop column RouteId;
Alter Table tblCentre Add RegionId uniqueidentifier  null;
ALTER TABLE [dbo].[tblCentre] ADD CONSTRAINT [Centre_Region_FK] FOREIGN KEY ([RegionId]) REFERENCES [dbo].[tblRegion] ([id])
GO

ALTER TABLE [dbo].[tblRoutes] DROP CONSTRAINT [FK_tblRoutes_tblCostCentre];
alter table tblRoutes drop column DistributorID;
Alter Table tblRoutes Add RegionId uniqueidentifier  null;
ALTER TABLE [dbo].[tblRoutes] ADD CONSTRAINT [FK_tblRoutes_tblRoute] FOREIGN KEY ([RegionId]) REFERENCES [dbo].[tblRegion] ([id])
GO

CREATE TABLE [dbo].[tblMasterDataAllocation]
(
[Id] [uniqueidentifier] NOT NULL,
[EntityAId] [uniqueidentifier] NOT NULL,
[EntityBId] [uniqueidentifier] NOT NULL,
[AllocationType] [int] NOT NULL,
[IM_DateCreated] [datetime] NOT NULL,
[IM_DateLastUpdated] [datetime] NOT NULL,
[IM_Status] [int] NOT NULL
)
GO

