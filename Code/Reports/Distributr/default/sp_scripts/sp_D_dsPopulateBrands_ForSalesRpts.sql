DROP PROCEDURE [dbo].[sp_D_dsPopulateBrands_ForSalesRpts]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[sp_D_dsPopulateBrands_ForSalesRpts]
as 
SELECT  ' ALL' AS BrandId, 
        ' ALL' AS BrandName

UNION 
SELECT lower(CONVERT(nvarchar(50), id)) AS BrandId, 
                                   name AS BrandName
FROM     tblProductBrand AS pb
WHERE pb.IM_Status = 1
ORDER BY BrandName
GO
