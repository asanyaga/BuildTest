DROP VIEW [dbo].[v_D_AllTransactions]
GO
CREATE VIEW [dbo].[v_D_AllTransactions]
AS
SELECT Trans.Id TransactionId,
       Trans.DocumentTypeId, 
       Trans.DocumentReference TransactionReference, 
       CONVERT(NVARCHAR(26),Trans.DocumentDateIssued,23) TransactionDate, 
       CONVERT(NVARCHAR(8),Trans.DocumentDateIssued,108 ) TransactionTime,
       Trans.OrderOrderTypeId ,
       Trans.DocumentStatusId ,
       Trans.OrderParentId,
       TransItems.OrderLineItemType,
       TransItems.Quantity, 
       TransItems.Value, 
       TransItems.Vat,
       TransItems.ProductDiscount,
       dist.Id  DistributorId,
       dist.Name  DistributorName,
	   sm.Id SalesmanId,
	   sm.Name SalesmanName,
       prod.id AS ProductId, 
       prod.Description AS ProductName, 
       prod.Returnable, 
       prod.ReturnableType, 
       prod.DomainTypeId,
       prod.ProductCode AS ProductCode 
 

FROM   dbo.tblDocument Trans
 JOIN  dbo.tblLineItems TransItems ON Trans.Id = TransItems.DocumentID
 JOIN  dbo.tblProduct prod ON TransItems.ProductID = prod.id 
 JOIN  dbo.tblCostCentre dist ON (Trans.DocumentIssuerCostCentreId = dist.Id or Trans.DocumentRecipientCostCentre = dist.Id)
 JOIN  dbo.tblCostCentre sm ON (Trans.DocumentIssuerCostCentreId = sm.Id or Trans.DocumentRecipientCostCentre = sm.Id)
WHERE dist.CostCentreType = 2 and sm.CostCentreType = 4
--SELECT Trans.Id TransactionId,
--       Trans.DocumentTypeId, 
--       Trans.DocumentReference TransactionReference, 
--       CONVERT(NVARCHAR(26),Trans.DocumentDateIssued,23) TransactionDate, 
--       CONVERT(NVARCHAR(8),Trans.DocumentDateIssued,108 ) TransactionTime,
--       Trans.OrderOrderTypeId ,
--       Trans.DocumentStatusId ,
--       Trans.OrderParentId,
--       TransItems.OrderLineItemType,
--       TransItems.Quantity, 
--       TransItems.Value, 
--       TransItems.Vat,
--       TransItems.ProductDiscount,
--       --cc.Id  ccId,
--       --cc.Name  ccName,
--       --cc.CostCentreType ccType,
--       convert(nvarchar(50),cc.Id)  DistributorId,
--       cc.Name  DistributorName,
--	   '' SalesmanId,
--	   '' SalesmanName,
--       prod.id AS ProductId, 
--       prod.Description AS ProductName, 
--       prod.Returnable, 
--       prod.ReturnableType, 
--       prod.DomainTypeId,
--       prod.ProductCode AS ProductCode 
 

--FROM   dbo.tblDocument Trans
-- JOIN  dbo.tblLineItems TransItems ON Trans.Id = TransItems.DocumentID
-- JOIN  dbo.tblProduct prod ON TransItems.ProductID = prod.id 
-- JOIN  dbo.tblCostCentre cc ON (Trans.DocumentIssuerCostCentreId = cc.Id or Trans.DocumentRecipientCostCentre = cc.Id)
--WHERE cc.CostCentreType = 2


--UNION

--SELECT Trans.Id TransactionId,
--       Trans.DocumentTypeId, 
--       Trans.DocumentReference TransactionReference, 
--       CONVERT(NVARCHAR(26),Trans.DocumentDateIssued,23) TransactionDate, 
--       CONVERT(NVARCHAR(8),Trans.DocumentDateIssued,108 ) TransactionTime,
--       Trans.OrderOrderTypeId ,
--       Trans.DocumentStatusId ,
--       Trans.OrderParentId,
--       TransItems.OrderLineItemType,
--       TransItems.Quantity, 
--       TransItems.Value, 
--       TransItems.Vat,
--       TransItems.ProductDiscount,
--       --cc.Id  ccId,
--       --cc.Name  ccName,
--       --cc.CostCentreType ccType,
--       '' DistributorId,
--       ''  DistributorName,
--	   convert(nvarchar(50),cc.Id)  SalesmanId,
--	   cc.Name SalesmanName,
--       prod.id AS ProductId, 
--       prod.Description AS ProductName, 
--       prod.Returnable, 
--       prod.ReturnableType, 
--       prod.DomainTypeId,
--       prod.ProductCode AS ProductCode 
 


--FROM   dbo.tblDocument Trans
-- JOIN  dbo.tblLineItems TransItems ON Trans.Id = TransItems.DocumentID
-- JOIN  dbo.tblProduct prod ON TransItems.ProductID = prod.id 
-- JOIN  dbo.tblCostCentre cc ON (Trans.DocumentIssuerCostCentreId = cc.Id or Trans.DocumentRecipientCostCentre = cc.Id)
--WHERE cc.CostCentreType = 4


 GO
 

