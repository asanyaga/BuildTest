IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_D_Audit_Hub_StockMovement_PerProduct]'))
DROP PROCEDURE [dbo].[sp_D_Audit_Hub_StockMovement_PerProduct]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_D_Audit_Hub_StockMovement_PerProduct]
(
@startDate AS DATE,
@endDate AS DATE,
@distributorId AS NVARCHAR(50),
@salesmanId AS NVARCHAR(50),
@productId as nvarchar(50)
)
AS 
if  RTRIM(LTRIM(@distributorId))='ALL'  begin set @distributorId='%' end
if  RTRIM(LTRIM(@salesmanId))='ALL'  begin set @salesmanId='%' end
if  RTRIM(LTRIM(@productId))='ALL'  begin set @productId='%' end

--** OPENING BALANCE HUB **
 SELECT ProductCode,ProductName ProductName,0 StockAdjustment,0 TotalSamplesIssued,0 TotalStockReturns,0 TotalSales,0 TotalLosses,0 TotalCloseOfDay,0 TotalPurchaseOrders,0 TotalInventoryTransfer,SUM(OpeningBalance)  OpeningBalance,0 StockTake,''  StockTakeDate,0 TotalGoodsReceived,0 TotalDispatch
 FROM v_D_OpeningBalance 
 WHERE --CONVERT(NVARCHAR(26),DateOfEntry,23) LIKE CONVERT(NVARCHAR(26),GETDATE(),23)
        CONVERT(NVARCHAR(26),DateOfEntry,23) = @startDate  
    AND CONVERT(NVARCHAR(50),CostCentreId) LIKE ISNULL(@distributorId,'ALL')
 GROUP BY ProductName,ProductCode
   
   UNION

--** GOODS RECEIVED NOTES **
SELECT ProductCode,ProductName ProductName,0 StockAdjustment,0 TotalSamplesIssued,0 TotalStockReturns,0 TotalSales,0 TotalLosses,0 TotalCloseOfDay,0 TotalPurchaseOrders,0 TotalInventoryTransfer,0 OpeningBalance,0 StockTake,''  StockTakeDate,SUM(Qty) TotalGoodsReceived,0 TotalDispatch
FROM [dbo].[v_D_GoodsReceivedNote]
 WHERE CONVERT(NVARCHAR(50),DistributorId) LIKE ISNULL(@distributorId,'%')
 AND CONVERT(NVARCHAR(50),SalesmanId) LIKE ISNULL(@salesmanId,'%')
 AND CONVERT(NVARCHAR(26),TransDate,23) BETWEEN @startDate AND @endDate
GROUP BY ProductName,ProductCode
  UNION
--** STOCK ADJUSTMENT **
SELECT ProductCode,ProductName ProductName,SUM(ActualQty) StockAdjustment,0 TotalSamplesIssued,0 TotalStockReturns,0 TotalSales,0 TotalLosses,0 TotalCloseOfDay,0 TotalPurchaseOrders,0 TotalInventoryTransfer,0 OpeningBalance,0 StockTake,''  StockTakeDate,0 TotalGoodsReceived,0 TotalDispatch
FROM v_D_StockAdjustment
WHERE CONVERT(NVARCHAR(26),AdjustmentDate,23) BETWEEN @startDate AND @endDate
GROUP BY ProductName,ProductCode

  UNION
--** RETURNS FROM MOBILE / ClOSE OF DAY **
SELECT ProductCode,ProductName ProductName,0 StockAdjustment,0 TotalSamplesIssued,0 TotalStockReturns,0 TotalSales,0 TotalLosses,SUM(Qty) TotalCloseOfDay,0 TotalPurchaseOrders,0 TotalInventoryTransfer,0 OpeningBalance,0 StockTake,''  StockTakeDate,0 TotalGoodsReceived,0 TotalDispatch
FROM [dbo].[v_D_CloseOfDay] 
WHERE CONVERT(NVARCHAR(50),DistributorId) LIKE ISNULL(@distributorId,'%')
    AND CONVERT(NVARCHAR(50),SalesmanId) LIKE ISNULL(@salesmanId,'%')
    AND CONVERT(NVARCHAR(26),SaleDate,23) BETWEEN @startDate AND @endDate 
	GROUP BY ProductName,ProductCode
  UNION
--** STOCK ISSUED TO MOBILE **
 SELECT ProductCode,ProductName ProductName,0 StockAdjustment,0 TotalSamplesIssued,0 TotalStockReturns,0 TotalSales,0 TotalLosses,0 TotalCloseOfDay,0 TotalPurchaseOrders,SUM(Qty) TotalInventoryTransfer,0 OpeningBalance,0 StockTake,''  StockTakeDate,0 TotalGoodsReceived,0 TotalDispatch
 FROM [dbo].[v_D_InvTransfer] 
 WHERE CONVERT(NVARCHAR(50),DistributorId) LIKE ISNULL(@distributorId,'%')
   AND CONVERT(NVARCHAR(50),SalesmanId) LIKE ISNULL(@salesmanId,'%')
   AND CONVERT(NVARCHAR(26),ItDate,23) BETWEEN @startDate AND @endDate
 GROUP BY ProductName,ProductCode
    UNION
--** DISPATCHED ORDERS **
SELECT ProductCode,ProductName ProductName,0 StockAdjustment,0 TotalSamplesIssued,0 TotalStockReturns,0 TotalSales,0 TotalLosses,0 TotalCloseOfDay,0 TotalPurchaseOrders,0 TotalInventoryTransfer,0 OpeningBalance,0 StockTake,''  StockTakeDate,0 TotalGoodsReceived,SUM(Qty) TotalDispatch
FROM [dbo].[v_D_DispatchNote]
WHERE CONVERT(NVARCHAR(50),DistributorId) LIKE ISNULL(@distributorId,'%')
  AND CONVERT(NVARCHAR(50),SalesmanId) LIKE ISNULL(@salesmanId,'%')
  AND CONVERT(NVARCHAR(26),DocumentDateIssued,23) BETWEEN @startDate AND @endDate
GROUP BY ProductName,ProductCode
   UNION
--** SALES HUB **
 SELECT  ProductCode,ProductName ProductName,0 StockAdjustment, 0 TotalSamplesIssued,0 TotalStockReturns,SUM(TotalSales) TotalSales,0 TotalLosses,0 TotalCloseOfDay,0 TotalPurchaseOrders,0 TotalInventoryTransfer,0 OpeningBalance,0 StockTake,''  StockTakeDate,0 TotalGoodsReceived,0 TotalDispatch
 FROM [dbo].[v_D_Sales]  
 WHERE CONVERT(NVARCHAR(50),DistributorId) LIKE ISNULL(@distributorId,'%')
   AND CONVERT(NVARCHAR(50),SalesmanId) LIKE ISNULL(@salesmanId,'%')
   AND CONVERT(NVARCHAR(26),SalesDate,23) BETWEEN @startDate AND @endDate
 GROUP BY ProductName,ProductCode
     UNION

--** RETURNS TO HQ **
 SELECT ProductCode,ProductName ProductName,0 StockAdjustment,0 TotalSamplesIssued, 0  TotalStockReturns,0 TotalSales,0 TotalLosses,0 TotalCloseOfDay,0 TotalPurchaseOrders,0 TotalInventoryTransfer,0 OpeningBalance,0 StockTake,''  StockTakeDate,0 TotalGoodsReceived,0 TotalDispatch
 FROM [dbo].[v_D_StockReturns]
 WHERE CONVERT(NVARCHAR(50),DistributorId) LIKE ISNULL(@distributorId,'%')
   AND CONVERT(NVARCHAR(26),ReturnDate,23) BETWEEN @startDate AND @endDate
 GROUP BY ProductName,ProductCode
    UNION          

--** LOSSES **
SELECT ProductCode,ProductName ProductName, 0 StockAdjustment,0 TotalSamplesIssued,0 TotalStockReturns,0 TotalSales,SUM(LostQty) TotalLosses,0 TotalCloseOfDay,0 TotalPurchaseOrders,0 TotalInventoryTransfer,0 OpeningBalance,0 StockTake,''  StockTakeDate,0 TotalGoodsReceived,0 TotalDispatch
FROM [dbo].[v_D_Losses] 
WHERE CONVERT(NVARCHAR(50),SalesmanId) LIKE ISNULL(@salesmanId,'%')
  AND CONVERT(NVARCHAR(50),DistributorId) LIKE ISNULL(@distributorId,'%')
  AND CONVERT(NVARCHAR(26),ReturnDate,23) BETWEEN @startDate AND @endDate 
 GROUP BY ProductName,ProductCode
   UNION
--** HUB SAMPLE **
SELECT ProductCode,ProductName ProductName,0 StockAdjustment,SUM(Qty) TotalSamplesIssued,0 TotalStockReturns,0 TotalSales,0 TotalLosses,0 TotalCloseOfDay,0 TotalPurchaseOrders,0 TotalInventoryTransfer,0 OpeningBalance,0 StockTake,''  StockTakeDate,0 TotalGoodsReceived,0 TotalDispatch
FROM [v_D_Samples] --
WHERE  CONVERT(NVARCHAR(50),DistributorId) LIKE ISNULL(@distributorId,'%')
   AND CONVERT(NVARCHAR(26),SalesDate,23) BETWEEN @startDate AND @endDate
GROUP BY ProductName,ProductCode
  UNION
--** PHYSICAL STOCK TAKE **
 SELECT ProductCode,ProductName ProductName,0 StockAdjustment,0 TotalSamplesIssued,0 TotalStockReturns,0 TotalSales,0 TotalLosses,0 TotalCloseOfDay,0 TotalPurchaseOrders,0 TotalInventoryTransfer,0 OpeningBalance,SUM(ActualQty) StockTake,CONVERT(NVARCHAR(26),StockTakeDate,23)  StockTakeDate,0 TotalGoodsReceived,0 TotalDispatch
 FROM [dbo].[v_D_StockTake] 
 WHERE  CONVERT(NVARCHAR(50),DistributorId) LIKE ISNULL(@distributorId,'%')
    AND CONVERT(NVARCHAR(26),StockTakeDate,23) BETWEEN @startDate AND @endDate

 GROUP BY ProductName,StockTakeDate,ProductCode
 

--*****************************************END********************************

 -- exec [dbo].[sp_D_Audit_Hub_StockMovement_PerProduct] @startDate ='2015-01-01',@endDate='2015-02-26',@distributorId ='ALL',@salesmanId ='ALL',@productId='ALL'

