DROP PROCEDURE [dbo].[sp_D_dsPopulateProducts_ForBrand_SubBrand]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[sp_D_dsPopulateProducts_ForBrand_SubBrand]
(
@brandId nvarchar(50),
@subBrandId nvarchar(50)
)
as 
if ltrim(rtrim(@brandId))='ALL'  begin set @brandId='%' end
if ltrim(rtrim(@subBrandId))='ALL'  begin set @subBrandId='%' end

SELECT        ' ALL' AS ProductId, 
              ' ALL' AS ProductName
UNION ALL
SELECT        LOWER(CONVERT(NVARCHAR(50), p.id)) AS ProductId, 
                                   p.Description AS ProductName
FROM            tblProduct AS p
WHERE p.IM_Status = 1
AND CONVERT(NVARCHAR(50),p.BrandId) like ISNULL(@brandId,'%')
AND CONVERT(NVARCHAR(50),p.FlavourId) like ISNULL(@subBrandId,'%')
ORDER BY ProductName
GO
