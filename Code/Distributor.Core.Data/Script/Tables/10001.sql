IF NOT EXISTS( SELECT * FROM INFORMATION_SCHEMA.COLUMNS  WHERE TABLE_NAME = 'tblOutletVisitReasonType' AND  COLUMN_NAME = 'Action')
BEGIN
  ALTER TABLE tblOutletVisitReasonType    ADD [Action] bit not NULL default 0
END