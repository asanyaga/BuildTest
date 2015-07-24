IF NOT EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'sp_A_Location')
   exec('CREATE PROCEDURE [sp_A_Location] AS BEGIN SET NOCOUNT ON; END')
GO
Alter PROCEDURE sp_A_Location
	 @CountryId NVARCHAR(50)    
AS
BEGIN
if  @CountryId='ALL'  begin set @CountryId='%' end

	SELECT  'ALL' AS LocationId, 'ALL' AS Location
	UNION
	SELECT LOWER(convert(varchar(50),tblRegion.id)) as LocationId,tblRegion.Name AS Location 
	FROM tblRegion INNER JOIN
	tblCountry ON tblRegion.Country = tblCountry.id	
	WHERE (CONVERT(NVARCHAR(50),tblCountry.id) LIKE ISNULL(@CountryId, N'%'))   
	ORDER BY Location
	END;

-- EXEC sp_A_Location @CountryId='ALL'

