alter table tblCentre drop constraint [Centre_Route_FK]
alter table tblCentre drop column RouteId
alter table tblCentre add HubId uniqueidentifier not null
alter table tblCentre add constraint [Centre_Hub_FK] foreign key ([HubId]) references [tblCostCentre] ([id])
GO

alter table tblCommodityProducer drop column RegionId

alter table tblContact drop constraint [FK_tblContact_tblMaritalStatus]
alter table tblContact drop column MaritalStatus
alter table tblContact add MaritalStatusId int null default 0
alter table tblContact drop constraint [FK_tblContact_tblCostCentre]

drop table tblMaritalStatus

create table tblMasterDataAllocation
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

create table tblContainerType
(
	[Id] [uniqueidentifier] NOT NULL Primary Key,	
	[Code] [nvarchar](50) NULL,
	[Name] [nvarchar](50) NOT NULL,
	[Description] [nvarchar](250) NULL,
	[LoadCariage] decimal(18,2) NULL,
	[TareWeight] decimal (18,2) NULL,
	[Lenght] decimal (18,2) NULL,
	[Width] decimal(18,2) NULL,
	[Height] decimal(18,2) NULL,
	[BubbleSpace] decimal(18,2) NULL,
	[Volume] decimal(18,2) NULL,
	[FreezerTemp] decimal(18,2) NULL,
	[Make] [nvarchar](50) NOT NULL,	
	[Model] [nvarchar](50) NOT NULL,	
	[CommodityGradeId] [uniqueidentifier] NULL,
	[IM_DateCreated] [datetime] NOT NULL,
	[IM_DateLastUpdated] [datetime] NOT NULL,
	[IM_Status] int NOT NULL
	);
	
alter table tblContainerType add  ContainerUseId int null;	
alter table tblContainerType add constraint ContainerType_CommodityGrade_FK foreign key (CommodityGradeId) references tblCommodityGrade(Id);

alter table tblEquipment drop column LoadCariage;
alter table tblEquipment drop column TareWeight;
alter table tblEquipment drop column Lenght;
alter table tblEquipment drop column Width;
alter table tblEquipment drop column Height;
alter table tblEquipment drop column BubbleSpace;
alter table tblEquipment drop column Volume;
alter table tblEquipment drop column FreezerTemp;
alter table tblEquipment drop column ContainerType;
alter table tblEquipment add  ContainerTypeId uniqueidentifier null;

alter table tblEquipment drop constraint Equipment_CommodityGrade_FK;
alter table tblEquipment drop column CommodityGradeId;

alter table tblEquipment add constraint Equipment_ContainerType_FK foreign key (ContainerTypeId) references tblContainerType(Id);

alter table tblSourcingDocument add  DriverName varchar(50) null;
alter table tblSourcingDocument add  VehicleRegNo varchar(50) null;

alter table tblSourcingLineItem add  ContainerNo varchar(50) null;
alter table tblSourcingLineItem add  LineItemStatusId int;

alter table tblSettings add  AppId int;

alter table tblRoutes
add RegionId uniqueidentifier


update tblRoutes
set tblRoutes.RegionId = tblCostCentre.Distributor_RegionId
from tblRoutes
inner join tblCostCentre on tblRoutes.DistributorID = tblCostCentre.Id

alter table tblRoutes drop constraint FK_tblRoutes_tblCostCentre;
alter table tblRoutes drop column DistributorID;
alter table tblRoutes add constraint FK_tblRoutes_tblRoute foreign key (RegionId) references tblRegion(Id);
