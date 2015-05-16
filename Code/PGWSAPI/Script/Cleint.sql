CREATE TABLE [dbo].[tblClient](
	[Id] int NOT NULL IDENTITY(1,1) primary key ,
	[ClientName] varchar(50) NOT NULL,
    [ClientKeyWord] varchar(50) NOT NULL,	
	[ClientUri] varchar(250) NOT NULL,
	[ApplicationId] varchar(50)  NULL,
	[ApplicationPassword] varchar(50)  NULL,
	[ExternalId] varchar(50) NOT NULL,
	[IM_DateCreated] [datetime] NOT NULL,
	[IM_DateLastUpdated] [datetime] NOT NULL,
	[IM_Status] [int] NOT NULL
	);
	
CREATE TABLE [dbo].[tblClientMember](
	[Id] int NOT NULL IDENTITY(1,1) primary key,
	[Name] varchar(50) NOT NULL,
    [Code] varchar(50) NOT NULL,	
	[MemberType]  int NOT NULL,
	[ClientId] [int] NOT NULL,
	[ExternalId] varchar(50) NOT NULL,
	[IM_DateCreated] [datetime] NOT NULL,
	[IM_DateLastUpdated] [datetime] NOT NULL,
	[IM_Status] [int] NOT NULL
	);
ALTER TABLE [dbo].[tblClientMember] ADD CONSTRAINT ClientMember_Client_FK FOREIGN KEY (ClientId) REFERENCES tblClient(Id);