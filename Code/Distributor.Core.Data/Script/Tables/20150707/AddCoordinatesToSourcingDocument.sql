IF NOT EXISTS( SELECT * FROM INFORMATION_SCHEMA.COLUMNS  WHERE TABLE_NAME = 'tblSourcingDocument' AND  COLUMN_NAME = 'Latitude')
BEGIN 
  ALTER TABLE tblSourcingDocument ADD [Latitude] Float default Null
END

IF NOT EXISTS( SELECT * FROM INFORMATION_SCHEMA.COLUMNS  WHERE TABLE_NAME = 'tblSourcingDocument' AND  COLUMN_NAME = 'Longitude')
BEGIN 
  ALTER TABLE tblSourcingDocument ADD [Longitude] Float default Null
END
