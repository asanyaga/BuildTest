DROP PROCEDURE [dbo].[sp_D_dsPopulateBrands]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[sp_D_dsPopulateBrands]
as 
SELECT  ' ALL' AS BrandId, 
        ' ALL' AS BrandName

UNION ALL
SELECT LOWER(CONVERT(nvarchar(50), id)) AS BrandId, 
                                   name AS BrandName
FROM     tblProductBrand AS pb
--WHERE pb.IM_Status = 1
ORDER BY BrandName

-- Exec [dbo].[sp_D_dsPopulateBrands]

GO
