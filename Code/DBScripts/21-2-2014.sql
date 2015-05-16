alter table [dbo].[tblDiscountItem] add IsByQuantity bit  not null default 0;
alter table [dbo].[tblDiscountItem] add Quantity decimal(18,2)  not null default 0;

alter table [dbo].[tblProductDiscountGroupItem] add IsByQuantity bit  not null default 0;
alter table [dbo].[tblProductDiscountGroupItem] add Quantity decimal(18,2)  not null default 0;
