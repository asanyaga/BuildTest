IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_D_Audit_Salesman_StockMovement_PerProduct]'))
DROP PROCEDURE [dbo].[sp_D_Audit_Salesman_StockMovement_PerProduct]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_D_Audit_Salesman_StockMovement_PerProduct]
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


--** INFLOW **
-------------------------------------------------------------
--** Opening Balance **
SELECT Salesman,ProductCode,ProductName Product,SUM(Qty) OpeningBalance,0 IssuedStock,0 DispatchedOrders,0 MobileSales,0 DeliveredOrders, 0 MobileSamples,0 InventoryReturns,0 Losses
--FROM [dbo].[v_D_CloseOfDay] 
FROM v_D_OpeningBalance_salesman
WHERE --CONVERT(NVARCHAR(50),DistributorId) LIKE ISNULL(@distributorId,'%')
     CONVERT(NVARCHAR(50),SalesmanId) LIKE ISNULL(@salesmanId,'%')
	AND CONVERT(NVARCHAR(50),ProductId) LIKE ISNULL(@productId,'%')
  --AND CONVERT(NVARCHAR(26),SaleDate,23) BETWEEN @startDate AND @endDate
    AND CONVERT(NVARCHAR(26),SaleDate,23) = @startDate 
	GROUP BY ProductName,ProductCode,Salesman
	UNION
--** Stock Issued to Mobile**
 SELECT Salesman,ProductCode,ProductName Product,0 OpeningBalance,SUM(Qty) IssuedStock,0 DispatchedOrders,0 MobileSales,0 DeliveredOrders, 0 MobileSamples,0 InventoryReturns,0 Losses
 FROM [dbo].[v_D_InvTransfer] 
 WHERE CONVERT(NVARCHAR(50),DistributorId) LIKE ISNULL(@distributorId,'%')
   AND CONVERT(NVARCHAR(50),SalesmanId) LIKE ISNULL(@salesmanId,'%')
   AND CONVERT(NVARCHAR(50),ProductId) LIKE ISNULL(@productId,'%')
   AND CONVERT(NVARCHAR(26),ItDate,23) BETWEEN @startDate AND @endDate
 GROUP BY ProductName,ProductCode,Salesman

	UNION
--** Dispatched Orders from Hub **
SELECT SalesmanName Salesman,ProductCode,ProductName Product,0 OpeningBalance,0 IssuedStock,SUM(Qty) DispatchedOrders,0 MobileSales,0 DeliveredOrders, 0 MobileSamples,0 InventoryReturns,0 Losses
FROM [dbo].[v_D_DispatchNote]
WHERE CONVERT(NVARCHAR(50),DistributorId) LIKE ISNULL(@distributorId,'%')
  AND CONVERT(NVARCHAR(50),SalesmanId) LIKE ISNULL(@salesmanId,'%')
  AND CONVERT(NVARCHAR(50),ProductId) LIKE ISNULL(@productId,'%')
  AND CONVERT(NVARCHAR(26),DocumentDateIssued,23) BETWEEN @startDate AND @endDate
GROUP BY ProductName,ProductCode,SalesmanName
	UNION
-------------------------------------------------------------


--** OUTFLOW **
-------------------------------------------------------------
--** Mobile Sales **
SELECT SalesmanName Salesman,ProductCode,ProductName,0 OpeningBalance,0 IssuedStock,0 DispatchedOrders,SUM(Qty) MobileSales,0 DeliveredOrders, 0 MobileSamples,0 InventoryReturns,0 Losses
FROM [dbo].[v_D_MobileSales]
WHERE CONVERT(NVARCHAR(50),DistributorId) LIKE ISNULL(@distributorId,'%')
  AND CONVERT(NVARCHAR(50),SalesmanId) LIKE ISNULL(@salesmanId,'%')
  AND CONVERT(NVARCHAR(50),ProductId) LIKE ISNULL(@productId,'%')
  AND CONVERT(NVARCHAR(26),DocumentDateIssued,23) BETWEEN @startDate AND @endDate
GROUP BY ProductName,ProductCode,SalesmanName
	UNION
--** Delivered Orders **
SELECT SalesmanName Salesman,ProductCode,ProductName Product,0 OpeningBalance,0 IssuedStock,0 DispatchedOrders,0 MobileSales,SUM(Qty) DeliveredOrders, 0 MobileSamples,0 InventoryReturns,0 Losses
FROM [dbo].[v_D_MobileDeliveries]
WHERE CONVERT(NVARCHAR(50),DistributorId) LIKE ISNULL(@distributorId,'%')
  AND CONVERT(NVARCHAR(50),SalesmanId) LIKE ISNULL(@salesmanId,'%')
  AND CONVERT(NVARCHAR(50),ProductId) LIKE ISNULL(@productId,'%')
  AND CONVERT(NVARCHAR(26),DocumentDateIssued,23) BETWEEN @startDate AND @endDate
GROUP BY ProductName,ProductCode,SalesmanName
UNION
--** Mobile Samples **
SELECT SalesmanName Salesman,ProductCode,ProductName Product,0 OpeningBalance,0 IssuedStock,0 DispatchedOrders,0 MobileSales,0 DeliveredOrders, Sum(Qty) MobileSamples,0 InventoryReturns,0 Losses
FROM [dbo].[v_D_MobileSamples]
WHERE CONVERT(NVARCHAR(50),DistributorId) LIKE ISNULL(@distributorId,'%')
  AND CONVERT(NVARCHAR(50),SalesmanId) LIKE ISNULL(@salesmanId,'%')
  AND CONVERT(NVARCHAR(50),ProductId) LIKE ISNULL(@productId,'%')
  AND CONVERT(NVARCHAR(26),SalesDate,23) BETWEEN @startDate AND @endDate
GROUP BY ProductId,ProductName,ProductCode,SalesmanName
UNION
--** Inventory Returns **
SELECT SalesmanName Salesman,ProductCode,ProductName Product,0 OpeningBalance,0 IssuedStock,0 DispatchedOrders,0 MobileSales,0 DeliveredOrders, 0 MobileSamples,Sum(Qty) InventoryReturns,0 Losses
FROM [dbo].[v_D_InventoryReturns]
WHERE CONVERT(NVARCHAR(50),DistributorId) LIKE ISNULL(@distributorId,'%')
  AND CONVERT(NVARCHAR(50),SalesmanId) LIKE ISNULL(@salesmanId,'%')
  AND CONVERT(NVARCHAR(50),ProductId) LIKE ISNULL(@productId,'%')
  AND CONVERT(NVARCHAR(26),DocumentDateIssued,23) BETWEEN @startDate AND @endDate
GROUP BY ProductName,ProductCode,SalesmanName
UNION
--** Losses **
SELECT SalesmanName Salesman,ProductCode,ProductName Product,0 OpeningBalance,0 IssuedStock,0 DispatchedOrders,0 MobileSales,0 DeliveredOrders, 0 MobileSamples,0 InventoryReturns,SUM(LostQty) Losses
FROM [dbo].[v_D_Losses] 
WHERE CONVERT(NVARCHAR(50),SalesmanId) LIKE ISNULL(@salesmanId,'%')
  AND CONVERT(NVARCHAR(50),DistributorId) LIKE ISNULL(@distributorId,'%')
  AND CONVERT(NVARCHAR(50),ProductId) LIKE ISNULL(@productId,'%')
  AND CONVERT(NVARCHAR(26),ReturnDate,23) BETWEEN @startDate AND @endDate 
GROUP BY ProductName,ProductCode,SalesmanName
-------------------------------------------------------------




--  EXEC [dbo].[sp_D_Audit_Salesman_StockMovement_PerProduct] @startDate ='2015-01-01',@endDate='2015-02-20',@distributorId='ALL',@salesmanId='ALL' ,@productId ='ALL'




-- SELECT ProductCode,Product,0 OpeningBalance,0 IssuedStock,0 DispatchedOrders,0 MobileSales,0 DeliveredOrders, 0 MobileSamples,0 InventoryReturns,0 Losses