DROP PROCEDURE [dbo].[sp_D_Reconciliation_OutletSummary_ByProduct]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[sp_D_Reconciliation_OutletSummary_ByProduct]
(
@startDate as datetime,
@endDate as datetime,
@countryId as nvarchar(50),
@regionId as nvarchar(50),
@distributorId as nvarchar(50),
@salesmanId as nvarchar(50),
@routeId as nvarchar(50),
@outletId as nvarchar(50)
)
as 
if LTRIM(RTRIM(@countryId))='ALL' begin set @countryId='%' end
if LTRIM(RTRIM(@regionId))='ALL' begin set @regionId='%' end
if LTRIM(RTRIM(@distributorId))='ALL' begin set @distributorId='%' end
if LTRIM(RTRIM(@salesmanId))='ALL' begin set @salesmanId='%' end
if LTRIM(RTRIM(@routeId))='ALL' begin set @routeId='%' end
if LTRIM(RTRIM(@outletId))='ALL' begin set @outletId='%' end


SELECT salesItems.Quantity * (salesItems.Value +  salesItems.Vat) AS GrossSales,
       salesItems.Quantity, 
       salesItems.Value, 
       salesItems.Vat,       
       outlet.id AS OutletId, 
       outlet.Name AS OutletName, 
       distributor.Id AS DistributorId, 
       distributor.Name AS DistributorName, 
       route.Name AS RouteName,   
       salesman.Name AS Salesman, 
       route.RouteID AS RouteIDroutesTable, 
       salesItems.ProductDiscount, 
       region.id as RegionId,
       region.Name AS RegionName,
       country.id as CountryId, 
       country.Name AS CountryName,
       product.id as ProductId,                
       product.Description AS ProductName,
       brand.id as BrandId, 
       brand.name AS BrandName,
      (salesItems.Vat *salesItems.Quantity ) AS TotalVAT,     
      (salesItems.ProductDiscount * salesItems.Quantity) AS TotalProductDiscount
FROM        dbo.tblCostCentre distributor
INNER JOIN  dbo.tblCostCentre outlet ON distributor.Id = outlet.ParentCostCentreId 
INNER JOIN  dbo.tblDocument sales
INNER JOIN  dbo.tblLineItems salesItems ON sales.Id = salesItems.DocumentID ON outlet.Id = sales.OrderIssuedOnBehalfOfCC
INNER JOIN  dbo.tblRoutes route ON outlet.RouteId = route.RouteID 

INNER JOIN  dbo.tblCostCentre AS salesman ON sales.DocumentRecipientCostCentre = salesman.Id OR sales.DocumentIssuerCostCentreId = salesman.Id

INNER JOIN  dbo.tblRegion region ON route.RegionId = region.id 
INNER JOIN  dbo.tblCountry country ON region.Country = country.id 
INNER JOIN  dbo.tblProduct product ON salesItems.ProductID = product.id 
INNER JOIN  dbo.tblProductBrand brand ON product.BrandId = brand.id
WHERE (sales.DocumentTypeId = 1)
  AND (sales.OrderOrderTypeId = 1 OR ( sales.OrderOrderTypeId = 3  AND ( sales.DocumentStatusId = 99)))
  AND (salesman.CostCentreType = 4)  
  AND (outlet.CostCentreType = 5) 
  AND CONVERT(NVARCHAR(50),distributor.Id) like ISNULL(@distributorId,'%')
   AND CONVERT(NVARCHAR(50),salesman.Id) like ISNULL(@salesmanId,'%')
     AND CONVERT(NVARCHAR(50),country.Id) like ISNULL(@countryId,'%')
       AND CONVERT(NVARCHAR(50),region.Id) like ISNULL(@regionId,'%')
         AND CONVERT(NVARCHAR(50),route.RouteID) like ISNULL(@routeId,'%')
           AND CONVERT(NVARCHAR(50),outlet.Id) like ISNULL(@outletId,'%')
            AND convert(nvarchar(26),sales.DocumentDateIssued,23) between @startDate and @endDate




 -- exec [dbo].[sp_D_Reconciliation_OutletSummary_ByProduct] @distributorId='ALL',@salesmanId='ALL',@countryId='ALL',@regionId='ALL',@routeId='ALL',@outletId='ALL',@startDate='2014-07-23',@endDate='2014-07-24'
  
  
  GO
                 