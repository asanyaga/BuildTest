CREATE TABLE [dbo].[tblCentreType](
	[Id] [uniqueidentifier] NOT NULL Primary Key,	
	[Code] [nvarchar](50) NULL,
	[Name] [nvarchar](50) NOT NULL,
	[Description] [nvarchar](250) NULL,
	[IM_DateCreated] [datetime] NOT NULL,
	[IM_DateLastUpdated] [datetime] NOT NULL,
	[IM_Status] int NOT NULL
	);
CREATE TABLE [dbo].[tblCentre](
	[Id] [uniqueidentifier] NOT NULL Primary Key,
	[CentreTypeId] [uniqueidentifier] not null,
	[RouteId] [uniqueidentifier] not null,
	[Code] [nvarchar](50) NULL,
	[Name] [nvarchar](50) NOT NULL,
	[Description] [nvarchar](250) NULL,
	[IM_DateCreated] [datetime] NOT NULL,
	[IM_DateLastUpdated] [datetime] NOT NULL,
	[IM_Status] int NOT NULL
	);
ALTER TABLE [dbo].[tblCentre] ADD CONSTRAINT Centre_Route_FK FOREIGN KEY (RouteId) REFERENCES tblRoutes(RouteID);
ALTER TABLE [dbo].[tblCentre] ADD CONSTRAINT Centre_CentreType_FK FOREIGN KEY (CentreTypeId) REFERENCES tblCentreType(Id);
CREATE TABLE [dbo].[tblCommodityType](
	[Id] [uniqueidentifier] NOT NULL Primary Key,	
	[Code] [nvarchar](50) NULL,
	[Name] [nvarchar](50) NOT NULL,
	[Description] [nvarchar](250) NULL,
	[IM_DateCreated] [datetime] NOT NULL,
	[IM_DateLastUpdated] [datetime] NOT NULL,
	[IM_Status] int NOT NULL
	);
CREATE TABLE [dbo].[tblCommodity](
	[Id] [uniqueidentifier] NOT NULL Primary Key,	
	[Code] [nvarchar](50) NULL,
	[Name] [nvarchar](50) NOT NULL,
	[Description] [nvarchar](250) NULL,
	[CommodityTypeId] [uniqueidentifier] not null,
	[IM_DateCreated] [datetime] NOT NULL,
	[IM_DateLastUpdated] [datetime] NOT NULL,
	[IM_Status] int NOT NULL
	);
ALTER TABLE [dbo].[tblCommodity] ADD CONSTRAINT Commodity_CommodityType_FK FOREIGN KEY (CommodityTypeId) REFERENCES tblCommodityType(Id);

CREATE TABLE [dbo].[tblCommodityGrade](
	[Id] [uniqueidentifier] NOT NULL Primary Key,
    [CommodityId] [uniqueidentifier] NOT NULL,	
	[Code] [nvarchar](50) NULL,
	[Name] [nvarchar](50) NOT NULL,
	[Description] [nvarchar](250) NULL,
	[UsageTypeId] int not null,
	[IM_DateCreated] [datetime] NOT NULL,
	[IM_DateLastUpdated] [datetime] NOT NULL,
	[IM_Status] int NOT NULL
	);	
ALTER TABLE [dbo].[tblCommodityGrade] ADD CONSTRAINT CommodityGrade_Commodity_FK FOREIGN KEY (CommodityId) REFERENCES tblCommodity(Id);

CREATE TABLE [dbo].[tblCommodityOwnerType](
	[Id] [uniqueidentifier] NOT NULL Primary Key,	
	[Code] [nvarchar](50) NULL,
	[Name] [nvarchar](50) NOT NULL,
	[Description] [nvarchar](250) NULL,
	[IM_DateCreated] [datetime] NOT NULL,
	[IM_DateLastUpdated] [datetime] NOT NULL,
	[IM_Status] int NOT NULL
	);
CREATE TABLE [dbo].[tblCommodityOwner](
	[Id] [uniqueidentifier] NOT NULL Primary Key,	
	[Code] [nvarchar](50) NULL,
	[Surname] [nvarchar](50) NOT NULL,
	[FirstName] [nvarchar](50) NOT NULL,
	[LastName] [nvarchar](50) NOT NULL,
	[Gender] int NOT NULL,
	[IdNo] [nvarchar](50) NOT NULL,
	[PINNo] [nvarchar](50) NOT NULL,
	[DOB] datetime NOT NULL,
	[MaritalStatusId] [uniqueidentifier] NOT NULL,
	[PhysicalAddress] [nvarchar](250) NULL,
	[PostalAddress] [nvarchar](250) NULL,
	[Email] [nvarchar](250) NULL,
	[PhoneNo] [nvarchar](20) NULL,
	[BusinessNo] [nvarchar](20) NULL,
	[FaxNo] [nvarchar](20) NULL,
	[OfficeNo] [nvarchar](20) NULL,
	[Description] [nvarchar](250) NULL,
	[CommodityOwnerTypeId] [uniqueidentifier] NOT NULL,
	[CostCentreId] [uniqueidentifier] NOT NULL,
	[IM_DateCreated] [datetime] NOT NULL,
	[IM_DateLastUpdated] [datetime] NOT NULL,
	[IM_Status] int NOT NULL
	);
ALTER TABLE [dbo].[tblCommodityOwner] ADD CONSTRAINT CommodityOwner_CommodityOwnerType_FK FOREIGN KEY (CommodityOwnerTypeId) REFERENCES tblCommodityOwnerType(Id);
ALTER TABLE [dbo].[tblCommodityOwner] ADD CONSTRAINT CommodityOwner_CommodityOwnerSupplier_FK FOREIGN KEY (CostCentreId) REFERENCES tblCostCentre(Id);
ALTER TABLE [dbo].[tblCommodityOwner] ADD CONSTRAINT CommodityOwner_Maritalstatus_FK FOREIGN KEY (MaritalStatusId) REFERENCES tblMaritalStatus(Id);
CREATE TABLE [dbo].[tblCommodityProducer](
	[Id] [uniqueidentifier] NOT NULL Primary Key,	
	[Code] [nvarchar](50) NULL,
	[Name] [nvarchar](50) NOT NULL,	
	[Acrage] [nvarchar](50) NOT NULL,
	[RegNo] [nvarchar](50) NOT NULL,	
	[RegionId] [uniqueidentifier] NOT NULL,
	[PhysicalAddress] [nvarchar](250) NULL,		
	[Description] [nvarchar](250) NULL,	
	[CostCentreId] [uniqueidentifier] NOT NULL,
	[IM_DateCreated] [datetime] NOT NULL,
	[IM_DateLastUpdated] [datetime] NOT NULL,
	[IM_Status] int NOT NULL
	);

ALTER TABLE [dbo].[tblCommodityProducer] ADD CONSTRAINT CommodityProducer_CommodityOwnerSupplier_FK FOREIGN KEY (CostCentreId) REFERENCES tblCostCentre(Id);

ALTER TABLE [dbo].[tblCostCentre] ADD  CostCentreType2 int not null default 0;
ALTER TABLE [dbo].[tblCostCentre] ADD  JoinDate Datetime  null ;

CREATE TABLE [dbo].[tblPurchasingClerkRoute](
	[Id] [uniqueidentifier] NOT NULL primary key,
	[RouteId] [uniqueidentifier] NOT NULL,
	[PurchasingClerkId] [uniqueidentifier] NOT NULL,
	[IM_DateCreated] [datetime] NOT NULL,
	[IM_DateLastUpdated] [datetime] NOT NULL,
	[IM_Status] [int] NOT NULL
 );
 ALTER TABLE [dbo].[tblPurchasingClerkRoute] ADD CONSTRAINT PurchasingClerkRoute_Route_FK FOREIGN KEY (RouteId) REFERENCES tblRoutes(RouteID);
 
 CREATE TABLE [dbo].[tblEquipment](
	[Id] [uniqueidentifier] NOT NULL Primary Key,	
	[Code] [nvarchar](50) NULL,
	[EquipmentNumber] [nvarchar](50) NULL,
	[Name] [nvarchar](50) NOT NULL,
	[LoadCariage] decimal NULL,
	[TareWeight] decimal NULL,
	[Lenght] decimal NULL,
	[Width] decimal NULL,
	[Height] decimal NULL,
	[BubbleSpace] decimal NULL,
	[Volume] decimal NULL,
	[FreezerTemp] decimal NULL,
	[Make] [nvarchar](50) NOT NULL,	
	[Model] [nvarchar](50) NOT NULL,	
	[CommodityGradeId] [uniqueidentifier] NULL,
	[PhysicalAddress] [nvarchar](250) NULL,
	[EquipmentType] int NOT NULL,
	[Description] [nvarchar](250) NULL,
	[CostCentreId] [uniqueidentifier] NOT NULL,
	[IM_DateCreated] [datetime] NOT NULL,
	[IM_DateLastUpdated] [datetime] NOT NULL,
	[IM_Status] int NOT NULL
	);
	
ALTER TABLE [dbo].[tblEquipment] ADD CONSTRAINT Equipment_CostCentre_FK FOREIGN KEY (CostCentreId) REFERENCES tblCostCentre(Id);
ALTER TABLE [dbo].[tblEquipment] ADD CONSTRAINT Equipment_CommodityGrade_FK FOREIGN KEY (CommodityGradeId) REFERENCES tblCommodityGrade(Id);

CREATE TABLE [dbo].[tblSourcingDocument](
	[Id] [uniqueidentifier] NOT NULL Primary Key,
	[DocumentTypeId] [int] NOT NULL,
	[DocumentStatusId] [int]  NOT NULL,
	[DocumentParentId] [uniqueidentifier]  NULL,
	[DocumentReference] [nvarchar](50) NULL,
    [DocumentIssuerCostCentreId] [uniqueidentifier] NOT NULL,
	[DocumentIssuerUserId] [uniqueidentifier] NOT NULL,
	[DocumentRecipientCostCentreId] [uniqueidentifier] NOT NULL,
	[DocumentOnBehalfOfCostCentreId] [uniqueidentifier]  NULL,
	[DocumentIssuerCostCentreApplicationId] [uniqueidentifier]  NULL,
	[DeliveredBy] [nvarchar](50) NULL,
	[Description] [nvarchar](250) NULL,
	[Note] [nvarchar](250) NULL,
	[CommodityOwnerId] [uniqueidentifier] NOT NULL,
	[CommodityProducerId] [uniqueidentifier]  NULL,
	[DateIssued] [datetime]  NOT NULL,
	[DocumentDate] [datetime]  NOT NULL,
	[DateSent] [datetime]  NOT NULL,	
	[IM_DateCreated] [datetime] NOT NULL,
	[IM_DateLastUpdated] [datetime] NOT NULL,
	[IM_Status] int NOT NULL
	);
	
CREATE TABLE [dbo].[tblSourcingLineItem](
	[Id] [uniqueidentifier] NOT NULL Primary Key,
	[ParentId] [uniqueidentifier]  NULL ,
	[DocumentId] [uniqueidentifier] NOT NULL,
	[CommodityId] [uniqueidentifier]  NULL,
	[GradeId] [uniqueidentifier]  NULL,
	[ContainerId] [uniqueidentifier]  NULL,
	[Weight] [decimal]  NULL,
	[ActualWeight] [decimal]  NULL,
	[TareWeight] [decimal]  NULL,
	[Description] [nvarchar](250) NULL,
	[Note] [nvarchar](250) NULL
	);
ALTER TABLE [dbo].[tblSourcingLineItem] ADD CONSTRAINT SourcingLineItem_SourcingDocument_FK FOREIGN KEY (DocumentId) REFERENCES tblSourcingDocument(Id);
	
	
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
alter table [dbo].[tblEquipment] drop column Equipment_CommodityGrade_FK;
alter table [dbo].[tblEquipment] drop column CommodityGradeId;
alter table [dbo].[tblEquipment] drop column ContainerType;
ALTER TABLE [dbo].[tblEquipment] ADD CONSTRAINT Equipment_ContainerType_FK FOREIGN KEY (ContainerTypeId) REFERENCES tblContainerType(Id);
alter table [dbo].[tblEquipment] add  ContainerTypeId uniqueidentifier null;

alter table [dbo].[tblSourcingLineItem] add  ContainerTypeId uniqueidentifier null;