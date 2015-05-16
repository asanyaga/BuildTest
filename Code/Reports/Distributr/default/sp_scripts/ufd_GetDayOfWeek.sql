
DROP FUNCTION dbo.ufd_GetDayOfWeek
GO
CREATE  FUNCTION dbo.ufd_GetDayOfWeek(@day int)
RETURNS VARCHAR(100)
AS
BEGIN
DECLARE @dayofweek nvarchar(100)
SELECT @dayofweek = CASE WHEN @day = 0 THEN 'SUNDAY'  
						 WHEN @day = 1 THEN 'MONDAY' 
						 WHEN @day = 2 THEN 'TUESDAY' 
						 WHEN @day = 3 THEN 'WEDNESDAY' 
						 WHEN @day = 4 THEN 'THURSDAY' 
						 WHEN @day = 5 THEN 'FRIDAY' 
						 WHEN @day = 6 THEN 'SATURDAY' 
						 ELSE 'UNKNOWN DAY' END 
RETURN @dayofweek;
END;
GO


-- SELECT dbo.ufd_GetDayOfWeek(0) AS DAYOFWEEK