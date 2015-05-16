--IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[v_D_StockTake]') AND type in (N'P', N'PC'))
DROP VIEW [dbo].[v_D_StockTake]
GO
CREATE VIEW [dbo].[v_D_StockTake]
AS
SELECT     dbo.tblDocument.Id AS DocumentId, 
           dbo.tblDocument.DocumentReference, 
           dbo.tblDocument.DocumentTypeId, 
           dbo.tblCostCentre.Id AS DistributorId, 
           dbo.tblCostCentre.Name AS DistributorName, 
           dbo.tblProduct.id AS ProductId, 
           dbo.tblProduct.Description AS ProductName, 
		   dbo.tblProduct.ProductCode,
           dbo.tblDocument.DocumentDateIssued AS StockTakeDate, 
           dbo.tblLineItems.Description AS Reasons, 
           SUM(dbo.tblLineItems.IAN_Actual) AS ActualQty
FROM       dbo.tblDocument 
 JOIN      dbo.tblCostCentre ON dbo.tblDocument.DocumentIssuerCostCentreId = dbo.tblCostCentre.Id 
 JOIN      dbo.tblLineItems ON dbo.tblDocument.Id = dbo.tblLineItems.DocumentID 
 JOIN      dbo.tblProduct ON dbo.tblLineItems.ProductID = dbo.tblProduct.id
WHERE     (dbo.tblDocument.DocumentTypeId = 9) 
 AND      (dbo.tblDocument.OrderOrderTypeId = 4)
GROUP BY  dbo.tblDocument.Id, dbo.tblDocument.DocumentReference,dbo.tblDocument.DocumentTypeId,dbo.tblCostCentre.Id,dbo.tblCostCentre.Name,
           dbo.tblProduct.id,dbo.tblProduct.Description,dbo.tblProduct.ProductCode,dbo.tblDocument.DocumentDateIssued,dbo.tblLineItems.Description
 
 
-- SELECT ProductName,ActualQty Stock, StockTakeDate FROM [dbo].[v_D_StockTake]