DROP PROCEDURE [dbo].[sp_D_dsLoadAllSubBrandNew]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_D_dsLoadAllSubBrandNew]
AS
SELECT TOP (1) ' ALL' AS subBrandId, 
               ' ALL' AS SubBrand
UNION all
SELECT DISTINCT lower(CONVERT(nvarchar(50), tblProductFlavour_1.id)) AS subBrandId, 
                                            tblProductFlavour_1.name AS SubBrand
FROM            tblProductFlavour AS tblProductFlavour_1
 INNER JOIN     tblProduct AS tblProduct_1 ON tblProductFlavour_1.id = tblProduct_1.FlavourId
WHERE tblProductFlavour_1.IM_Status = 1
ORDER BY SubBrand
GO
