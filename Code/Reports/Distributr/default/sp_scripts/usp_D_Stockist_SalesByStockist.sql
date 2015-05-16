---STOCKIST SALES
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_D_Stockist_SalesByStockist]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[usp_D_Stockist_SalesByStockist]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_D_Stockist_SalesByStockist]
(
@startDate as date,
@endDate as date,
@distributorId AS NVARCHAR(50),
@stockistId AS NVARCHAR(50),
@productId AS NVARCHAR(50)

)
AS 
if  ltrim(rtrim(@distributorId))='ALL'  begin set @distributorId='%' end
if  ltrim(rtrim(@stockistId))='ALL'  begin set @stockistId='%' end
if  ltrim(rtrim(@productId))='ALL'  begin set @productId='%' end

;WITH StockistCrateSales_CTE AS (
SELECT stockist.Id as StockistId,stockist.Name as StockistName,
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
 JOIN dbo.tblCostCentre stockist on sale.DocumentIssuerCostCentreId = stockist.Id or sale.DocumentRecipientCostCentre = stockist.Id
 JOIN dbo.tblCostCentre dist ON dist.Id = stockist.ParentCostCentreId
 JOIN dbo.tblProductPackaging prodPack ON prod.PackagingId = prodPack.Id
 JOIN dbo.tblProduct retunable ON prod.Returnable = retunable.id
 JOIN dbo.tblProduct retunablePack ON retunable.Returnable = retunablePack.id
WHERE (sale.OrderOrderTypeId = 1) 
  AND ((sale.DocumentTypeId = 1)OR(sale.OrderOrderTypeId = 3) AND (sale.DocumentStatusId = 99))
  AND stockist.CostCentreType = 4
  AND stockist.CostCentreType2 = 1
  AND CONVERT(NVARCHAR(50),dist.Id) LIKE ISNULL(@distributorId,'%')
  AND CONVERT(NVARCHAR(50),stockist.id) LIKE ISNULL(@stockistId,'%')
  AND CONVERT(NVARCHAR(50),prod.id) LIKE ISNULL(@productId,'%')
  AND CONVERT(NVARCHAR(26),sale.DocumentDateIssued,23) BETWEEN @startDate AND @endDate
GROUP BY stockist.Id,stockist.Name,
		 prod.id, prod.Description,prodPack.description, prod.Returnable,saleItems.ProductDiscount,
		 retunable.Description,retunablePack.Description,retunablePack.Capacity,saleItems.Quantity,saleItems.Vat,saleItems.Value
)
SELECT StockistId,
	   StockistName,
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
GROUP BY StockistId,StockistName,
		 ProductId,ProductName,PackagingName,ReturnablePackCapacity



GO

-- EXEC [dbo].[usp_D_Stockist_SalesByStockist] @startDate='2015-03-05',@endDate='2015-03-09',@stockistId=' ALL',@productId=' ALL',@distributorId=' ALL'
