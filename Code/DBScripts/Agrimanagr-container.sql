
	CREATE TABLE [dbo].[tblContainerType](
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
ALTER TABLE [dbo].[tblContainerType] ADD CONSTRAINT ContainerType_CommodityGrade_FK FOREIGN KEY (CommodityGradeId) REFERENCES tblCommodityGrade(Id);
alter table [dbo].[tblEquipment] drop column LoadCariage;
alter table [dbo].[tblEquipment] drop column TareWeight;
alter table [dbo].[tblEquipment] drop column Lenght;
alter table [dbo].[tblEquipment] drop column Width;
alter table [dbo].[tblEquipment] drop column Height;
alter table [dbo].[tblEquipment] drop column BubbleSpace;
alter table [dbo].[tblEquipment] drop column Volume;
alter table [dbo].[tblEquipment] drop column FreezerTemp;
alter table [dbo].[tblEquipment]  column Equipment_CommodityGrade_FK;
alter table [dbo].[tblEquipment] drop column CommodityGradeId;
alter table [dbo].[tblEquipment] drop column ContainerType;
alter table [dbo].[tblEquipment] add  ContainerTypeId uniqueidentifier null;
ALTER TABLE [dbo].[tblEquipment] ADD CONSTRAINT Equipment_ContainerType_FK FOREIGN KEY (ContainerTypeId) REFERENCES tblContainerType(Id);

alter table [dbo].[tblSourcingLineItem] add  ContainerNo varchar(50) null;

---03-mar-2013
alter table [dbo].[tblContainerType] add  ContainerUseId;

---13-march-2013
alter table [dbo].[tblSourcingDocument] add  DriverName varchar(50) null;
alter table [dbo].[tblSourcingDocument] add  VehicleRegNo varchar(50) null;

alter table [dbo].[tblSourcingLineItem] add  LineItemStatusId int;
alter table [dbo].[tblSettings] add  AppId int;











