DROP PROCEDURE [dbo].[sp_D_SaleValueDiscount_For_CountryRegionRouteOutletDistSalesman]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[sp_D_SaleValueDiscount_For_CountryRegionRouteOutletDistSalesman]
(
@startDate as datetime,
@endDate as datetime,
@countryId AS NVARCHAR(50),
@regionId AS NVARCHAR(50),
@routeId AS NVARCHAR(50),
@outletId AS NVARCHAR(50),
@distributorId AS NVARCHAR(50),
@salesmanId AS NVARCHAR(50)
)
as 
if  RTRIM(LTRIM(@countryId))='ALL'  begin set @countryId='%' end
if  RTRIM(LTRIM(@regionId))='ALL'  begin set @regionId='%' end
if  RTRIM(LTRIM(@routeId))='ALL'  begin set @routeId='%' end
if  RTRIM(LTRIM(@outletId))='ALL'  begin set @outletId='%' end
if  RTRIM(LTRIM(@distributorId))='ALL'  begin set @distributorId='%' end
if  RTRIM(LTRIM(@salesmanId))='ALL'  begin set @salesmanId='%' end

SELECT sale.Id AS DocID, 
       sale.DocumentTypeId AS DocTypeId, 
       sale.OrderOrderTypeId AS OOtypeId, 
       sale.DocumentStatusId AS DocStatId, 
       sale.SaleDiscount AS SaleValueDiscounts, 
       sale.DocumentDateIssued AS DocDateIss, 
       sale.DocumentReference AS DocRef, 
       sale.DocumentRecipientCostCentre, 
       sale.DocumentIssuerCostCentreId 
FROM   dbo.tblDocument sale
 JOIN  dbo.tblCostCentre ON sale.OrderIssuedOnBehalfOfCC = dbo.tblCostCentre.Id 
 JOIN  dbo.tblRoutes ON dbo.tblCostCentre.RouteId = dbo.tblRoutes.RouteID 
 JOIN  dbo.tblRegion ON dbo.tblRoutes.RegionId = dbo.tblRegion.id
WHERE  (DocumentTypeId = 1) 
  AND ((OrderOrderTypeId = 1) OR(OrderOrderTypeId = 3 AND DocumentStatusId = 99))
  AND((CONVERT(NVARCHAR(50),DocumentIssuerCostCentreId) LIKE ISNULL(@distributorId,'%') and CONVERT(NVARCHAR(50),DocumentRecipientCostCentre) LIKE ISNULL(@salesmanId,'%')) 
   OR (CONVERT(NVARCHAR(50),DocumentIssuerCostCentreId) LIKE ISNULL(@salesmanId,'%') and CONVERT(NVARCHAR(50),DocumentRecipientCostCentre) LIKE ISNULL(@distributorId,'%'))) 
  AND (CONVERT(NVARCHAR(26),DocumentDateIssued ,23)) between @startDate and @endDate
  
  AND (CONVERT(NVARCHAR(50),dbo.tblRegion.Country) LIKE ISNULL(@countryId,'%'))
  AND (CONVERT(NVARCHAR(50),dbo.tblRoutes.RegionId) LIKE ISNULL(@regionId,'%'))
  AND (CONVERT(NVARCHAR(50),dbo.tblCostCentre.RouteId) LIKE ISNULL(@routeId,'%'))
  AND (CONVERT(NVARCHAR(50),sale.OrderIssuedOnBehalfOfCC) LIKE ISNULL(@outletId,'%'))
