DROP PROCEDURE [dbo].[sp_D_DB_Outlets_PerRegion]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[sp_D_DB_Outlets_PerRegion]
(
@startDate as datetime,
@endDate as datetime, 
@countryId as nvarchar(100),
@regionId as nvarchar(100),
@HqId as nvarchar(100),
@distributorId as nvarchar(100),
@routesId as nvarchar(100),
@salesmanId as nvarchar(100),
@outletId as nvarchar(100)
)
as 
if  @regionId='ALL'   begin set @regionId='%' end
if  @countryId='ALL'  begin set @countryId='%' end
if  @HqId='ALL'  begin set @HqId='%' end
if  @distributorId='ALL'  begin set @distributorId='%' end
if  @routesId='ALL'  begin set @routesId='%' end
if  @salesmanId='ALL'  begin set @salesmanId='%' end
if  @outletId='ALL'  begin set @outletId='%' end

SELECT     Outlet.Id AS OutletId, 
           Outlet.Name AS OutletName, 
           Outlet.CostCentreType AS OutletCCtype,           
           Distributor.Id AS DistributorId, 
           Distributor.Name AS DistributorName, 
           Distributor.CostCentreType AS DistributorCCtype, 
           Hq.Id AS HqId, 
           Hq.Name AS HqName, 
           Hq.CostCentreType AS HqCCtype, 
           [Routes].RouteID AS RouteId, 
           [Routes].Name AS RouteName, 
           Region.id AS RegionId, 
           Region.Name AS RegionName, 
           Country.id AS CountryId, 
           Country.Name AS CountryName, 
           Salesman.Id AS SalesmanId, 
           Salesman.Name AS SalesmanName
FROM       dbo.tblCostCentre as Outlet INNER JOIN
                      dbo.tblCostCentre AS Distributor ON Outlet.ParentCostCentreId = Distributor.Id INNER JOIN
                      dbo.tblCostCentre AS Hq ON Distributor.ParentCostCentreId = Hq.Id INNER JOIN
                      dbo.tblRoutes as [Routes] ON Outlet.RouteId = [Routes].RouteID INNER JOIN
                      dbo.tblRegion as Region ON [Routes].RegionId = Region.id INNER JOIN
                      dbo.tblCountry as Country ON Region.Country = Country.id INNER JOIN
                      dbo.tblSalemanRoute  ON [Routes].RouteID = dbo.tblSalemanRoute.RouteId AND [Routes].RouteID = dbo.tblSalemanRoute.RouteId INNER JOIN
                      dbo.tblCostCentre AS Salesman ON dbo.tblSalemanRoute.SalemanId = Salesman.Id
WHERE     (Outlet.CostCentreType = 5) 
      AND (Distributor.CostCentreType = 2) 
      AND (Hq.CostCentreType = 1)
      AND (CONVERT(VARCHAR(50),Country.id) LIKE ISNULL (@countryId,'%'))
      AND (CONVERT(VARCHAR(50),Region.id) LIKE ISNULL (@regionId,'%'))
      AND (CONVERT(VARCHAR(50),Hq.id) LIKE ISNULL (@HqId,'%'))
      AND (CONVERT(VARCHAR(50),Distributor.id) LIKE ISNULL (@distributorId,'%'))
      AND (CONVERT(VARCHAR(50),Salesman.id) LIKE ISNULL (@salesmanId,'%'))
      AND (CONVERT(VARCHAR(50),Outlet.id) LIKE ISNULL (@outletId,'%'))
      AND (CONVERT(VARCHAR(50),[Routes].RouteID) LIKE ISNULL (@routesId,'%'))


      
 --  Exec  [dbo].[sp_D_DB_Outlets_PerRegion]   @regionId='ALL',@countryId='ALL',@HqId='ALL',@distributorId='ALL',@routesId='ALL',@salesmanId='ALL',@outletId='ALL',@startDate ='1-Jan-2013',@endDate ='31-Dec-2013'

 GO     

      





