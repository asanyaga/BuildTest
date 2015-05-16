IF OBJECT_ID (N'dbo.udf_D_RoundOff', N'FN') IS NOT NULL
DROP FUNCTION  dbo.udf_D_RoundOff;
GO
CREATE FUNCTION dbo.udf_D_RoundOff(@NetValue DECIMAL(18,4))
RETURNS INT 
AS 
BEGIN
    DECLARE @value INT;
	SELECT @value = CASE WHEN (CONVERT(DECIMAL(18,2),ROUND(@NetValue,2,1))%1) >= 0.04 THEN CEILING(@NetValue) ELSE FLOOR(@NetValue) END 
    RETURN @value;
END;
GO


-- SELECT dbo.udf_D_RoundOff(11.0392)