IF   EXISTS( SELECT * FROM INFORMATION_SCHEMA.COLUMNS  WHERE TABLE_NAME = 'tblDocument' AND  COLUMN_NAME = 'IRNOrderReferences' and DATA_TYPE ='nvarchar' and CHARACTER_MAXIMUM_LENGTH=200)
BEGIN
  ALTER TABLE tblDocument  alter column IRNOrderReferences  varchar(max)  
END

