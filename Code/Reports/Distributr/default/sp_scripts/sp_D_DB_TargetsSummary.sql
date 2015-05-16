DROP PROCEDURE [dbo].[sp_D_DB_TargetsSummary]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[sp_D_DB_TargetsSummary]
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



SELECT     Distributor.Id AS DistributorId, 
           Distributor.Name AS DistributorName, 
           Distributor.CostCentreType AS DistributorCCtype, 
           Outlets.Id AS OutletId, 
           Outlets.Name AS OutletName, 
           Outlets.CostCentreType AS OutletCCtype, 
           [Routes].RouteID AS RouteId, 
           [Routes].Name AS RouteName, 
           Region.id AS RegionId, 
           Region.Name AS RegionName, 
           Country.id AS CountryId, 
           Country.Name AS CountryName, 
           Targets.id AS TargetId, 
           Targets.TargetValue, 
           dbo.tblTargetPeriod.Id AS TargetPeriodId, 
           dbo.tblTargetPeriod.Name AS TargetPeriodName, 
           dbo.tblTargetPeriod.StartDate, 
           dbo.tblTargetPeriod.EndDate, 
           Targets.IsQuantityTarget, 
           Hq.Id AS HqId, 
           Hq.Name AS HqName, 
           Hq.CostCentreType AS HqCCtype

FROM         dbo.tblTargetPeriod 
        INNER JOIN dbo.tblTarget as Targets ON dbo.tblTargetPeriod.Id = Targets.PeriodId 
        INNER JOIN dbo.tblCostCentre as Distributor
        INNER JOIN dbo.tblCostCentre AS Outlets ON Distributor.Id = Outlets.ParentCostCentreId 
        INNER JOIN dbo.tblRoutes as [Routes] ON Outlets.RouteId = [Routes].RouteID 
        INNER JOIN dbo.tblRegion as Region ON [Routes].RegionId = Region.id 
        INNER JOIN dbo.tblCountry as Country ON Region.Country = Country.id ON Targets.CostCentreId = Distributor.Id 
        INNER JOIN dbo.tblCostCentre AS Hq ON Distributor.ParentCostCentreId = Hq.Id
WHERE     (Distributor.CostCentreType = 2) 
      AND (Outlets.CostCentreType = 5)
      AND (CONVERT(VARCHAR(50),Country.id) LIKE ISNULL (@countryId,'%'))
      AND (CONVERT(VARCHAR(50),Region.id) LIKE ISNULL (@regionId,'%'))
      AND (CONVERT(VARCHAR(50),Hq.id) LIKE ISNULL (@HqId,'%'))
      AND (CONVERT(VARCHAR(50),Distributor.id) LIKE ISNULL (@distributorId,'%'))
      --AND (CONVERT(VARCHAR(50),Salesman.id) LIKE ISNULL (@salesmanId,'%'))
      AND (CONVERT(VARCHAR(50),[Routes].RouteID) LIKE ISNULL (@routesId,'%'))
      AND (CONVERT(VARCHAR(50),Outlets.id) LIKE ISNULL (@outletId,'%'))
      
--   Exec      [dbo].[sp_D_DB_TargetsSummary]  @regionId='ALL', @countryId='ALL',@HqId='ALL',@distributorId='ALL',@routesId='ALL',@salesmanId='ALL',@outletId='ALL',@startDate ='1-Jan-2013',@endDate ='1-Jan-2013'


GO