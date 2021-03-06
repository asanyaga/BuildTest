/****** Object:  StoredProcedure [dbo].[sp_D_RegionalSales_ByCountry]    Script Date: 07/24/2013 08:51:32 ******/
DROP PROCEDURE [dbo].[sp_D_RegionalSales_ByCountry]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[sp_D_RegionalSales_ByCountry]
(

@startDate as datetime,
@endDate as datetime,
@distributorID as nvarchar(50),
@regionId as nvarchar(50),
@countryId as nvarchar(50)
)
as 
if LTRIM(RTRIM(@distributorID))='ALL' begin set @distributorID='%' end
if LTRIM(RTRIM(@regionId))='ALL' begin set @regionId='%' end
if LTRIM(RTRIM(@countryId))='ALL' begin set @countryId='%' end

SELECT saleItems.ProductID, 
       saleItems.Description, 
       sales.DocumentTypeId, 
       saleItems.Quantity, 
       saleItems.Value, 
       saleItems.Vat, 
       sales.DocumentIssuerCostCentreId,   
       sales.DocumentDateIssued as SendDateTime, 
       salesman.Name AS Salesman, 
       distributor.Id AS DistributorID, 
       distributor.Name AS Distributor, 
       HQ.Name AS Producer, 
       HQ.CostCentreType AS ProducerID, 
       salesman.Id AS SalesManID, 
       prod.Description AS Product, 
       prodBrand.name AS Brand, 
       saleItems.ProductDiscount, 
       distributor.Distributor_RegionId,
       region.id AS RegionId, 
       region.Name AS Region,
       country.id AS CountryId, 
       country.Name AS Country, 
       sales.SaleDiscount,
       (saleItems.Quantity * saleItems.ProductDiscount) as TotalProductDiscount,
	   ROUND(((saleItems.Quantity)*(saleItems.Value + saleItems.Vat)) +  (saleItems.ProductDiscount*saleItems.Quantity) ,2,1) as GrossAmount,
	   dbo.udf_D_RoundOff((saleItems.Quantity*saleItems.Value)) as NetRoundOff,
	   ROUND((saleItems.Quantity*saleItems.Value),2,1) as NetTruncate,
	   dbo.udf_D_RoundOff(((saleItems.Quantity)*(saleItems.Value + saleItems.Vat))) as GrossRoundOff


FROM   dbo.tblDocument sales 
INNER JOIN dbo.tblLineItems saleItems ON sales.Id = saleItems.DocumentID 
INNER JOIN dbo.tblCostCentre salesman ON (sales.DocumentRecipientCostCentre = salesman.Id or sales.DocumentIssuerCostCentreId = salesman.Id )
INNER JOIN dbo.tblCostCentre AS distributor ON salesman.ParentCostCentreId = distributor.Id 
INNER JOIN dbo.tblCostCentre AS HQ ON distributor.ParentCostCentreId = HQ.Id 
INNER JOIN dbo.tblProduct prod ON saleItems.ProductID = prod.id 
INNER JOIN dbo.tblProductBrand prodBrand ON prod.BrandId = prodBrand.id 
INNER JOIN dbo.tblRegion region ON distributor.Distributor_RegionId = region.id 
INNER JOIN dbo.tblCountry country ON region.Country = country.id
WHERE (sales.DocumentTypeId = 1) 
       AND (sales.OrderOrderTypeId = 1 OR (sales.OrderOrderTypeId = 3  AND (sales.DocumentStatusId = 99))) 
       AND (CONVERT(VARCHAR(26), sales.DocumentDateIssued,23) BETWEEN @startDate AND @endDate) 
       AND (salesman.CostCentreType = 4)       
       AND (CONVERT(NVARCHAR(50),region.id) LIKE ISNULL(@regionId, '%')) 
       AND (CONVERT(NVARCHAR(50),country.id) LIKE ISNULL(@countryId, '%'))
       AND (CONVERT(NVARCHAR(50),distributor.Id) LIKE ISNULL(@distributorID, '%'))
       
   -- exec [dbo].[sp_D_RegionalSales_ByCountry] @startDate = '01-march-2014',@endDate = '11-march-2014',@regionId='ALL',@countryId='ALL',@distributorID='ALL'


GO
