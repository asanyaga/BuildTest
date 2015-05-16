IF  EXISTS( SELECT * FROM INFORMATION_SCHEMA.COLUMNS  WHERE TABLE_NAME = 'tblInventoryImports' AND  COLUMN_NAME = 'IsProcessed')
BEGIN
  ALTER TABLE tblInventoryImports    drop column IsProcessed ;
END

IF not EXISTS( SELECT * FROM INFORMATION_SCHEMA.COLUMNS  WHERE TABLE_NAME = 'tblInventoryImports' AND  COLUMN_NAME = 'ImportStatus')
BEGIN
  ALTER TABLE tblInventoryImports    add  ImportStatus int not null ;
END