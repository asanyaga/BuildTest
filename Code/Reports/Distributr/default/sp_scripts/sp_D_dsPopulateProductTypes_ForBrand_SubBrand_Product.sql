DROP PROCEDURE [dbo].[sp_D_dsPopulateProductTypes_ForBrand_SubBrand_Product]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[sp_D_dsPopulateProductTypes_ForBrand_SubBrand_Product]

(
@brandId nvarchar(50),
@subBrandId nvarchar(50),
@productId nvarchar(50)

)
as 
if ltrim(rtrim(@brandId))='ALL'  begin set @brandId='%' end
if ltrim(rtrim(@subBrandId))='ALL'  begin set @subBrandId='%' end
if ltrim(rtrim(@productId))='ALL'  begin set @productId='%' end

SELECT    ' ALL'  AS ProductTypeId,
          ' ALL'  AS ProductType
UNION ALL
SELECT DISTINCT   LOWER(CONVERT(nvarchar(50),pt.id)) AS ProductTypeId,
                                             pt.name AS ProductType 
FROM      dbo.tblProductType pt
RIGHT OUTER JOIN  dbo.tblProduct p on pt.id = p.ProductTypeId
WHERE pt.IM_Status = 1
AND  CONVERT(NVARCHAR(50),p.BrandId) LIKE ISNULL(@brandId,'%')
AND  CONVERT(NVARCHAR(50),p.FlavourId) LIKE ISNULL(@subBrandId,'%')
AND  CONVERT(NVARCHAR(50),p.id) LIKE ISNULL(@productId,'%')
ORDER BY ProductType

