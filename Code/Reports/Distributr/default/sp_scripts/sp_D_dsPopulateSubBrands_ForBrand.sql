/****** Object:  StoredProcedure [dbo].[sp_D_dsPopulateSubBrands_ForBrand]    Script Date: 07/24/2013 08:51:32 ******/
DROP PROCEDURE [dbo].[sp_D_dsPopulateSubBrands_ForBrand]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_D_dsPopulateSubBrands_ForBrand]
(
@brandId nvarchar(50)
)
AS
if ltrim(rtrim(@brandId))='ALL'  begin set @brandId='%' end

SELECT TOP (1) ' ALL' AS SubBrandId, 
               ' ALL' AS SubBrand
UNION all
SELECT DISTINCT CONVERT(nvarchar(50), sb.id) AS SubBrandId, 
                                      sb.name AS SubBrand
FROM            tblProductFlavour AS sb 
INNER JOIN  tblProduct AS p ON sb.id = p.FlavourId
WHERE sb.IM_Status = 1
AND  CONVERT(NVARCHAR(50),p.BrandId) LIKE ISNULL(@brandId,'%')
ORDER BY SubBrand

-- exec sp_D_dsPopulateSubBrands_ForBrand @brandId='69E51F40-3085-462C-A7E0-758ED8698E03'


GO
