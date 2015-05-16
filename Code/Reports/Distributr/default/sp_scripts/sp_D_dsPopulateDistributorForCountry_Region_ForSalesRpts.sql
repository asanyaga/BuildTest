/****** Object:  StoredProcedure [dbo].[sp_D_dsPopulateDistributorForCountry_Region_ForSalesRpts]    Script Date: 07/24/2013 08:51:32 ******/
DROP PROCEDURE [dbo].[sp_D_dsPopulateDistributorForCountry_Region_ForSalesRpts]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_D_dsPopulateDistributorForCountry_Region_ForSalesRpts]
@CountryId AS NVARCHAR(50),
@RegionId AS NVARCHAR(50)
AS
if  ltrim(rtrim(@CountryId))='ALL'  begin set @CountryId='%' end
if  ltrim(rtrim(@RegionId))='ALL'  begin set @RegionId='%' end

SELECT  ' ALL' AS DistributorId,
        ' ALL' AS Distributor

UNION ALL

SELECT    LOWER(CONVERT(nvarchar(50), dist.Id)) AS DistributorId,
                                      dist.Name AS Distributor
FROM        tblCostCentre dist
INNER JOIN  tblRegion reg on dist.Distributor_RegionId = reg.id
WHERE       dist.CostCentreType = 2
AND dist.IM_Status = 1
AND convert(nvarchar(50),reg.id) like ISNULL(@RegionId,'%')
AND convert(nvarchar(50),reg.Country) like ISNULL(@CountryId,'%')
ORDER BY Distributor

--   Exec  [dbo].[sp_D_dsPopulateDistributorForCountry_Region_ForSalesRpts]




GO
