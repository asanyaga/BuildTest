DROP PROCEDURE [dbo].[sp_D_Audit_AuditLog]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_D_Audit_AuditLog]
(
@startDate AS DATE,
@endDate AS DATE,
@distributorId AS NVARCHAR(50),
@salesmanId AS NVARCHAR(50)
)
AS 
if  RTRIM(LTRIM(@distributorId))='ALL'  begin set @distributorId='%' end
if  RTRIM(LTRIM(@salesmanId))='ALL'  begin set @salesmanId='%' end


;WITH AuditLog_CTE(TransactionId,
                   DocumentTypeId,
				   TransactionReference,
				   TransactionDate,
				   TransactionTime,
				   OrderOrderTypeId,
				   DocumentStatusId,
				   OrderParentId,
				   OrderLineItemType,
				   Quantity,Value,Vat,ProductDiscount,
				   --ccId,ccName,ccType,
				   DistributorId,DistributorName,
				   SalesmanId,SalesmanName,
				   ProductId,ProductName,
				   Returnable,ReturnableType,
				   DomainTypeId,
				   ProductCode,
				   TransactionType,
				   TransactionTypeId )
AS(
SELECT t.*,'Sales' TransactionType,1 TransactionTypeId 
FROM [dbo].[v_D_AllTransactions] t
WHERE( t.OrderOrderTypeId = 1) 
  AND ((t.DocumentTypeId = 1)OR(t.OrderOrderTypeId = 3) AND (t.DocumentStatusId = 99))

   UNION ALL
   
SELECT t.*,'Orders' TransactionType,2 TransactionTypeId 
FROM [dbo].[v_D_AllTransactions] t
WHERE t.OrderOrderTypeId = 3
  AND t.DocumentTypeId = 1
 
   UNION ALL
  
SELECT t.*,'Purchase Orders' TransactionType,3 TransactionTypeId 
FROM [dbo].[v_D_AllTransactions] t
WHERE t.OrderOrderTypeId = 2
  AND t.DocumentTypeId = 1 
  AND t.DocumentStatusId = 99 
  AND (t.TransactionId = t.OrderParentId)
   
   UNION ALL
  
SELECT t.*,'Close Of Day' TransactionType,4 TransactionTypeId 
FROM [dbo].[v_D_AllTransactions] t
WHERE t.OrderOrderTypeId = 1
  AND t.DocumentTypeId = 7 
  AND t.OrderLineItemType = 0
     
   UNION ALL
  
SELECT t.*,'Losses' TransactionType,5 TransactionTypeId 
FROM [dbo].[v_D_AllTransactions] t
WHERE t.DocumentTypeId = 7 
  AND t.OrderLineItemType <> 0
      
   UNION ALL
  
SELECT t.*,'Inventory Transfer' TransactionType,6 TransactionTypeId 
FROM [dbo].[v_D_AllTransactions] t
WHERE t.DocumentTypeId = 5 
      
   UNION ALL
  
SELECT t.*,'Stock Returns' TransactionType,6 TransactionTypeId 
FROM [dbo].[v_D_AllTransactions] t
WHERE t.DocumentTypeId = 7
  AND t.OrderOrderTypeId = 2
  )
   
  SELECT * FROM AuditLog_CTE
  WHERE TransactionDate BETWEEN @startDate AND @endDate 
  and convert(nvarchar(50),DistributorId) like isnull(@distributorId,'%')
  and convert(nvarchar(50),SalesmanId) like isnull(@salesmanId,'%') 

   --AND ((ccType = 2 AND CONVERT(NVARCHAR(50),ccId) LIKE ISNULL(@distributorId,'%'))
   -- OR  (ccType = 4 AND CONVERT(NVARCHAR(50),ccId) LIKE ISNULL(@salesmanId,'%')))

  --ORDER BY TransactionDate DESC
 
-- EXEC  [dbo].[sp_D_Audit_AuditLog] @startDate='2014-10-01',@endDate='2014-10-08',@distributorId='E8C61376-8FD2-46B8-B293-952D7B86F54F',@salesmanId='F4748636-28C7-4DBB-BAE1-3D30566763B6'                  

GO