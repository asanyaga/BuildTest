ALTER TABLE tblContact DROP CONSTRAINT [FK_tblContact_tblMaritalStatus];
alter table tblContact drop column MaritalStatus;
Alter Table tblContact Add MaritalStatusId int null default 0;
DROP TABLE tblMaritalStatus
GO

ALTER TABLE tblCentre DROP CONSTRAINT [Centre_Route_FK];
alter table tblCentre drop column RouteId;
Alter Table tblCentre Add RegionId uniqueidentifier  null;
ALTER TABLE tblCentre ADD CONSTRAINT [Centre_Region_FK] FOREIGN KEY ([RegionId]) REFERENCES [tblRegion] ([id])
GO

ALTER TABLE tblRoutes DROP CONSTRAINT [FK_tblRoutes_tblCostCentre];
alter table tblRoutes drop column DistributorID;
Alter Table tblRoutes Add RegionId uniqueidentifier  null;
ALTER TABLE tblRoutes ADD CONSTRAINT [FK_tblRoutes_tblRoute] FOREIGN KEY ([RegionId]) REFERENCES [tblRegion] ([id])
GO

CREATE TABLE tblMasterDataAllocation
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


