DECLARE @Counter AS INT

SELECT @Counter =  MAX(CAST(Counter AS INT)) 
	FROM tblDocumentRefCounter 
	WHERE SalesmanCode = RIGHT('00' + '{0}', 2)

IF (SELECT @Counter) IS NOT NULL
BEGIN
	SET @Counter = @Counter + 1

	INSERT INTO tblDocumentRefCounter(SalesmanCode,Counter)
	VALUES(RIGHT('00' + '{0}', 2),RIGHT('000000' + CAST(@Counter AS VARCHAR(6)), 6))
END
ELSE
BEGIN
	SET @Counter = 1
	INSERT INTO tblDocumentRefCounter(SalesmanCode,Counter)
	VALUES(RIGHT('00' + '{0}', 2),RIGHT('000000' + CAST(@Counter AS VARCHAR(6)), 6))
END
SELECT '{0}' +RIGHT('000000' + CAST(@Counter AS VARCHAR(6)), 6)