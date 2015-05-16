IF NOT EXISTS( SELECT * FROM INFORMATION_SCHEMA.COLUMNS  WHERE TABLE_NAME = 'tblLineItems' AND  COLUMN_NAME = 'InitialQuantity')
BEGIN
  ALTER TABLE tblLineItems    ADD [InitialQuantity] decimal(18,2) not NULL default 0
END