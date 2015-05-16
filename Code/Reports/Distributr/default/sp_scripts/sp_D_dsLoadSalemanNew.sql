DROP PROCEDURE [dbo].[sp_D_dsLoadSalemanNew]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_D_dsLoadSalemanNew]
AS

SELECT TOP(1) 
        'ALL' AS salesmanId,
        'ALL' AS salesmanName, 
            0 AS  CostCentreType

UNION ALL

SELECT    LOWER(CONVERT(nvarchar(50), Id)) AS salesmanId,
                                       Name AS salesmanName, 
                                       CostCentreType
FROM    tblCostCentre
WHERE (CostCentreType = 4)
 AND  tblCostCentre.IM_Status = 1
ORDER BY salesmanName
GO
