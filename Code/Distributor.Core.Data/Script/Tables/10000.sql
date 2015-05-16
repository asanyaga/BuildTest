IF (NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE   TABLE_NAME = 'tblOutletVisitReasonType'))
	BEGIN
		CREATE TABLE [tblOutletVisitReasonType](
		[id] [uniqueidentifier] NOT NULL primary Key ,
		[Name] [varchar](50) NULL,
		[Description] [varchar](50) NULL,
		[IM_DateCreated] [datetime] NOT NULL,
		[IM_DateLastUpdated] [datetime] NOT NULL,
		[IM_Status] [int] NOT NULL,
		)
 ; END




