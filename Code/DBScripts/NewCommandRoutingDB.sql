GO
/****** Object:  Table [dbo].[tblDistributrCommand]    Script Date: 08/31/2011 01:30:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblDistributrCommand](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[CommandId] [uniqueidentifier] NOT NULL,
	[DocumentId] [uniqueidentifier] NOT NULL,
	[DateCommandInserted] [datetime] NOT NULL,
	[CommandGeneratedByCostCentreApplicationId] [int] NOT NULL,
	[CommandGeneratedByUserId] [int] NOT NULL,
	[CommandType] [nvarchar](50) NOT NULL,
	[JsonCommand] [nvarchar](2000) NOT NULL,
 CONSTRAINT [PK__tblDistr__3214EC071273C1CD] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblCommandRouteMain]    Script Date: 08/31/2011 01:30:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblCommandRouteMain](
	[id] [bigint] IDENTITY(1,1) NOT NULL,
	[CommandId] [uniqueidentifier] NOT NULL,
	[DocumentId] [uniqueidentifier] NOT NULL,
	[DateCommandInserted] [datetime] NOT NULL,
	[CommandGeneratedByCostCentreApplicationId] [int] NOT NULL,
	[CommandGeneratedByUserId] [int] NOT NULL,
	[CostCentreApplicationCommandSequenceId] [int] NOT NULL,
	[CommandType] [nvarchar](70) NOT NULL,
	[JsonCommand] [nvarchar](2000) NOT NULL,
 CONSTRAINT [PK_tblCommandRouteMain] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblRoutingStatus]    Script Date: 08/31/2011 01:30:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblRoutingStatus](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[DistributorCommandId] [bigint] NOT NULL,
	[DestinationCostCentreApplicationId] [int] NOT NULL,
	[Delivered] [bit] NOT NULL,
	[DateDelivered] [datetime] NULL,
	[Executed] [bit] NOT NULL,
	[DateExecuted] [datetime] NULL,
 CONSTRAINT [PK__tblRouti__3214EC071B0907CE] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblRoutingCentre]    Script Date: 08/31/2011 01:30:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblRoutingCentre](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[DistributorCommandId] [bigint] NOT NULL,
	[RoutingCostCentreId] [int] NOT NULL,
 CONSTRAINT [PK__tblRouti__3214EC07164452B1] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblCommandRouteItem]    Script Date: 08/31/2011 01:30:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblCommandRouteItem](
	[id] [bigint] IDENTITY(1,1) NOT NULL,
	[CommandRouteMainId] [bigint] NOT NULL,
	[CommandDestinationCostCentreApplicationId] [int] NOT NULL,
	[CommandDelivered] [bit] NOT NULL,
	[DateCommandDelivered] [datetime] NULL,
	[CommandExecuted] [bit] NOT NULL,
	[DateCommandExecuted] [datetime] NULL,
 CONSTRAINT [PK_tblCommandRouteItem] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  ForeignKey [FK_tblCommandRouteItem_tblCommandRouteMain]    Script Date: 08/31/2011 01:30:32 ******/
ALTER TABLE [dbo].[tblCommandRouteItem]  WITH CHECK ADD  CONSTRAINT [FK_tblCommandRouteItem_tblCommandRouteMain] FOREIGN KEY([CommandRouteMainId])
REFERENCES [dbo].[tblCommandRouteMain] ([id])
GO
ALTER TABLE [dbo].[tblCommandRouteItem] CHECK CONSTRAINT [FK_tblCommandRouteItem_tblCommandRouteMain]
GO
/****** Object:  ForeignKey [FK__tblRoutin__Distr__07F6335A]    Script Date: 08/31/2011 01:30:32 ******/
ALTER TABLE [dbo].[tblRoutingCentre]  WITH CHECK ADD FOREIGN KEY([DistributorCommandId])
REFERENCES [dbo].[tblDistributrCommand] ([Id])
GO
/****** Object:  ForeignKey [FK__tblRoutin__Distr__07020F21]    Script Date: 08/31/2011 01:30:32 ******/
ALTER TABLE [dbo].[tblRoutingStatus]  WITH CHECK ADD FOREIGN KEY([DistributorCommandId])
REFERENCES [dbo].[tblDistributrCommand] ([Id])
GO
