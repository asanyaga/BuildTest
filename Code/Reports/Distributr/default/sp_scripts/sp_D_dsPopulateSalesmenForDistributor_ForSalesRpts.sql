DROP PROCEDURE [dbo].[sp_D_dsPopulateSalesmenForDistributor_ForSalesRpts]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[sp_D_dsPopulateSalesmenForDistributor_ForSalesRpts]
( 
 @distributorID as nvarchar(50) 
 )
as
if  LTRIM(RTRIM(@distributorID))='ALL'  begin set @distributorID='%' end
SELECT  ' ALL' AS SalesmanId, 
        ' ALL' AS Salesman, 
             0 AS CostCentreType
UNION ALL
SELECT LOWER(CONVERT(nvarchar(50), Id)) AS SalesmanId, 
                                   Name AS Salesman,
                                      CostCentreType
FROM       tblCostCentre AS salesman
WHERE  (CostCentreType = 4)
   AND (convert(varchar(50),salesman.ParentCostCentreId) like ISNULL(@distributorID,'%'))
   AND salesman.IM_Status = 1      
ORDER BY Salesman