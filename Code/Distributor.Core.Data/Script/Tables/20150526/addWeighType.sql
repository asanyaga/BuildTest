IF NOT EXISTS( SELECT * FROM INFORMATION_SCHEMA.COLUMNS  WHERE TABLE_NAME = 'tblSourcingLineItem' AND  COLUMN_NAME = 'WeighType')
BEGIN 
  ALTER TABLE tblSourcingLineItem ADD [WeighType] int default Null
END
