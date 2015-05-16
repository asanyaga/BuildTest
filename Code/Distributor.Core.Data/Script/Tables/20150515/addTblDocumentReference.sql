IF NOT EXISTS (SELECT * FROM sys.objects WHERE name LIKE N'tblDocumentRefCounter')
BEGIN
	CREATE TABLE [dbo].[tblDocumentRefCounter](
	[SalesmanCode] [varchar](2) NOT NULL,
	[Counter] [varchar](6) NOT NULL,
	PRIMARY KEY CLUSTERED 
	(
		[SalesmanCode] ASC,
		[Counter] ASC
	)
)
END