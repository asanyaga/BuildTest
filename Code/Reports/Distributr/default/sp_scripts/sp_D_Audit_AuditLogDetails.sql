DROP PROCEDURE [dbo].[sp_D_Audit_AuditLogDetails]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_D_Audit_AuditLogDetails]
(
@docId AS NVARCHAR(50)
)
AS 
if  RTRIM(LTRIM(@docId))='ALL'  begin set @docId='%' end


;WITH AuditLog_CTE(TransactionId,
                  DocumentTypeId,
				  TransactionReference,
				  TransactionDate,
				  TransactionTime,
				  OrderOrderTypeId,
				  DocumentStatusId,
				  OrderParentId,
				  OrderLineItemType,
				  Quantity,
				  Value,
				  Vat,
				  ProductDiscount,
				  ccId,
				  ccName,
				  salesmanId,
				  salesmanName,
				  ProductId,
				  ProductName,
				  Returnable,
				  ReturnableType,
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
  WHERE TransactionId LIKE ISNULL(@docId,'%')

  ORDER BY TransactionDate DESC


-- EXEC [dbo].[sp_D_Audit_AuditLogDetails] @docId='0f7f701b-aff3-4387-9f67-4a14847c4fe2'

 
                      