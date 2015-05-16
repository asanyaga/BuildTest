CREATE TABLE [tblActivityType](
	[Id] [uniqueidentifier] NOT NULL primary key,
	[Code] [nvarchar](50) NULL,
	[Name] [nvarchar](250) NOT NULL,
	[Description] [nvarchar](250)  NULL,
	[IsInputsRequired] bit NOT NULL,
	[IsInfectionsRequired] bit NOT NULL,
	[IsServicesRequired] bit NOT NULL,
	[IsProduceRequired] bit NOT NULL,	
	[IM_DateCreated] [datetime] NOT NULL,
	[IM_DateLastUpdated] [datetime] NOT NULL,	
	[IM_Status] [int] NOT NULL
 )
 CREATE TABLE [tblActivityDocument](
	[Id] [uniqueidentifier] NOT NULL primary key,
	[ActivityReference] [nvarchar](250) NULL,
	[ActivityTypeId] [uniqueidentifier] NOT NULL,	
	[Description] [nvarchar](250)  NULL,
	[ActivityDate] [datetime] not null,
	[CommodityProducerId] [uniqueidentifier] not null,
	[SeasonId] [uniqueidentifier] not null,
	[ClerkId] [uniqueidentifier] not null,
	[hubId] [uniqueidentifier] not null,
	[RouteId] [uniqueidentifier] not null,
	[CentreId] [uniqueidentifier] not null,
	[CommoditySupplierId] [uniqueidentifier] not null,
	[IM_DateCreated] [datetime] NOT NULL,
	[IM_DateLastUpdated] [datetime] NOT NULL,	
	[IM_Status] [int] NOT NULL
 )
 ALTER TABLE [dbo].[tblActivityDocument] ADD CONSTRAINT ActivityDocument_ActivityType_FK FOREIGN KEY (ActivityTypeId) REFERENCES [tblActivityType](Id);

 CREATE TABLE [tblActivityInputLineItem](
	[Id] [uniqueidentifier] NOT NULL primary key,	
	[ProductId] [uniqueidentifier] NOT NULL,	
	[Description] [nvarchar](250)  NULL,
	[Quantity] decimal(18,2) not null,
	[MF_Date] [datetime]  NULL,
	[EP_Date] [datetime]  NULL,
	[ActivityId] [uniqueidentifier] not null,	
	[IM_DateCreated] [datetime] NOT NULL,
	[IM_DateLastUpdated] [datetime] NOT NULL,	
	[IM_Status] [int] NOT NULL
 )
  ALTER TABLE [dbo].[tblActivityInputLineItem] ADD CONSTRAINT ActivityInputLineItem_ActivityDocument_FK FOREIGN KEY ([ActivityId]) REFERENCES [tblActivityDocument](Id);

 CREATE TABLE [tblActivityServiceLineItem](
	[Id] [uniqueidentifier] NOT NULL primary key,	
	[ServiceProviderId] [uniqueidentifier] NOT NULL,	
	[ServiceId] [uniqueidentifier] NOT NULL,	
	[ShiftId] [uniqueidentifier] NOT NULL,	
	[Description] [nvarchar](250)  NULL,
	[ActivityId] [uniqueidentifier] not null,	
	[IM_DateCreated] [datetime] NOT NULL,
	[IM_DateLastUpdated] [datetime] NOT NULL,	
	[IM_Status] [int] NOT NULL
 )
 ALTER TABLE [dbo].[tblActivityServiceLineItem] ADD CONSTRAINT ActivityServiceLineItem_ActivityDocument_FK FOREIGN KEY ([ActivityId]) REFERENCES [tblActivityDocument](Id);

 CREATE TABLE [tblActivityInfectionLineItem](
	[Id] [uniqueidentifier] NOT NULL primary key,	
	[InfectionId] [uniqueidentifier] NOT NULL,
	[InfectionRate] decimal(4,2) NOT NULL,				
	[Description] [nvarchar](250)  NULL,
	[ActivityId] [uniqueidentifier] not null,	
	[IM_DateCreated] [datetime] NOT NULL,
	[IM_DateLastUpdated] [datetime] NOT NULL,	
	[IM_Status] [int] NOT NULL
 )
 ALTER TABLE [dbo].[tblActivityInfectionLineItem] ADD CONSTRAINT ActivityInfectionLineItem_ActivityDocument_FK FOREIGN KEY ([ActivityId]) REFERENCES [tblActivityDocument](Id);

 CREATE TABLE [tblActivityProduceLineItem](
	[Id] [uniqueidentifier] NOT NULL primary key,	
	[CommodityId] [uniqueidentifier] NOT NULL,
    [GradeId] [uniqueidentifier] NOT NULL,
	[Weight] decimal(18,2) NOT NULL,				
	[Description] [nvarchar](250)  NULL,
	[ActivityId] [uniqueidentifier] not null,
	[ServiceProviderId] [uniqueidentifier] NOT NULL,	
	[IM_DateCreated] [datetime] NOT NULL,
	[IM_DateLastUpdated] [datetime] NOT NULL,	
	[IM_Status] [int] NOT NULL
 )
 ALTER TABLE [dbo].[tblActivityProduceLineItem] ADD CONSTRAINT ActivityProduceLineItem_ActivityDocument_FK FOREIGN KEY ([ActivityId]) REFERENCES [tblActivityDocument](Id);
