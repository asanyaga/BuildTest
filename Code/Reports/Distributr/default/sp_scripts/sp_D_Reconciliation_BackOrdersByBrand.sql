
DROP PROCEDURE [dbo].[sp_D_Reconciliation_BackOrdersByBrand]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_D_Reconciliation_BackOrdersByBrand]
(
@startDate AS DATE,
@endDate AS DATE,
@distributorId AS NVARCHAR(50),
@salesmanId AS NVARCHAR(50),
@countryId AS NVARCHAR(50),
@regionId AS NVARCHAR(50),
@routeId AS NVARCHAR(50),
@outletId AS NVARCHAR(50),
@brandId AS NVARCHAR(50)


)
AS 
if LTRIM(RTRIM(@countryId))='ALL' begin set @countryId='%' end
if LTRIM(RTRIM(@regionId))='ALL' begin set @regionId='%' end
if LTRIM(RTRIM(@distributorId))='ALL' begin set @distributorId='%' end
if LTRIM(RTRIM(@salesmanId))='ALL' begin set @salesmanId='%' end
if LTRIM(RTRIM(@routeId))='ALL' begin set @routeId='%' end
if LTRIM(RTRIM(@outletId))='ALL' begin set @outletId='%' end
if LTRIM(RTRIM(@brandId))='ALL' begin set @brandId='%' end


SELECT     orders.DocumentReference, 
           orders.DocumentDateIssued, 
           outlet.Id AS OutletId, 
           outlet.Name AS OutletName, 
           [route].RouteID AS RouteId, 
           [route].Name AS RouteName, 
           salesman.Id AS SalesmanId, 
           salesman.Name AS SalesmanName, 
           ordersItems.LostSaleQuantity, 
           ordersItems.ApprovedQuantity, 
           ordersItems.description,
           brand.id as BrandId, 
           brand.name as BrandName

FROM         dbo.tblProduct 
INNER JOIN   dbo.tblProductBrand brand ON dbo.tblProduct.BrandId = brand.id 
INNER JOIN   dbo.tblDocument AS orders 
INNER JOIN   dbo.tblLineItems AS ordersItems ON orders.Id = ordersItems.DocumentID 
INNER JOIN   dbo.tblCostCentre AS outlet ON orders.OrderIssuedOnBehalfOfCC = outlet.Id 
INNER JOIN   dbo.tblRoutes  [route] ON outlet.RouteId = [route].RouteID 
INNER JOIN   dbo.tblCostCentre AS salesman ON orders.DocumentRecipientCostCentre = salesman.Id ON dbo.tblProduct.id = ordersItems.ProductID
INNER JOIN   dbo.tblCostCentre as distributor on salesman.ParentCostCentreId = distributor.Id
INNER JOIN   dbo.tblRegion as region on distributor.Distributor_RegionId = region.id
INNER JOIN   dbo.tblCountry as country on region.Country = country.id

WHERE     (orders.DocumentTypeId = 1) 
      AND (orders.OrderOrderTypeId = 3) 
      AND (ordersItems.OrderLineItemType = 1) 
      AND (orders.Id <> orders.DocumentParentId)
      AND (orders.DocumentStatusId = 4) 
      AND (outlet.CostCentreType = 5) 
      AND (salesman.CostCentreType = 4)      
      AND (CONVERT(NVARCHAR(50), country.id) LIKE ISNULL(@countryId,N'%'))
      AND (CONVERT(NVARCHAR(50), region.id) LIKE ISNULL(@regionId,N'%'))
      AND (CONVERT(NVARCHAR(50), distributor.Id) LIKE ISNULL(@distributorId,N'%'))
      AND (CONVERT(NVARCHAR(50), salesman.Id) LIKE ISNULL(@salesmanId,N'%'))
      AND (CONVERT(NVARCHAR(50), outlet.Id) LIKE ISNULL(@outletId,N'%'))
      AND (CONVERT(NVARCHAR(50), [route].RouteID) LIKE ISNULL(@routeId,N'%'))
      AND (CONVERT(NVARCHAR(50), brand.id) LIKE ISNULL(@brandId,N'%'))
      AND (CONVERT(nvarchar(26),orders.DocumentDateIssued,23) between @startDate and @endDate)

      
   --   Exec  [dbo].[sp_D_Reconciliation_BackOrdersByBrand] @distributorId='ALL',@salesmanId='ALL',@countryId='ALL',@regionId='ALL',@routeId='ALL',@outletId='ALL',@brandId='ALL',@startDate = '1-jan-2013',@endDate = '30-dec-2013'
  
  Go
