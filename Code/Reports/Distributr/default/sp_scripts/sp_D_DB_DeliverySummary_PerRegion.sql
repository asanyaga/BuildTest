DROP PROCEDURE [dbo].[sp_D_DB_DeliverySummary_PerRegion]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[sp_D_DB_DeliverySummary_PerRegion]
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

SELECT     docs.Id AS DocId, 
           docs.DocumentReference, 
           LineItems.Quantity *(LineItems.Value + LineItems.Vat) as GrossValue,            
           Salesman.Id AS SalesmanId, 
           Salesman.Name AS SalesmanName, 
           Salesman.CostCentreType AS SalesmanCCtype, 
           Distributor.Id AS DistributorId, 
           Distributor.Name AS DistributorName, 
           Distributor.CostCentreType AS DistributorCCtype, 
           docs.DocumentParentId, 
           tblDocument_1.DocumentReference AS DeliveryDoc, 
           tblDocument_1.Id AS DeliveryId, 
           Outlet.Name AS OutletName, 
           Outlet.Id AS OutletId, 
           Outlet.CostCentreType AS OutletCCtype, 
           [Routes].RouteID AS RouteId, 
           [Routes].Name AS RouteName, 
           docs.SendDateTime, 
           Hq.Id AS HqId, 
           Hq.Name AS HqName, 
           Hq.CostCentreType AS HqCCtype, 
           Region.id AS RegionId, 
           Region.Name AS RegionName, 
           Country.id AS CountryId, 
           Country.Name AS CountryName

FROM       dbo.tblDocument AS docs 
           INNER JOIN dbo.tblLineItems AS LineItems ON docs.Id = LineItems.DocumentID 
           INNER JOIN dbo.tblCostCentre AS Salesman ON docs.DocumentIssuerCostCentreId = Salesman.Id 
           INNER JOIN dbo.tblCostCentre AS Distributor ON docs.DocumentRecipientCostCentre = Distributor.Id 
           INNER JOIN dbo.tblDocument AS tblDocument_1 ON docs.DocumentParentId = tblDocument_1.Id 
           INNER JOIN dbo.tblCostCentre AS Outlet ON tblDocument_1.OrderIssuedOnBehalfOfCC = Outlet.Id 
           INNER JOIN dbo.tblRoutes as [Routes] ON Outlet.RouteId = [Routes].RouteID 
           INNER JOIN dbo.tblCostCentre as Hq ON Distributor.ParentCostCentreId = Hq.Id 
           INNER JOIN dbo.tblRegion as Region ON [Routes].RegionId = Region.id 
           INNER JOIN dbo.tblCountry as Country ON Region.Country = Country.id

WHERE     (docs.DocumentTypeId = 2) 
      AND (docs.OrderOrderTypeId = 2)
      AND (Salesman.CostCentreType = 4) 
      AND (Distributor.CostCentreType = 2)
      AND (CONVERT(VARCHAR(26), docs.DocumentDateIssued,23) BETWEEN @startDate AND @endDate) 
      AND (CONVERT(VARCHAR(50),Country.id) LIKE ISNULL (@countryId,'%'))
      AND (CONVERT(VARCHAR(50),Region.id) LIKE ISNULL (@regionId,'%'))
      AND (CONVERT(VARCHAR(50),Hq.id) LIKE ISNULL (@HqId,'%'))
      AND (CONVERT(VARCHAR(50),Distributor.id) LIKE ISNULL (@distributorId,'%'))
      AND (CONVERT(VARCHAR(50),Salesman.id) LIKE ISNULL (@salesmanId,'%'))
      AND (CONVERT(VARCHAR(50),[Routes].RouteID) LIKE ISNULL (@routesId,'%'))
      AND (CONVERT(VARCHAR(50),Outlet.id) LIKE ISNULL (@outletId,'%'))
      
      
 --Exec  [dbo].[sp_D_DB_DeliverySummary_PerRegion]   @regionId='ALL',@countryId='ALL',@HqId='ALL',@distributorId='ALL',@routesId='ALL',@salesmanId='ALL',@outletId='ALL',@startDate ='1-Jan-2013',@endDate ='31-Dec-2013'

 GO     

      





