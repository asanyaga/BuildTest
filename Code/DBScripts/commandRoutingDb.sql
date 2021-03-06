
GO
/****** Object:  Table [dbo].[tblCommandRouteMain]    Script Date: 06/25/2011 09:17:16 ******/
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
/****** Object:  Table [dbo].[tblCommandRouteItem]    Script Date: 06/25/2011 09:17:16 ******/
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
/****** Object:  ForeignKey [FK_tblCommandRouteItem_tblCommandRouteMain]    Script Date: 06/25/2011 09:17:16 ******/
ALTER TABLE [dbo].[tblCommandRouteItem]  WITH CHECK ADD  CONSTRAINT [FK_tblCommandRouteItem_tblCommandRouteMain] FOREIGN KEY([CommandRouteMainId])
REFERENCES [dbo].[tblCommandRouteMain] ([id])
GO
ALTER TABLE [dbo].[tblCommandRouteItem] CHECK CONSTRAINT [FK_tblCommandRouteItem_tblCommandRouteMain]
GO
