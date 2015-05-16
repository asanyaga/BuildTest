---RETURNABLES ORDERS
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_D_CrateOrders_PerRegion]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[sp_D_CrateOrders_PerRegion]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_D_CrateOrders_PerRegion]
(
@startDate as date,
@endDate as date,
@countryId AS NVARCHAR(50),
@regionId AS NVARCHAR(50)

)
AS 
if  ltrim(rtrim(@countryId))='ALL'  begin set @countryId='%' end
if  ltrim(rtrim(@regionId))='ALL'  begin set @regionId='%' end

;WITH CrateSales_CTE as(
SELECT cntry.id as CountryId,cntry.Name as CountryName,
       reg.id as RegionId,reg.Name as RegionName,
       SUM(saleItems.Quantity) as TotalSales,
	   SUM(saleItems.Vat * saleItems.Quantity) TotalVat,
	   SUM(saleItems.ProductDiscount) ProductDiscount,
	   prod.id ProductId,
	   prod.Description ProductName,
	   prodPack.description PackagingName,
	   prod.Returnable ReturnableId,
	   retunable.Description ReturnableName,
	   retunablePack.Description ReturnablePack,
	   retunablePack.Capacity ReturnablePackCapacity,
	   SUM(saleItems.Quantity) TotalQty,
	   CAST(ROUND((SUM(saleItems.Quantity) / retunablePack.Capacity),1) AS INT) NoOfCrates,
	   CAST((SUM(saleItems.Quantity) % retunablePack.Capacity) AS INT)  ExtraBottle,
   	   ROUND(((saleItems.Quantity)*(saleItems.Value + saleItems.Vat)) +  (saleItems.ProductDiscount*saleItems.Quantity) ,2,1) as GrossAmount,
	   dbo.udf_D_RoundOff((saleItems.Quantity*saleItems.Value)) as NetRoundOff,
	   ROUND((saleItems.Quantity*saleItems.Value),2,1) as NetTruncate,
	   dbo.udf_D_RoundOff(((saleItems.Quantity)*(saleItems.Value + saleItems.Vat))) as GrossRoundOff
FROM  dbo.tblDocument sale
 JOIN dbo.tblLineItems saleItems ON sale.Id = saleItems.DocumentID
 JOIN dbo.tblProduct prod ON saleItems.ProductID = prod.id 
 JOIN dbo.tblCostCentre dist on sale.DocumentIssuerCostCentreId = dist.Id or sale.DocumentRecipientCostCentre = dist.Id
 JOIN dbo.tblCostCentre salesman  on sale.DocumentIssuerCostCentreId = salesman.Id or sale.DocumentRecipientCostCentre = salesman.Id
 JOIN dbo.tblRegion reg ON dist.Distributor_RegionId = reg.id 
 JOIN dbo.tblCountry cntry ON reg.Country = cntry.id
 JOIN dbo.tblProductPackaging prodPack ON prod.PackagingId = prodPack.Id
 JOIN dbo.tblProduct retunable ON prod.Returnable = retunable.id
 JOIN dbo.tblProduct retunablePack ON retunable.Returnable = retunablePack.id
WHERE sale.DocumentTypeId = 1
  AND sale.OrderOrderTypeId = 3
  AND dist.CostCentreType = 2
  AND salesman.CostCentreType = 4
  AND CONVERT(NVARCHAR(50),cntry.id) LIKE ISNULL(@countryId,'%')
  AND CONVERT(NVARCHAR(50),reg.id) LIKE ISNULL(@regionId,'%')
  AND CONVERT(NVARCHAR(26),sale.DocumentDateIssued,23) BETWEEN @startDate AND @endDate
GROUP BY cntry.id,cntry.Name,
         reg.id,reg.Name,
		 prod.id, prod.Description,prodPack.description, prod.Returnable,saleItems.ProductDiscount,
		 retunable.Description,retunablePack.Description,retunablePack.Capacity,saleItems.Quantity,saleItems.Vat,saleItems.Value
)
SELECT CountryId,CountryName,
	   RegionId,RegionName,
	   ProductName,
       SUM(TotalSales) TotalSales,
	   SUM(TotalVat) TotalVat,
	   SUM(ProductDiscount) ProductDiscount,
   	   CAST(ROUND((SUM(TotalQty) / ReturnablePackCapacity),1) AS INT) NoOfCrates,
	   ReturnablePackCapacity ,
	   CAST((SUM(TotalQty) % ReturnablePackCapacity) AS INT)  ExtraBottle,
	   SUM(GrossAmount) GrossAmount,
	   SUM(NetTruncate) NetTruncate,
	   SUM(NetRoundOff) NetRoundOff,
	   SUM(GrossRoundOff) GrossRoundOff
FROM CrateSales_CTE
GROUP BY CountryId,CountryName,RegionId,RegionName,ProductName,PackagingName,ReturnablePackCapacity

-- EXEC [dbo].[sp_D_CrateOrders_PerRegion] @startDate='2015-01-01',@endDate='2015-01-14',@countryId=' ALL',@regionId=' ALL'