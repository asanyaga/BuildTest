DROP PROCEDURE [dbo].[sp_D_DB_SalesVsTargets_PerRegion]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[sp_D_DB_SalesVsTargets_PerRegion]
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


SELECT     Distributor.Id AS DistributorId, Distributor.Name AS DistributorName, Distributor.CostCentreType AS DistributorCCtype, Outlets.Id AS OutletId, 
                      Outlets.Name AS OutletName, Outlets.CostCentreType AS OutletCCtype, Routes.RouteID AS RouteId, Routes.Name AS RouteName, Region.id AS RegionId, 
                      Region.Name AS RegionName, Country.id AS CountryId, Country.Name AS CountryName, Targets.id AS TargetId, Targets.TargetValue, 
                      dbo.tblTargetPeriod.Id AS TargetPeriodId, dbo.tblTargetPeriod.Name AS TargetPeriodName, dbo.tblTargetPeriod.StartDate, dbo.tblTargetPeriod.EndDate, 
                      dbo.tblDocument.DocumentDateIssued, Targets.IsQuantityTarget, Hq.Id AS HqId, Hq.Name AS HqName, Hq.CostCentreType AS HqCCtype, 
                      dbo.tblDocument.DocumentTypeId, dbo.tblDocument.DocumentReference, dbo.tblDocument.OrderOrderTypeId, dbo.tblDocument.DocumentStatusId, 
                      (dbo.tblLineItems.Quantity * (dbo.tblLineItems.Value + dbo.tblLineItems.Vat)) as GrossSaleValue
FROM         dbo.tblTargetPeriod INNER JOIN
                      dbo.tblTarget AS Targets ON dbo.tblTargetPeriod.Id = Targets.PeriodId INNER JOIN
                      dbo.tblCostCentre AS Distributor INNER JOIN
                      dbo.tblCostCentre AS Outlets ON Distributor.Id = Outlets.ParentCostCentreId INNER JOIN
                      dbo.tblRoutes AS Routes ON Outlets.RouteId = Routes.RouteID INNER JOIN
                      dbo.tblRegion AS Region ON Routes.RegionId = Region.id INNER JOIN
                      dbo.tblCountry AS Country ON Region.Country = Country.id ON Targets.CostCentreId = Distributor.Id INNER JOIN
                      dbo.tblCostCentre AS Hq ON Distributor.ParentCostCentreId = Hq.Id INNER JOIN
                      dbo.tblDocument ON Distributor.Id = dbo.tblDocument.DocumentIssuerCostCentreId INNER JOIN
                      dbo.tblLineItems ON dbo.tblDocument.Id = dbo.tblLineItems.DocumentID
WHERE     (Distributor.CostCentreType = 2) AND (Outlets.CostCentreType = 5) AND (dbo.tblDocument.DocumentTypeId = 1) AND (dbo.tblDocument.OrderOrderTypeId = 1) OR
                      (dbo.tblDocument.OrderOrderTypeId = 3) AND (dbo.tblDocument.DocumentStatusId = 99) AND (dbo.tblDocument.DocumentDateIssued BETWEEN 
                      dbo.tblTargetPeriod.StartDate AND dbo.tblTargetPeriod.EndDate) 
