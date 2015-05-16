CREATE TABLE [dbo].[tblServiceProvider]
(
	[id] [uniqueidentifier] NOT NULL,
	[Code] [nvarchar](50) NULL,
	[Name] [nvarchar](250) NOT NULL,
	[PIN] [nvarchar](50)  NULL,
	[IDNo] [nvarchar](50) NOT NULL,
	[Mobile] [nvarchar](50) NOT NULL,
	[BankId] [uniqueidentifier] NULL,
	[BankBranchId] [uniqueidentifier] NULL,
	[AccountName] [nvarchar](250)  NULL,
	[AccountNumber] [nvarchar](250)  NULL,
	[Gender] [int]  NULL,
	[Description] [nvarchar](450) NULL,
	[IM_DateCreated] [datetime] NOT NULL,	
	[IM_DateLastUpdated] [datetime] NOT NULL,
	[IM_Status] [int] NOT NULL,
	 CONSTRAINT [PK_tblServiceProvider] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]


CREATE TABLE [dbo].[tblService]
(
	[id] [uniqueidentifier] NOT NULL,
	[Code] [nvarchar](50) NULL,
	[Name] [nvarchar](250) NOT NULL,
	[Cost] [decimal](18,2)  NULL,	
	[Description] [nvarchar](450) NULL,
	[IM_DateCreated] [datetime] NOT NULL,	
	[IM_DateLastUpdated] [datetime] NOT NULL,
	[IM_Status] [int] NOT NULL,
	 CONSTRAINT [PK_tblService] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

CREATE TABLE [dbo].[tblShift]
(
	[id] [uniqueidentifier] NOT NULL,
	[Code] [nvarchar](50) NULL,
	[Name] [nvarchar](250) NOT NULL,
	[StartTime] [datetime] NOT NULL,
	[EndTime] [datetime] NOT NULL,	
	[Description] [nvarchar](450) NULL,
	[IM_DateCreated] [datetime] NOT NULL,	
	[IM_DateLastUpdated] [datetime] NOT NULL,
	[IM_Status] [int] NOT NULL,
	 CONSTRAINT [PK_tblShift] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]


CREATE TABLE [dbo].[tblSeason]
(
	[id] [uniqueidentifier] NOT NULL,
	[CommodityProducerId] [uniqueidentifier] NOT NULL,
	[Code] [nvarchar](50) NULL,
	[Name] [nvarchar](250) NOT NULL,
	[StartDate] [datetime] NOT NULL,
	[EndDate] [datetime] NOT NULL,
	[Description] [nvarchar](450) NULL,
	[IM_DateCreated] [datetime] NOT NULL,	
	[IM_DateLastUpdated] [datetime] NOT NULL,
	[IM_Status] [int] NOT NULL,
	 CONSTRAINT [PK_tblSeason] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]


CREATE TABLE [dbo].[tblInfection]
(
	[id] [uniqueidentifier] NOT NULL,
	[Code] [nvarchar](50) NULL,
	[Name] [nvarchar](250) NOT NULL,
	[Type] [int] NOT NULL,
	[Description] [nvarchar](450) NULL,
	[IM_DateCreated] [datetime] NOT NULL,	
	[IM_DateLastUpdated] [datetime] NOT NULL,
	[IM_Status] [int] NOT NULL,
	 CONSTRAINT [PK_tblInfection] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]