IF not EXISTS( SELECT * FROM INFORMATION_SCHEMA.COLUMNS  WHERE TABLE_NAME = 'tblProductDiscountGroup' AND  COLUMN_NAME = 'ProductRef')
BEGIN
  ALTER TABLE tblProductDiscountGroup    add  ProductRef uniqueidentifier  null ;
  ALTER TABLE tblProductDiscountGroup    add  DiscountRate decimal(16,4)  null ;
  ALTER TABLE tblProductDiscountGroup    add  Quantity decimal(16,4)  null ;
  ALTER TABLE tblProductDiscountGroup    add  EffectiveDate Datetime  null ;
  ALTER TABLE tblProductDiscountGroup    add  EndDate Datetime  null ;
  ALTER TABLE tblProductDiscountGroup ADD CONSTRAINT ProductDiscountGroup_Product_FK FOREIGN KEY (ProductRef) REFERENCES [tblProduct](Id);
END

