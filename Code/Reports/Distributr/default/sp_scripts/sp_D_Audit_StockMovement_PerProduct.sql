IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_D_Audit_StockMovement_PerProduct]'))
DROP PROCEDURE [dbo].[sp_D_Audit_StockMovement_PerProduct]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_D_Audit_StockMovement_PerProduct]
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
SELECT ProductName ProductName,0 StockAdjustment,SUM(Qty) TotalSamplesIssued,0 TotalStockReturns,0 TotalSales,0 TotalLosses,0 TotalCloseOfDay,0 TotalPurchaseOrders,0 TotalInventoryTransfer,0 OpeningBalance,0 StockTake,''  StockTakeDate
FROM [v_D_Samples] --
WHERE CONVERT(NVARCHAR(50),DistributorId) LIKE ISNULL(@distributorId,'%')
  -- AND CONVERT(NVARCHAR(50),SalesmanId) LIKE ISNULL(@salesmanId,'%')
   AND CONVERT(NVARCHAR(26),SalesDate,23) BETWEEN @startDate AND @endDate
GROUP BY ProductName
  UNION
SELECT ProductName ProductName,SUM(ActualQty) StockAdjustment,0 TotalSamplesIssued,0 TotalStockReturns,0 TotalSales,0 TotalLosses,0 TotalCloseOfDay,0 TotalPurchaseOrders,0 TotalInventoryTransfer,0 OpeningBalance,0 StockTake,''  StockTakeDate
FROM v_D_StockAdjustment
GROUP BY ProductName
union
--**STOCK RETURNS: Stock returned to HQ**
 SELECT ProductName ProductName,0 StockAdjustment,0 TotalSamplesIssued, SUM(Qty)  TotalStockReturns,0 TotalSales,0 TotalLosses,0 TotalCloseOfDay,0 TotalPurchaseOrders,0 TotalInventoryTransfer,0 OpeningBalance,0 StockTake,''  StockTakeDate
  FROM [dbo].[v_D_StockReturns]
  WHERE CONVERT(NVARCHAR(50),DistributorId) LIKE ISNULL(@distributorId,'%')
    --AND CONVERT(NVARCHAR(50),SalesmanId) LIKE ISNULL(@salesmanId,'%')
    AND CONVERT(NVARCHAR(26),ReturnDate,23) BETWEEN @startDate AND @endDate
	GROUP BY ProductName
 UNION          
--**SALES:All Sales by Distributor(POS + MOBILE )**

 SELECT  ProductName ProductName,0 StockAdjustment, 0 TotalSamplesIssued,0 TotalStockReturns,SUM(TotalSales) TotalSales,0 TotalLosses,0 TotalCloseOfDay,0 TotalPurchaseOrders,0 TotalInventoryTransfer,0 OpeningBalance,0 StockTake,''  StockTakeDate
 FROM [dbo].[v_D_Sales]  
 WHERE CONVERT(NVARCHAR(50),DistributorId) LIKE ISNULL(@distributorId,'%')
   AND CONVERT(NVARCHAR(50),SalesmanId) LIKE ISNULL(@salesmanId,'%')
   AND CONVERT(NVARCHAR(26),SalesDate,23) BETWEEN @startDate AND @endDate
   GROUP BY ProductName
UNION
 --**LOSSES** 
 SELECT ProductName ProductName, 0 StockAdjustment,0 TotalSamplesIssued,0 TotalStockReturns,0 TotalSales,SUM(LostQty) TotalLosses,0 TotalCloseOfDay,0 TotalPurchaseOrders,0 TotalInventoryTransfer,0 OpeningBalance,0 StockTake,''  StockTakeDate
 FROM [dbo].[v_D_Losses] 
 WHERE CONVERT(NVARCHAR(50),SalesmanId) LIKE ISNULL(@salesmanId,'%')
 --  AND CONVERT(NVARCHAR(50),DistributorId) LIKE ISNULL(@distributorId,'%')
   AND CONVERT(NVARCHAR(26),ReturnDate,23) BETWEEN @startDate AND @endDate 
   GROUP BY ProductName
 --**CLOSE OF DAY:   
 UNION

  SELECT ProductName ProductName,0 StockAdjustment,0 TotalSamplesIssued,0 TotalStockReturns,0 TotalSales,0 TotalLosses,SUM(Qty) TotalCloseOfDay,0 TotalPurchaseOrders,0 TotalInventoryTransfer,0 OpeningBalance,0 StockTake,''  StockTakeDate
  FROM [dbo].[v_D_CloseOfDay] 
  WHERE CONVERT(NVARCHAR(50),DistributorId) LIKE ISNULL(@distributorId,'%')
    AND CONVERT(NVARCHAR(50),SalesmanId) LIKE ISNULL(@salesmanId,'%')
    AND CONVERT(NVARCHAR(26),SaleDate,23) BETWEEN @startDate AND @endDate 
	GROUP BY ProductName
  UNION
--**PURCHASE ORDERS
  SELECT ProductName ProductName,0 StockAdjustment, 0 TotalSamplesIssued,0 TotalStockReturns,0 TotalSales,0 TotalLosses,0 TotalCloseOfDay,SUM(TotalPurchaseOrders)  TotalPurchaseOrders,0 TotalInventoryTransfer,0 OpeningBalance,0 StockTake,''  StockTakeDate
  FROM [dbo].[v_D_PurchaseOrders] 
  WHERE CONVERT(NVARCHAR(50),DistributorId) LIKE ISNULL(@distributorId,'%')
    --AND CONVERT(NVARCHAR(50),SalesmanId) LIKE ISNULL(@salesmanId,'%')
    AND CONVERT(NVARCHAR(26),PoDate,23) BETWEEN @startDate AND @endDate
	GROUP BY ProductName
  UNION
--**INVENTORY TRANSFER: Stock Issues to a Salesman**

 SELECT ProductName ProductName,0 StockAdjustment,0 TotalSamplesIssued,0 TotalStockReturns,0 TotalSales,0 TotalLosses,0 TotalCloseOfDay,0 TotalPurchaseOrders,SUM(Qty) TotalInventoryTransfer,0 OpeningBalance,0 StockTake,''  StockTakeDate
 FROM [dbo].[v_D_InvTransfer] 
 WHERE CONVERT(NVARCHAR(50),DistributorId) LIKE ISNULL(@distributorId,'%')
   AND CONVERT(NVARCHAR(50),SalesmanId) LIKE ISNULL(@salesmanId,'%')
   AND CONVERT(NVARCHAR(26),ItDate,23) BETWEEN @startDate AND @endDate
   GROUP BY ProductName
 UNION
-- ** OPENING BALANCE ** 
 SELECT ProductName ProductName,0 StockAdjustment,0 TotalSamplesIssued,0 TotalStockReturns,0 TotalSales,0 TotalLosses,0 TotalCloseOfDay,0 TotalPurchaseOrders,0 TotalInventoryTransfer,SUM(OpeningBalance)  OpeningBalance,0 StockTake,''  StockTakeDate
  FROM v_D_OpeningBalance
  WHERE CONVERT(NVARCHAR(26),DateOfEntry,23) LIKE CONVERT(NVARCHAR(26),GETDATE(),23)
  GROUP BY ProductName
 UNION
--**STOCK TAKE **
 SELECT ProductName ProductName,0 StockAdjustment,0 TotalSamplesIssued,0 TotalStockReturns,0 TotalSales,0 TotalLosses,0 TotalCloseOfDay,0 TotalPurchaseOrders,0 TotalInventoryTransfer,0 OpeningBalance,SUM(ActualQty) StockTake,CONVERT(NVARCHAR(26),StockTakeDate,23)  StockTakeDate
 FROM [dbo].[v_D_StockTake] 
 GROUP BY ProductName,StockTakeDate
 --GROUP BY StockTakeDate

 -- exec [dbo].[sp_D_Audit_StockMovement_PerProduct] @startDate ='2015-01-01',@endDate='2015-03-30',@distributorId ='ALL',@salesmanId ='ALL',@productId='ALL'

