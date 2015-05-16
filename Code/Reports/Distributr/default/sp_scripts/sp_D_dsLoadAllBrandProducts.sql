DROP PROCEDURE [dbo].[sp_D_dsLoadAllBrandProducts]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_D_dsLoadAllBrandProducts]
(
@brandId AS NVARCHAR(50),
@flavourId AS NVARCHAR(50)
)
as
if  @brandId='ALL'  begin set @brandId='%' end
if  @flavourId='ALL'  begin set @flavourId='%' end

SELECT TOP (1) ' ALL' AS ProductId, 
               ' ALL' AS ProductName
UNION all
SELECT DISTINCT lower(CONVERT(nvarchar(50), product.id)) AS ProductId, 
                                            product.Description AS ProductName
FROM       tblProductBrand brand
INNER JOIN tblProduct product ON brand.id = product.BrandId 
INNER JOIN tblProductFlavour flavour ON brand.id = flavour.BrandId
WHERE CONVERT(nvarchar(50),flavour.id) like ISNULL(@flavourId,'%')
  and CONVERT(nvarchar(50),brand.id) like ISNULL(@brandId,'%')
  and product.IM_Status = 1
ORDER BY ProductName

-- Exec [dbo].[sp_D_dsLoadAllBrandProducts] @brandId='ALL',@flavourId='ALL'

GO
