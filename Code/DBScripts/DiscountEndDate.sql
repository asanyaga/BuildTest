alter table [dbo].[tblCertainValueCertainProductDiscountItem] add EndDate  datetime  null;
alter table [dbo].[tblCustomerDiscountItem] add EndDate  datetime  null;
alter table [dbo].[tblDiscountItem] add EndDate  datetime  null;
alter table [dbo].[tblFreeOfChargeDiscount] add StartDate  datetime  null;
alter table [dbo].[tblFreeOfChargeDiscount] add EndDate  datetime  null;
alter table [dbo].[tblProductDiscountGroupItem] add EndDate  datetime  null;
alter table [dbo].[tblPromotionDiscountItem] add EndDate  datetime  null;
alter table [dbo].[tblSaleValueDiscountItems] add EndDate  datetime  null;