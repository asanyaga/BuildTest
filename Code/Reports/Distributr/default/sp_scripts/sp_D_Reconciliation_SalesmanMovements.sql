DROP PROCEDURE [dbo].[sp_D_Reconciliation_SalesmanMovements]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[sp_D_Reconciliation_SalesmanMovements]
(
@startDate as datetime,
@endDate as datetime,
@countryId as nvarchar(50),
@regionId as nvarchar(50),
@distributorId AS NVARCHAR(50),
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


SELECT     sales.DocumentReference as SalesReference, 
           case when sales.Latitude = null then 'No Latitude' else sales.Latitude  end as Latitude ,
           case when sales.Longitude = null then 'No Longitude' else sales.Longitude  end as Longitude ,
           -- sales.Longitude,           
           saleItems.Quantity, 
           saleItems.Value, 
           saleItems.Vat, 
           
           convert(nvarchar(26),sales.DocumentDateIssued,23) as SalesDate, 
           RIGHT(CONVERT(VARCHAR(20),sales.DocumentDateIssued, 100), 7) as SalesTime, 
           --convert(time,sales.DocumentDateIssued,23) as SalesTime, 
           outlet.Id as OutletId, 
           outlet.Name as OutletName,           
           salesman.Id as SalesmanId, 
           salesman.Name as SalesmanName,
           [route].RouteID as RouteId,
           [route].Name as RouteName
FROM       dbo.tblDocument sales 
INNER JOIN dbo.tblLineItems saleItems ON sales.Id = saleItems.DocumentID 
INNER JOIN dbo.tblCostCentre outlet ON sales.OrderIssuedOnBehalfOfCC = outlet.Id 
INNER JOIN dbo.tblCostCentre salesman ON (sales.DocumentIssuerCostCentreId = salesman.Id or sales.DocumentRecipientCostCentre = salesman.Id)
inner join dbo.tblRoutes [route] on outlet.RouteId = [route].RouteID
inner join dbo.tblCostCentre distributor on salesman.ParentCostCentreId = distributor.Id
inner join dbo.tblRegion region on distributor.Distributor_RegionId = region.id
inner join dbo.tblCountry country on region.Country = country.id

WHERE     (sales.DocumentTypeId = 1) 
      AND ((sales.OrderOrderTypeId = 1) OR  (sales.OrderOrderTypeId = 3 AND sales.DocumentStatusId = 99)) 
      AND (outlet.CostCentreType = 5) 
      AND (salesman.CostCentreType = 4)
      AND (CONVERT(NVARCHAR(50), country.id) LIKE ISNULL(@countryId,N'%'))
      AND (CONVERT(NVARCHAR(50), region.id) LIKE ISNULL(@regionId,N'%'))
      AND (CONVERT(NVARCHAR(50), distributor.Id) LIKE ISNULL(@distributorId,N'%'))
      AND (CONVERT(NVARCHAR(50), salesman.Id) LIKE ISNULL(@salesmanId,N'%'))
      AND (CONVERT(NVARCHAR(50), outlet.Id) LIKE ISNULL(@outletId,N'%'))
      AND (CONVERT(NVARCHAR(50), [route].RouteID) LIKE ISNULL(@routeId,N'%'))
      AND (CONVERT(nvarchar(26),sales.DocumentDateIssued,23) between @startDate and @endDate)
 
      
 order by sales.DocumentDateIssued desc
 
 
--   Exec [dbo].[sp_D_Reconciliation_SalesmanMovements] @countryId='ALL',@regionId='ALL', @distributorId='ALL',@salesmanId='ALL',@routeId='ALL',@outletId='ALL',@startDate ='1-Jan-2014',@endDate='30-Dec-2014'


GO