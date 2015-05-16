IF not exiSTS( SELECT * FROM INFORMATION_SCHEMA.COLUMNS  WHERE TABLE_NAME = 'tblInventoryImports' AND  COLUMN_NAME = 'Hub')
BEGIN
  ALTER TABLE tblInventoryImports  add  Hub uniqueidentifier null ;
END
IF not exiSTS( SELECT * FROM INFORMATION_SCHEMA.COLUMNS  WHERE TABLE_NAME = 'tblInventoryImports' AND  COLUMN_NAME = 'SalesmanId')
BEGIN
  ALTER TABLE tblInventoryImports  add  SalesmanId uniqueidentifier null ;
END

IF not exiSTS( SELECT * FROM INFORMATION_SCHEMA.COLUMNS  WHERE TABLE_NAME = 'tblInventoryImports' AND  COLUMN_NAME = 'ProductId')
BEGIN
  ALTER TABLE tblInventoryImports  add  ProductId uniqueidentifier null ;
END
