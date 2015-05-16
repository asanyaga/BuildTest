--STOCKIST STOCK--
--Doesn't include Returnables
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_D_Stockist_StockByStockist]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[usp_D_Stockist_StockByStockist]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_D_Stockist_StockByStockist]
(
@distributorId AS NVARCHAR(50),
@stockistId AS NVARCHAR(50),
@productId AS NVARCHAR(50)
)
AS 
if  LTRIM(RTRIM(@distributorId)) = 'ALL'  begin set @distributorId='%' end
if  LTRIM(RTRIM(@stockistId)) = 'ALL'  begin set @stockistId='%' end
if  LTRIM(RTRIM(@productId)) = 'ALL'  begin set @productId='%' end

SELECT stockist.Id StockistId,stockist.Name StockistName,
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
	JOIN tblCostCentre stockist ON stock.WareHouseId = stockist.Id
    JOIN dbo.tblCostCentre dist ON dist.Id = stockist.ParentCostCentreId

WHERE     stockist.CostCentreType = 4
	AND   stockist.CostCentreType2 = 1
	AND   stock.Balance >= 0 
	AND   CONVERT(NVARCHAR(50),dist.Id) LIKE ISNULL(@distributorId,'%')
	AND   CONVERT(NVARCHAR(50),stockist.id) LIKE ISNULL(@stockistId,'%')
	AND   CONVERT(NVARCHAR(50),prod.id) LIKE ISNULL(@productId,'%')
GO
-- EXEC usp_D_Stockist_StockByStockist @stockistId='ALL',@productId='ALL',@distributorId='ALL'