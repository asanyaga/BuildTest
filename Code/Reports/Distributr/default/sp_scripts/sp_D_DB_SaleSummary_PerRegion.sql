DROP PROCEDURE [dbo].[sp_D_DB_SaleSummary_PerRegion]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[sp_D_DB_SaleSummary_PerRegion]
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

SELECT     HQ.Id AS HqID, 
           HQ.CostCentreType AS HqCCTypeID, 
           HQ.Name AS HqName, 
           Distributor.Id AS DistributorID, 
           Distributor.Name AS DistributorName, 
           Salesman.Id AS SalesmanID, 
           Salesman.CostCentreType AS SalesmanCCTypeID, 
           Salesman.Name AS SalesmanName, 
           LineItem.Description, 
           docs.DocumentTypeId, 
           LineItem.Quantity * (LineItem.Value + LineItem.Vat) AS GrossSaleValue, 
           Salesman.ParentCostCentreId, 
           LineItem.ProductDiscount, 
           Distributor.Distributor_RegionId, 
           Region.Name AS Region, 
           Region.id as RegionId,
           Country.Name AS Country,
           Country.id as CountryId ,
           docs.SaleDiscount, docs.Id AS DocumentID, 
           Outlet.Id AS OutletID, 
           Outlet.Name AS OutletName,           
           Outlet.CostCentreType AS OutletCCtypeID, 
           [Routes].Name as RouteName, 
           [Routes].RouteID as RouteID
FROM       dbo.tblDocument AS docs INNER JOIN
                      dbo.tblLineItems AS LineItem ON docs.Id = LineItem.DocumentID INNER JOIN
                      dbo.tblCostCentre AS Salesman ON docs.DocumentRecipientCostCentre = Salesman.Id INNER JOIN
                      dbo.tblCostCentre AS Distributor ON Salesman.ParentCostCentreId = Distributor.Id INNER JOIN
                      dbo.tblCostCentre AS HQ ON Distributor.ParentCostCentreId = HQ.Id INNER JOIN
                      dbo.tblRegion  as Region ON Distributor.Distributor_RegionId = Region.id INNER JOIN
                      dbo.tblCountry as Country ON Region.Country = Country.id INNER JOIN
                      dbo.tblCostCentre as Outlet ON docs.OrderIssuedOnBehalfOfCC = Outlet.Id INNER JOIN
                      dbo.tblSalemanRoute ON Salesman.Id = dbo.tblSalemanRoute.SalemanId INNER JOIN
                      dbo.tblRoutes as [Routes] ON dbo.tblSalemanRoute.RouteId = [Routes].RouteID
WHERE     (docs.DocumentTypeId = 1) 
      AND (docs.OrderOrderTypeId = 1 OR docs.OrderOrderTypeId = 3) 
      AND (Salesman.CostCentreType = 4) 
      AND (docs.DocumentStatusId = 99)
      AND (CONVERT(VARCHAR(26), docs.DocumentDateIssued,23) BETWEEN @startDate AND @endDate) 
      AND (CONVERT(VARCHAR(50),Country.id) LIKE ISNULL (@countryId,'%'))
      AND (CONVERT(VARCHAR(50),Region.id) LIKE ISNULL (@regionId,'%'))
      AND (CONVERT(VARCHAR(50),HQ.id) LIKE ISNULL (@HqId,'%'))
      AND (CONVERT(VARCHAR(50),Distributor.id) LIKE ISNULL (@distributorId,'%'))
      AND (CONVERT(VARCHAR(50),Salesman.id) LIKE ISNULL (@salesmanId,'%'))
      AND (CONVERT(VARCHAR(50),[Routes].RouteID) LIKE ISNULL (@routesId,'%'))
      AND (CONVERT(VARCHAR(50),Outlet.id) LIKE ISNULL (@outletId,'%'))
      
      
-- Exec  [dbo].[sp_D_DB_SaleSummary_PerRegion]   @regionId='ALL',@countryId='ALL',@HqId='ALL',@distributorId='ALL',@routesId='ALL',@salesmanId='ALL',@outletId='ALL',@startDate ='1-Jan-2013',@endDate ='31-Dec-2013'

 GO     

      





