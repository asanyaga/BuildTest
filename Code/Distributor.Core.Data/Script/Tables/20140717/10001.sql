﻿IF NOT EXISTS( SELECT * FROM INFORMATION_SCHEMA.COLUMNS  WHERE TABLE_NAME = 'tblSourcingLineItem' AND  COLUMN_NAME = 'FinalWeight')
BEGIN
  ALTER TABLE tblSourcingLineItem    ADD [FinalWeight] decimal(18,2)  NULL;
END
