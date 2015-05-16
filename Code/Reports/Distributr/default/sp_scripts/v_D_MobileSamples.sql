IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[v_D_MobileSamples]'))
DROP VIEW [dbo].[v_D_MobileSamples]
GO
CREATE VIEW [dbo].[v_D_MobileSamples]
AS
SELECT     sale.Id AS DocumentId, 
           sale.DocumentReference,
           sale.DocumentDateIssued as SalesDate, 
           sale.OrderOrderTypeId, 
           sale.DocumentStatusId, 
           dbo.tblLineItems.Description, 
           dbo.tblLineItems.Quantity Qty, 
           dbo.tblCostCentre.Id AS DistributorId, 
           dbo.tblCostCentre.Name AS DistributorName, 
           dbo.tblCostCentre.CostCentreType AS DistributorCCtype, 
		   sm.Id SalesmanId,
		   sm.Name SalesmanName,
           dbo.tblProduct.id AS ProductId, 
           dbo.tblProduct.Description AS ProductName,
		   dbo.tblProduct.ProductCode

FROM       dbo.tblDocument sale 
 JOIN      dbo.tblLineItems ON sale.Id = dbo.tblLineItems.DocumentID 
 JOIN      dbo.tblProduct ON dbo.tblLineItems.ProductID = dbo.tblProduct.id 
 JOIN      dbo.tblCostCentre sm ON ((sale.DocumentIssuerCostCentreId = sm.Id) OR (sale.DocumentIssuerCostCentreId = sm.Id))
 JOIN      dbo.tblCostCentre ON sm.ParentCostCentreId = dbo.tblCostCentre.Id

WHERE     (sale.Id = sale.OrderParentId) 
     AND  (dbo.tblCostCentre.CostCentreType = 2)
	 AND   sm.CostCentreType = 4
	 AND  (sale.DocumentTypeId = 1) 
	 AND  (sale.OrderOrderTypeId = 1)
	 AND  (dbo.tblLineItems.OrderLineItemType = 1 or dbo.tblLineItems.OrderLineItemType = 2)
	 AND  (sale.DocumentStatusId = 99)
	 AND  (dbo.tblLineItems.DiscountLineItemTypeId = 0 or dbo.tblLineItems.DiscountLineItemTypeId = 5 or dbo.tblLineItems.DiscountLineItemTypeId = 6)
	 --AND     rtrim(ltrim(dbo.tblLineItems.Description)) like 'Sale (Promotion)' 
  --    or rtrim(ltrim(dbo.tblLineItems.Description)) like 'Sale(Free of Charge)'
 AND dbo.tblLineItems.Value = 0

     --   select * from  [dbo].[v_D_MobileSamples]