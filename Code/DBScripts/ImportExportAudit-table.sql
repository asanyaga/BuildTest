
/****** Object:  Table [dbo].[tblExportImportAudit]    Script Date: 05/15/2013 17:20:33 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[tblExportImportAudit](
	[DocumentId] [uniqueidentifier] NOT NULL,
	[IntegrationModule] [int] NOT NULL,
	[DocumentAuditStatus] [int] NOT NULL,
	[DocumentReference] [nvarchar](50) NULL,
	[ExternalDocumentReference] [nvarchar](50) NULL,
	[DocumentType] [int] NULL,
	[DateUploaded] [datetime] NULL,
 CONSTRAINT [PK_tblExportImportAudit] PRIMARY KEY CLUSTERED 
(
	[DocumentId] ASC,
	[IntegrationModule] ASC,
	[DocumentAuditStatus] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


