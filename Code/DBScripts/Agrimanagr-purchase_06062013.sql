alter table [dbo].[tblSourcingLineItem] drop column ActualWeight;
alter table [dbo].[tblSourcingLineItem] add  NoOfContainer decimal(16,2);