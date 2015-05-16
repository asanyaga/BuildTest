---STOCKIST SALESMAN SALES
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_D_Stockist_SalesByStockistSalesman]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[usp_D_Stockist_SalesByStockistSalesman]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_D_Stockist_SalesByStockistSalesman]
(
@startDate as date,
@endDate as date,
@distributorId AS NVARCHAR(50),
@stockistSalesmanId AS NVARCHAR(50),
@productId AS NVARCHAR(50)

)
AS 
if  ltrim(rtrim(@distributorId))='ALL'  begin set @distributorId='%' end
if  ltrim(rtrim(@stockistSalesmanId))='ALL'  begin set @stockistSalesmanId='%' end
if  ltrim(rtrim(@productId))='ALL'  begin set @productId='%' end

;WITH StockistCrateSales_CTE AS (
SELECT stockistSalesman.Id as StockistSalesmanId,stockistSalesman.Name as StockistSalesmanName,
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
 JOIN dbo.tblCostCentre stockistSalesman  on sale.DocumentIssuerCostCentreId = stockistSalesman.Id or sale.DocumentRecipientCostCentre = stockistSalesman.Id
 JOIN dbo.tblCostCentre dist ON dist.Id = stockistSalesman.ParentCostCentreId
 JOIN dbo.tblProductPackaging prodPack ON prod.PackagingId = prodPack.Id
 JOIN dbo.tblProduct retunable ON prod.Returnable = retunable.id
 JOIN dbo.tblProduct retunablePack ON retunable.Returnable = retunablePack.id
WHERE (sale.OrderOrderTypeId = 1) 
  AND ((sale.DocumentTypeId = 1)OR(sale.OrderOrderTypeId = 3) AND (sale.DocumentStatusId = 99))
  AND stockistSalesman.CostCentreType = 4
  AND stockistSalesman.CostCentreType2 = 2
  AND CONVERT(NVARCHAR(50),dist.Id) LIKE ISNULL(@distributorId,'%')
  AND CONVERT(NVARCHAR(50),stockistSalesman.id) LIKE ISNULL(@stockistSalesmanId,'%')
  AND CONVERT(NVARCHAR(50),prod.id) LIKE ISNULL(@productId,'%')
  AND CONVERT(NVARCHAR(26),sale.DocumentDateIssued,23) BETWEEN @startDate AND @endDate
GROUP BY stockistSalesman.Id,stockistSalesman.Name,
		 prod.id, prod.Description,prodPack.description, prod.Returnable,saleItems.ProductDiscount,
		 retunable.Description,retunablePack.Description,retunablePack.Capacity,saleItems.Quantity,saleItems.Vat,saleItems.Value
)
SELECT StockistSalesmanId,
	   StockistSalesmanName,
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
FROM StockistCrateSales_CTE
GROUP BY StockistSalesmanId,StockistSalesmanName,
		 ProductId,ProductName,PackagingName,ReturnablePackCapacity



GO

-- EXEC [dbo].[usp_D_Stockist_SalesByStockistSalesman] @startDate='2015-03-05',@endDate='2015-03-06',@stockistSalesmanId=' ALL',@productId=' ALL',@distributorId='ALL'
