
DROP PROCEDURE [dbo].[sp_D_Sales_SalesSummaryByProduct]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[sp_D_Sales_SalesSummaryByProduct]
(

@startDate as datetime,
@endDate as datetime,
@distributorID AS NVARCHAR(50),
@salesmanId AS NVARCHAR(50),
@brandId AS NVARCHAR(50),
@productId AS NVARCHAR(50),
@outletId AS NVARCHAR(50)

)
as 
if LTRIM(RTRIM(@distributorID))='ALL' begin set @distributorID='%' end
if LTRIM(RTRIM(@salesmanId))='ALL' begin set @salesmanId='%' end
if LTRIM(RTRIM(@brandId))='ALL' begin set @brandId='%' end
if LTRIM(RTRIM(@productId))='ALL' begin set @productId='%' end
if LTRIM(RTRIM(@outletId))='ALL' begin set @outletId='%' end

SELECT hq.Name AS HQ, 
       hq.Id AS HQId,
       distributor.Id AS DistributorId, 
       distributor.Name AS DistributorName, 
       salesman.Id AS SalesmanId, 
       salesman.Name AS SalesmanName, 
       outlet.Id OutletId,
       outlet.Name OutletName,
       saleItems.ProductID as ProductId,
       prod.Description AS ProductName,
       saleItems.Quantity Quantity,
       saleItems.Value Value, 
       saleItems.Vat Vat, 
       saleItems.Quantity * (saleItems.Value + saleItems.Vat) as SaleAmount, 
       sale.DocumentDateIssued as SaleDate, 
       brand.name AS Brand,
       brand.id AS BrandId,  
       saleItems.ProductDiscount ProductDiscount, 
       sale.SaleDiscount SaleDiscount,
       (saleItems.ProductDiscount * saleItems.Quantity) as TotalProductDiscount,
	   	  ROUND(((saleItems.Quantity)*(saleItems.Value + saleItems.Vat)) +  (saleItems.ProductDiscount*saleItems.Quantity) ,2,1) as GrossAmount,
	   dbo.udf_D_RoundOff((saleItems.Quantity*saleItems.Value)) as NetRoundOff,
	   ROUND((saleItems.Quantity*saleItems.Value),2,1) as NetTruncate,
	   dbo.udf_D_RoundOff(((saleItems.Quantity)*(saleItems.Value + saleItems.Vat))) as GrossRoundOff


FROM        dbo.tblDocument sale 
INNER JOIN  dbo.tblLineItems saleItems ON sale.Id = saleItems.DocumentID 
INNER JOIN  dbo.tblCostCentre salesman ON (sale.DocumentRecipientCostCentre = salesman.Id or sale.DocumentIssuerCostCentreId = salesman.Id )
INNER JOIN  dbo.tblCostCentre AS distributor ON salesman.ParentCostCentreId = distributor.Id 
INNER JOIN  dbo.tblCostCentre AS hq ON distributor.ParentCostCentreId = hq.Id 
INNER JOIN  dbo.tblProduct prod ON saleItems.ProductID = prod.id 
INNER JOIN  dbo.tblProductBrand brand ON prod.BrandId = brand.id
INNER JOIN dbo.tblCostCentre outlet on sale.OrderIssuedOnBehalfOfCC = outlet.Id
WHERE (sale.DocumentTypeId = 1)
 AND (sale.OrderOrderTypeId = 1 OR (sale.OrderOrderTypeId = 3 AND sale.DocumentStatusId = 99)) 
 AND (salesman.CostCentreType = 4)
 AND (CONVERT(VARCHAR(26),sale.DocumentDateIssued,23) BETWEEN @startDate AND @endDate)
 AND (CONVERT(NVARCHAR(50),distributor.Id) LIKE ISNULL(@distributorID, '%'))
 AND (CONVERT(NVARCHAR(50),salesman.Id) LIKE ISNULL(@salesmanId, '%')) 
 AND (CONVERT(NVARCHAR(50),brand.id) LIKE ISNULL(@brandId, '%'))
 AND (CONVERT(NVARCHAR(50),prod.id) LIKE ISNULL(@productId, '%'))
 AND (CONVERT(NVARCHAR(50), outlet.Id ) LIKE ISNULL(@outletId, '%'))
 
 
 
 
 
 --  [dbo].[sp_D_Sales_SalesSummaryByProduct] @startDate='2014-03-01',@endDate ='2014-03-20',@distributorID ='E8C61376-8FD2-46B8-B293-952D7B86F54F',@salesmanId ='C84CBCAA-4E07-4114-8638-9EE512588598',@brandId  ='ALL',@productId  ='E52FB37C-763A-4990-A89F-84A4ADDE11B7',@outletId  ='ALL'

 
 
 
 go