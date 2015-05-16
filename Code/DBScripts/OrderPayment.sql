CREATE TABLE [dbo].[tblOrderPaymentInfo](
	[Id] [uniqueidentifier] NOT NULL Primary Key,	
	[DocumentId] [uniqueidentifier] NOT NULL,
	[Amount] Decimal(18,2)  not NULL ,
	[ConfirmedAmount] Decimal(18,2)  not NULL ,
	[PaymentMode] int  not NULL,
	[MMoneyPaymentType] varchar(50)  NULL,
	[PaymentRefId] varchar(50) NULL,
	[IsConfirmed]  bit  NULL,	
	[NotificationId] [nvarchar](50) NULL,
	[Description] [nvarchar](250) NULL,
	[TransactionDate] datetime NULL,
	[IsProcessed]  bit  NULL
	);
ALTER TABLE [dbo].[tblOrderPaymentInfo] ADD CONSTRAINT OrderPayment_Order_FK FOREIGN KEY (DocumentId) REFERENCES tblDocument(Id);