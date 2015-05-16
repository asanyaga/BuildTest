DROP PROCEDURE [dbo].[sp_D_dsPopulateOutletsForDistributor_Salesman_Route]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_D_dsPopulateOutletsForDistributor_Salesman_Route]
(
@distributorId AS NVARCHAR(50),
@salesmanId AS NVARCHAR(50),
@routeId AS NVARCHAR(50)
)
as
if ltrim(rtrim(@routeId))='ALL'  begin set @routeId='%' end
if ltrim(rtrim(@distributorId))='ALL'  begin set @distributorId='%' end
if ltrim(rtrim(@salesmanId))='ALL'  begin set @salesmanId='%' end

SELECT     ' ALL' as OutletId, 
           ' ALL' as OutletName
UNION ALL
SELECT DISTINCT  lower(convert(nvarchar(50),outlets.Id)) AS OutletId, 
                 (convert(nvarchar(50),outlets.Name)) AS OutletName
FROM         dbo.tblCostCentre outlets 
INNER JOIN   tblSalemanRoute sr on outlets.RouteId = sr.RouteId 
--INNER JOIN   tblCostCentre sm on sr.SalemanId = sm.Id
WHERE     (outlets.CostCentreType = 5)
   AND convert(nvarchar(50),outlets.ParentCostCentreId) like ISNULL(@distributorId,'%')
   AND convert(nvarchar(50),sr.SalemanId) like ISNULL(@salesmanId,'%')
   AND convert(nvarchar(50),outlets.RouteId ) like ISNULL(@routeId,'%')

   AND outlets.IM_Status = 1
   
ORDER BY OutletName asc
   
 -- Exec  [dbo].[sp_D_dsPopulateOutletsForDistributor_Salesman_Route] @distributorId='ALL',@salesmanId='ALL',@routeId='ALL'
   
 GO