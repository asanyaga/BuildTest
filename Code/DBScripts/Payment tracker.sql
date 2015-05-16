
GO

/****** Object:  Table [dbo].[tblAccount]    Script Date: 06/22/2012 14:14:57 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[tblPaymentTracker](
	[id] [uniqueidentifier] NOT NULL,
	[CostCenterId] [uniqueidentifier] NOT NULL,
	[PaymentModeId] [int] NOT NULL,
	[Balance] [money] NULL default 0,
	[PendingConfirmBalance] [money] NULL default 0,
 CONSTRAINT [PK_tblPaymentTracker] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[tblPaymentTracker]  WITH CHECK ADD  CONSTRAINT [FK_tblPaymentTracker_tblCostCentre] FOREIGN KEY([CostCenterId])
REFERENCES [dbo].[tblCostCentre] ([Id])
GO

ALTER TABLE [dbo].[tblPaymentTracker] CHECK CONSTRAINT [FK_tblPaymentTracker_tblCostCentre]
GO


