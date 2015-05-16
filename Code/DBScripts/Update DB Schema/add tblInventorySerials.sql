USE [cokeunittests]
GO

/****** Object:  Table [dbo].[tblInventorySerials]    Script Date: 08/28/2012 10:11:00 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[tblInventorySerials](
	[Id] [uniqueidentifier] NOT NULL,
	[CostCentreId] [uniqueidentifier] NOT NULL,
	[ProductId] [uniqueidentifier] NOT NULL,
	[DocumentId] [uniqueidentifier] NOT NULL,
	[From] [nvarchar](150) NOT NULL,
	[To] [nvarchar](150) NOT NULL,
	[IM_DateCreated] [datetime] NOT NULL,
	[IM_DateLastUpdated] [datetime] NOT NULL,
	[IM_Status] [int] NOT NULL,
 CONSTRAINT [PK_tblInventorySerials] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[tblInventorySerials]  WITH CHECK ADD  CONSTRAINT [FK_tblInventorySerials_tblCostCentre] FOREIGN KEY([CostCentreId])
REFERENCES [dbo].[tblCostCentre] ([Id])
GO

ALTER TABLE [dbo].[tblInventorySerials] CHECK CONSTRAINT [FK_tblInventorySerials_tblCostCentre]
GO

ALTER TABLE [dbo].[tblInventorySerials]  WITH CHECK ADD  CONSTRAINT [FK_tblInventorySerials_tblDocument] FOREIGN KEY([DocumentId])
REFERENCES [dbo].[tblDocument] ([Id])
GO

ALTER TABLE [dbo].[tblInventorySerials] CHECK CONSTRAINT [FK_tblInventorySerials_tblDocument]
GO

ALTER TABLE [dbo].[tblInventorySerials]  WITH CHECK ADD  CONSTRAINT [FK_tblInventorySerials_tblProduct] FOREIGN KEY([ProductId])
REFERENCES [dbo].[tblProduct] ([id])
GO

ALTER TABLE [dbo].[tblInventorySerials] CHECK CONSTRAINT [FK_tblInventorySerials_tblProduct]
GO


