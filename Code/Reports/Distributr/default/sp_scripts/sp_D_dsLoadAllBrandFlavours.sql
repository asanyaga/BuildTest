DROP PROCEDURE [dbo].[sp_D_dsLoadAllBrandFlavours]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_D_dsLoadAllBrandFlavours]
(
@brandId AS NVARCHAR(50),
@productId AS NVARCHAR(50)
)
as
if  @brandId='ALL'  begin set @brandId='%' end
if  @productId='ALL'  begin set @productId='%' end

SELECT TOP (1) ' ALL' AS FlavourId, 
               ' ALL' AS FlavourName
UNION all
SELECT DISTINCT lower(CONVERT(nvarchar(50), flavour.id)) AS FlavourId, 
                                            flavour.name AS FlavourName
FROM       tblProductBrand brand
INNER JOIN tblProduct product ON brand.id = product.BrandId 
INNER JOIN tblProductFlavour flavour ON brand.id = flavour.BrandId
WHERE CONVERT(nvarchar(50),product.id) like ISNULL(@productId,'%')
  and CONVERT(nvarchar(50),brand.id) like ISNULL(@brandId,'%')
  and product.IM_Status = 1 
ORDER BY FlavourName

-- Exec [dbo].[sp_D_dsLoadAllBrandFlavours] @brandId='2e19b27a-9cd0-4c57-a80f-cbd7bbfb10c2',@productId='ac2786d7-e998-47c5-b030-03d99944e002'

GO
