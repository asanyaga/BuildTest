
--rename column below
sp_RENAME 'tblLineItems.[Receipt_InvoiceId]' , 'PaymentDocLineItemId', 'COLUMN'

--add column to tblDocument
begin tran 
if not exists (select * from information_schema.columns where table_name = 'tblDocument' and column_name = 'PaymentDocId')
	alter table tblDocument
		add PaymentDocId [uniqueidentifier] NULL
commit

if not exists (select * from information_schema.columns where table_name = 'tblLineItems' and column_name = 'NotificationId')
	alter table tblLineItems
		add NotificationId nvarchar(1000)