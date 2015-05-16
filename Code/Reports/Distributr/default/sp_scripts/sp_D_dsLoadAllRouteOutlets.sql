DROP PROCEDURE [dbo].[sp_D_dsLoadAllRouteOutlets]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_D_dsLoadAllRouteOutlets]
(
@distributorId AS NVARCHAR(50),
@routeId AS NVARCHAR(50)
)
as
if  @distributorId='ALL'  begin set @distributorId='%' end
if  @routeId='ALL'  begin set @routeId='%' end
SELECT ' ALL' as OutletId, 
       ' ALL' as OutletName
       union all
SELECT     lower(convert(nvarchar(50),outlets.Id)) AS OutletId, 
           lower(convert(nvarchar(50),outlets.Name)) AS OutletName
FROM         dbo.tblCostCentre outlets 
INNER JOIN   dbo.tblRoutes [route] ON outlets.RouteId = [route].RouteID
WHERE     (outlets.CostCentreType = 5)
   AND convert(nvarchar(50),[route].RouteID) like ISNULL(@routeId,'%')
   AND convert(nvarchar(50),outlets.ParentCostCentreId) like ISNULL(@distributorId,'%')
   AND outlets.IM_Status = 1
   
ORDER BY OutletName asc
   
 -- Exec  [dbo].[sp_D_dsLoadAllRouteOutlets] @distributorId='ALL',@routeId='ALL'
   
 GO