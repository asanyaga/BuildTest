DROP PROCEDURE [dbo].[sp_D_dsLoadAllDistributorSalesmen]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_D_dsLoadAllDistributorSalesmen]
AS

SELECT  TOP (1) 'ALL' AS SalesmanId, 
                'ALL' AS Salesman
UNION ALL
SELECT        LOWER(CONVERT(nvarchar(50), Id)) AS SalesmanId, 
                                          Name AS Salesman
FROM       tblCostCentre AS tblCostCentre_1
WHERE        (CostCentreType = 4) and tblCostCentre_1.IM_Status = 1
ORDER BY Salesman
GO