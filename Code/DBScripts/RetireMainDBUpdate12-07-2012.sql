alter table tblDocument add DocumentParentId [uniqueidentifier];
alter table tblDocument add SendDateTime Datetime;

Create table  tblRetireDocumentSetting(
    [Id] [uniqueidentifier] Primary Key NOT NULL,
	[RetireTypeId] [int] NOT NULL,
	[Duration] [int] NOT NULL,
	[IM_DateCreated] [datetime] NOT NULL,
	[IM_DateLastUpdated] [datetime] NOT NULL,
	[IM_IsActive] [bit] NOT NULL,
);

Alter table dbo.tblDocument Alter column [DocumentDateIssued] datetime
