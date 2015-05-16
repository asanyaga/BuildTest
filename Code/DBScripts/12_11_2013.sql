
CREATE TABLE [dbo].[tblCostCentreMapping](
	[Id] [uniqueidentifier] NOT NULL primary key,
	[MapToCostCentreId] [uniqueidentifier] NOT NULL,	
	[IM_DateCreated] [datetime] NOT NULL,
	[IM_DateLastUpdated] [datetime] NOT NULL,
	[IM_Status] [int] NOT NULL
	);
ALTER TABLE [dbo].[tblCostCentreMapping] ADD CONSTRAINT Mapping_CostCentre_FK FOREIGN KEY (Id) REFERENCES tblCostCentre(Id);