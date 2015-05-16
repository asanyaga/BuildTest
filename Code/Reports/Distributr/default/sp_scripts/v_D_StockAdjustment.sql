--IF NOT EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'v_D_StockAdjustment')
--   exec('CREATE PROCEDURE [dbo].[v_D_StockAdjustment] AS BEGIN SET NOCOUNT ON; END')
--GO

--ALTER PROCEDURE [dbo].[v_D_StockAdjustment] 
--AS


----IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[v_D_StockAdjustment]') AND type in (N'P', N'PC'))
DROP VIEW [dbo].[v_D_StockAdjustment]
GO

CREATE VIEW [dbo].[v_D_StockAdjustment]
AS

SELECT  dbo.tblDocument.DocumentDateIssued AS AdjustmentDate, 
        dbo.tblLineItems.ProductID ProdictId, 
     	dbo.tblProduct.Description ProductName,  
		dbo.tblProduct.ProductCode  ,
		dbo.tblLineItems.Quantity ExpectedQty, 
		dbo.tblLineItems.IAN_Actual ActualQty, 
		dbo.tblLineItems.Description AS Reason
FROM    dbo.tblDocument
 JOIN   dbo.tblLineItems ON dbo.tblDocument.Id = dbo.tblLineItems.DocumentID 
 JOIN   dbo.tblProduct ON dbo.tblLineItems.ProductID = dbo.tblProduct.id
WHERE  (dbo.tblDocument.DocumentTypeId = 9)
and dbo.tblDocument.DocumentRecipientCostCentre like cast(cast(0 as binary) as uniqueidentifier)


-- SELECT * FROM v_D_StockAdjustment