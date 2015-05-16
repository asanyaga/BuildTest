IF   EXISTS( SELECT * FROM INFORMATION_SCHEMA.COLUMNS  WHERE TABLE_NAME = 'tblOutletVisitReasonType' AND  COLUMN_NAME = 'Action' and DATA_TYPE ='bit')
BEGIN
   DECLARE @TableName AS NVARCHAR(255),
     @ColumnName AS NVARCHAR(255),
     @ConstraintName AS NVARCHAR(255),
     @DropConstraintSQL AS NVARCHAR(255)

SET @TableName = 'tblOutletVisitReasonType'
SET @ColumnName = 'Action'


SET @ConstraintName =
     (SELECT TOP 1 o.name FROM sysobjects o
     JOIN syscolumns c
     ON o.id = c.cdefault
     JOIN sysobjects t
     ON c.id = t.id
     WHERE o.xtype = 'd'
     AND c.name = @ColumnName
     AND t.name = @TableName)


SET @DropConstraintSQL = 'ALTER TABLE ' + @TableName + ' DROP ' + @ConstraintName


EXEC (@DropConstraintSQL)
  ALTER TABLE tblOutletVisitReasonType  alter column [Action]  int  
END

