IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_D_Audit_StockMovement]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[sp_D_Audit_StockMovement]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_D_Audit_StockMovement]
(
@startDate AS DATE,
@endDate AS DATE,
@distributorId AS NVARCHAR(50),
@salesmanId AS NVARCHAR(50)
)
AS 
if  RTRIM(LTRIM(@distributorId))='ALL'  begin set @distributorId='%' end
if  RTRIM(LTRIM(@salesmanId))='ALL'  begin set @salesmanId='%' end


SELECT
--**SAMPLE: Promo Discounts & FOC Discounts**
(SELECT SUM(Qty) 
 FROM [dbo].[v_D_Samples]
 WHERE CONVERT(NVARCHAR(50),DistributorId) LIKE ISNULL(@distributorId,'%')
 --AND CONVERT(NVARCHAR(50),SalesmanId) LIKE ISNULL(@salesmanId,'%')
   AND CONVERT(NVARCHAR(26),SalesDate,23) BETWEEN @startDate AND @endDate
  ) as TotalSamplesIssued,
 (SELECT Sum(ActualQty)
  FROM dbo.v_D_StockAdjustment) as StockAdjustment,
--**STOCK RETURNS: Stock returned to HQ**
 (SELECT SUM(Qty) 
  FROM [dbo].[v_D_StockReturns]
  WHERE CONVERT(NVARCHAR(50),DistributorId) LIKE ISNULL(@distributorId,'%')
 -- AND CONVERT(NVARCHAR(50),SalesmanId) LIKE ISNULL(@salesmanId,'%')
    AND CONVERT(NVARCHAR(26),ReturnDate,23) BETWEEN @startDate AND @endDate
            ) as TotalStockReturns ,
--**SALES:All Sales by Distributor(POS + MOBILE )**
(SELECT SUM(TotalSales)
 FROM [dbo].[v_D_Sales]   
 WHERE CONVERT(NVARCHAR(50),DistributorId) LIKE ISNULL(@distributorId,'%')
   AND CONVERT(NVARCHAR(50),SalesmanId) LIKE ISNULL(@salesmanId,'%')
   AND CONVERT(NVARCHAR(26),SalesDate,23) BETWEEN @startDate AND @endDate
  ) as TotalSales,
 --**LOSSES** 
(SELECT SUM(LostQty)  
 FROM [dbo].[v_D_Losses]
 WHERE CONVERT(NVARCHAR(50),SalesmanId) LIKE ISNULL(@salesmanId,'%')
 --  AND CONVERT(NVARCHAR(50),DistributorId) LIKE ISNULL(@distributorId,'%')
   AND CONVERT(NVARCHAR(26),ReturnDate,23) BETWEEN @startDate AND @endDate ) as TotalLosses,
 --**CLOSE OF DAY:   
 (SELECT SUM(Qty) 
  FROM [dbo].[v_D_CloseOfDay]
  WHERE CONVERT(NVARCHAR(50),DistributorId) LIKE ISNULL(@distributorId,'%')
    AND CONVERT(NVARCHAR(50),SalesmanId) LIKE ISNULL(@salesmanId,'%')
    AND CONVERT(NVARCHAR(26),SaleDate,23) BETWEEN @startDate AND @endDate ) as TotalCloseOfDay,
--**PURCHASE ORDERS
 (SELECT SUM(TotalPurchaseOrders) 
  FROM [dbo].[v_D_PurchaseOrders]
  WHERE CONVERT(NVARCHAR(50),DistributorId) LIKE ISNULL(@distributorId,'%')
  --AND CONVERT(NVARCHAR(50),SalesmanId) LIKE ISNULL(@salesmanId,'%')
    AND CONVERT(NVARCHAR(26),PoDate,23) BETWEEN @startDate AND @endDate
   )as TotalPurchaseOrders,
--**INVENTORY TRANSFER: Stock Issues to a Salesman**
(SELECT SUM(Qty) 
 FROM [dbo].[v_D_InvTransfer]
 WHERE CONVERT(NVARCHAR(50),DistributorId) LIKE ISNULL(@distributorId,'%')
   AND CONVERT(NVARCHAR(50),SalesmanId) LIKE ISNULL(@salesmanId,'%')
   AND CONVERT(NVARCHAR(26),ItDate,23) BETWEEN @startDate AND @endDate ) as TotalInventoryTransfer,
-- ** OPENING BALANCE **
(SELECT SUM(OpeningBalance) FROM v_D_OpeningBalance
 WHERE CONVERT(NVARCHAR(26),DateOfEntry,23) LIKE CONVERT(NVARCHAR(26),GETDATE(),23)
 ) as OpeningBalance,
--**STOCK TAKE **
(SELECT TOP(1) SUM(ActualQty) FROM [dbo].[v_D_StockTake]GROUP BY StockTakeDate ORDER BY StockTakeDate DESC) as StockTake,
(SELECT TOP(1) CONVERT(NVARCHAR(26),StockTakeDate,23) FROM [dbo].[v_D_StockTake] GROUP BY StockTakeDate ORDER BY StockTakeDate DESC) as StockTakeDate



-- exec [dbo].[sp_D_Audit_StockMovement] @startDate ='2014-01-01',@endDate='2014-11-11',@distributorId ='ALL',@salesmanId ='ALL'

