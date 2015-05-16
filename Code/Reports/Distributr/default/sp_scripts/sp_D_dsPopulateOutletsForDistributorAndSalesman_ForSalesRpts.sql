DROP PROCEDURE [dbo].[sp_D_dsPopulateOutletsForDistributorAndSalesman_ForSalesRpts]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_D_dsPopulateOutletsForDistributorAndSalesman_ForSalesRpts]
(
@distributorID AS NVARCHAR(50),
@salesManID AS NVARCHAR(50)

)
as
if  ltrim(rtrim(@distributorID))='ALL'  begin set @distributorID='%' end
if  ltrim(rtrim(@salesManID))='ALL'  begin set @salesManID='%' end

SELECT     ' ALL' as OutletId, 
           ' ALL' as OutletName
UNION ALL
SELECT DISTINCT  lower(convert(nvarchar(50),outlets.Id)) AS OutletId, 
                 (convert(nvarchar(50),outlets.Name)) AS OutletName
FROM         dbo.tblCostCentre outlets 
INNER JOIN   tblSalemanRoute sr on outlets.RouteId = sr.RouteId 
--INNER JOIN   tblCostCentre sm on sr.SalemanId = sm.Id
WHERE     (outlets.CostCentreType = 5)
   AND convert(nvarchar(50),outlets.ParentCostCentreId) like ISNULL(@distributorID,'%')
   AND convert(nvarchar(50),sr.SalemanId) like ISNULL(@salesManID,'%')
   AND outlets.IM_Status = 1
   
ORDER BY OutletName asc
   
 -- Exec  [dbo].[sp_D_dsPopulateOutletsForDistributorAndSalesman_ForSalesRpts] @distributorID='ALL',@salesManID='ALL'
   
 GO