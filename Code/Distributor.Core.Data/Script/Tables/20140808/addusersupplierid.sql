IF NOT EXISTS( SELECT * FROM INFORMATION_SCHEMA.COLUMNS  WHERE TABLE_NAME = 'tblusers' AND  COLUMN_NAME = 'SupplierId')
BEGIN
  alter table tblusers add SupplierId uniqueidentifier 
END