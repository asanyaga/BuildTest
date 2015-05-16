--STOCKIST STOCK--
--Doesn't include Returnables
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_D_Stockist_StockByStockistSalesman]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[usp_D_Stockist_StockByStockistSalesman]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_D_Stockist_StockByStockistSalesman]
(
@distributorId AS NVARCHAR(50),
@stockistSalesmanId AS NVARCHAR(50),
@productId AS NVARCHAR(50)
)
AS 
if  LTRIM(RTRIM(@distributorId)) = 'ALL'  begin set @distributorId='%' end
if  LTRIM(RTRIM(@stockistSalesmanId)) = 'ALL'  begin set @stockistSalesmanId='%' end
if  LTRIM(RTRIM(@productId)) = 'ALL'  begin set @productId='%' end

SELECT stockistSalesman.Id StockistSalesmanId,stockistSalesman.Name StockistSalesmanName,
       prod.id ProductId,prod.Description ProductName,
	   CAST(ROUND((stock.Balance/retunablePack.Capacity),1) AS INT) NoOfCrates,
	   CAST(ROUND((stock.Balance%retunablePack.Capacity),1) AS INT) ExtraBottles,
	   retunablePack.Capacity ReturnablePackCapacity,
	   stock.Balance AvailableQty,
	   stock.Balance * prod.ExFactoryPrice StockValue
FROM     tblInventory stock
	JOIN tblProduct prod ON stock.ProductId = prod.id
	JOIN dbo.tblProductPackaging prodPack ON prod.PackagingId = prodPack.Id
	JOIN dbo.tblProduct retunable ON prod.Returnable = retunable.id
	JOIN dbo.tblProduct retunablePack ON retunable.Returnable = retunablePack.id
	JOIN tblCostCentre stockistSalesman ON stock.WareHouseId = stockistSalesman.Id
    JOIN dbo.tblCostCentre dist ON dist.Id = stockistSalesman.ParentCostCentreId

WHERE     stockistSalesman.CostCentreType = 4
	AND   stockistSalesman.CostCentreType2 = 2
	AND   stock.Balance >= 0 
    AND   CONVERT(NVARCHAR(50),dist.Id) LIKE ISNULL(@distributorId,'%')
	AND   CONVERT(NVARCHAR(50),stockistSalesman.id) LIKE ISNULL(@stockistSalesmanId,'%')
	AND   CONVERT(NVARCHAR(50),prod.id) LIKE ISNULL(@productId,'%')
GO
-- EXEC usp_D_Stockist_StockByStockistSalesman @stockistSalesmanId='ALL',@productId='ALL',@distributorId='ALL'