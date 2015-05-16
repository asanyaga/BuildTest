
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_D_OrderValueDiscount]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[usp_D_OrderValueDiscount]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_D_OrderValueDiscount]
(
@startDate as datetime,
@endDate as datetime,
@distributorID nvarchar(50),
@salesmanId nvarchar(50),
@countryId nvarchar(50),
@regionId nvarchar(50),
@outletId nvarchar(50),
@routeId nvarchar(50),
@brandId nvarchar(50),
@subBrandId nvarchar(50),
@productId nvarchar(50),
@productTypeId nvarchar(50)
)
AS 
if  LTRIM(rtrim(@distributorId)) = 'ALL'  begin set @distributorId='%' end
if  LTRIM(rtrim(@productId)) = 'ALL'  begin set @productId='%' end
if  LTRIM(rtrim(@salesmanId)) = 'ALL'  begin set @salesmanId='%' end
if  LTRIM(rtrim(@countryId)) = 'ALL'  begin set @countryId='%' end
if  LTRIM(rtrim(@regionId)) = 'ALL'  begin set @regionId='%' end
if  LTRIM(rtrim(@outletId)) = 'ALL'  begin set @outletId='%' end
if  LTRIM(rtrim(@routeId)) = 'ALL'  begin set @routeId='%' end
if  LTRIM(rtrim(@brandId)) = 'ALL'  begin set @brandId='%' end
if  LTRIM(rtrim(@subBrandId)) = 'ALL'  begin set @subBrandId='%' end
if  LTRIM(rtrim(@productTypeId)) = 'ALL'  begin set @productTypeId='%' end


SELECT  sale.Id,(SELECT TOP(1) sale.SaleDiscount) SaleValueDiscounts
 FROM   dbo.tblDocument sale
 JOIN  dbo.tblLineItems saleItems ON saleItems.DocumentID = sale.Id
 JOIN  dbo.tblProduct prod ON saleItems.ProductID =  prod.id
 JOIN  dbo.tblCostCentre dist ON sale.DocumentIssuerCostCentreId = dist.Id OR sale.DocumentRecipientCostCentre = dist.Id
 JOIN  dbo.tblCostCentre sm ON sale.DocumentIssuerCostCentreId = sm.Id OR sale.DocumentRecipientCostCentre = sm.Id
 JOIN  dbo.tblCostCentre outlet ON sale.OrderIssuedOnBehalfOfCC = outlet.Id 
 JOIN dbo.tblRegion reg on dist.Distributor_RegionId  = reg.id

WHERE   (DocumentTypeId = 1) 
   AND OrderOrderTypeId = 3
   AND dist.CostCentreType = 2
   AND sm.CostCentreType = 4
   AND (CONVERT(NVARCHAR(26),DocumentDateIssued ,23)) between @startDate and @endDate
   AND CONVERT(NVARCHAR(50),reg.Country) LIKE ISNULL(@countryId,'%')
   AND CONVERT(NVARCHAR(50),reg.id) LIKE ISNULL(@regionId,'%')
   AND CONVERT(NVARCHAR(50),dist.Id) LIKE ISNULL(@distributorId,'%')
   AND CONVERT(NVARCHAR(50),sm.Id) LIKE ISNULL(@salesmanId,'%')
   AND CONVERT(NVARCHAR(50),outlet.Id) LIKE ISNULL(@outletId,'%')
   AND CONVERT(NVARCHAR(50),outlet.RouteId) LIKE ISNULL(@routeId,'%')
   AND CONVERT(NVARCHAR(50),prod.BrandId) LIKE ISNULL(@brandId,'%')   
   AND CONVERT(NVARCHAR(50),prod.id) LIKE ISNULL(@productId,'%')   
   AND CONVERT(NVARCHAR(50),prod.FlavourId) LIKE ISNULL(@subBrandId,'%')   
   AND CONVERT(NVARCHAR(50),prod.ProductTypeId) LIKE ISNULL(@productTypeId,'%')   
GROUP BY sale.Id,sale.SaleDiscount 
GO

-- EXEC [dbo].[usp_D_OrderValueDiscount] @startDate ='2014-01-22',@endDate ='2015-01-22',@distributorID=' ALL',@salesmanId=' ALL',@countryId=' ALL',@regionId=' ALL',@outletId=' ALL',@routeId=' ALL',@brandId=' ALL',@subBrandId=' ALL',@productId=' ALL',@productTypeId=' ALL'
