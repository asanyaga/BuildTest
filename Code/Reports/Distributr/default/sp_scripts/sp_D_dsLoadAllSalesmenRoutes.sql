DROP PROCEDURE [dbo].[sp_D_dsLoadAllSalesmenRoutes]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_D_dsLoadAllSalesmenRoutes]
(
@distributorId AS NVARCHAR(50),
@salesmanId AS NVARCHAR(50)
)
as
if  @distributorId='ALL'  begin set @distributorId='%' end
if  @salesmanId='ALL'  begin set @salesmanId='%' end
SELECT ' ALL' as RouteId, 
       ' ALL' as RouteName
       union all
SELECT Distinct lower(convert(nvarchar(50),[route].RouteID)) AS RouteId, 
                lower(convert(nvarchar(50),[route].Name)) AS RouteName
FROM       dbo.tblCostCentre salesman
INNER JOIN dbo.tblSalemanRoute ON salesman.Id = dbo.tblSalemanRoute.SalemanId 
INNER JOIN dbo.tblRoutes [route] ON dbo.tblSalemanRoute.RouteId = [route].RouteID 
WHERE     (salesman.CostCentreType = 4)
and convert(nvarchar(50), salesman.id) like ISNULL(@salesmanId,'%')
and convert(nvarchar(50), salesman.ParentCostCentreId) like ISNULL(@distributorId,'%')
and [route].IM_Status = 1
Order by RouteName asc


-- Exec [dbo].[sp_D_dsLoadAllSalesmenRoutes] @distributorId='916b71dd-89f2-49a6-b982-3e41a193b17e',@salesmanId='f99960ab-ecd9-4d00-a63e-4a71d75a6ff6'

Go