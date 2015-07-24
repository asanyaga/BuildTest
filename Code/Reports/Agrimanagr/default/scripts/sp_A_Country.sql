IF NOT EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'sp_A_Country')
   exec('CREATE PROCEDURE [sp_A_Country] AS BEGIN SET NOCOUNT ON; END')
GO
Alter PROCEDURE sp_A_Country 
AS
BEGIN
	SELECT  'ALL' AS CountryId, 'ALL' AS Name
	UNION
	SELECT LOWER(convert(varchar(50),id)) as CountryId,Name 
	from tblCountry 
	order by Name
	END;

-- EXEC sp_A_Country

