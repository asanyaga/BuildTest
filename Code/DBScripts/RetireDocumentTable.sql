Create table  tblRetireDocumentSetting(
    [Id] [uniqueidentifier] Primary Key NOT NULL,
	[RetireTypeId] [int] NOT NULL,
	[Duration] [int] NOT NULL,
	[IM_DateCreated] [datetime] NOT NULL,
	[IM_DateLastUpdated] [datetime] NOT NULL,
	[IM_IsActive] [bit] NOT NULL,
);