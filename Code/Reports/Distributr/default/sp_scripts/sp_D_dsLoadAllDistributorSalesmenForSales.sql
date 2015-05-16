DROP PROCEDURE [dbo].[sp_D_dsLoadAllDistributorSalesmenForSales]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_D_dsLoadAllDistributorSalesmenForSales]
(
@distributorId AS NVARCHAR(50)
)
as
if  @distributorId='ALL'  begin set @distributorId='%' end
SELECT  TOP (1) ' ALL' AS SalesmanId, 
                ' ALL' AS SalesmanName
UNION ALL
SELECT        LOWER(CONVERT(nvarchar(50), Id)) AS SalesmanId, 
                                          Name AS SalesmanName
FROM       tblCostCentre AS salesman
WHERE        (CostCentreType = 4)
 and   convert(nvarchar(50),salesman.ParentCostCentreId) like ISNULL(@distributorId,'%')
 and salesman.IM_Status = 1
ORDER BY SalesmanName


--   Exec [dbo].[sp_D_dsLoadAllDistributorSalesmenForSales] @distributorId='167480a6-dc20-44b9-a1b6-0b523914b5ce'

GO