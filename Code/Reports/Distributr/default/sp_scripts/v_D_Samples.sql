IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[v_D_Samples]'))
DROP VIEW [dbo].[v_D_Samples]
GO
CREATE VIEW [dbo].[v_D_Samples]
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
           dbo.tblProduct.id AS ProductId, 
           dbo.tblProduct.Description AS ProductName,
		   dbo.tblProduct.ProductCode

FROM       dbo.tblDocument sale INNER JOIN
           dbo.tblLineItems ON sale.Id = dbo.tblLineItems.DocumentID INNER JOIN
           dbo.tblProduct ON dbo.tblLineItems.ProductID = dbo.tblProduct.id INNER JOIN
           dbo.tblCostCentre ON ((sale.DocumentIssuerCostCentreId = dbo.tblCostCentre.Id) OR (sale.DocumentIssuerCostCentreId = dbo.tblCostCentre.Id))

WHERE     (sale.Id = sale.OrderParentId) AND (dbo.tblCostCentre.CostCentreType = 2)AND
         ((sale.DocumentTypeId = 1) AND 
          (sale.OrderOrderTypeId = 1)OR((sale.OrderOrderTypeId = 3)AND(sale.DocumentStatusId = 99)))AND
          (dbo.tblLineItems.DiscountLineItemTypeId = 0 or dbo.tblLineItems.DiscountLineItemTypeId = 5)AND
          rtrim(ltrim(dbo.tblLineItems.Description)) like 'Sale (Promotion)' 
          or rtrim(ltrim(dbo.tblLineItems.Description)) like 'Sale(Free of Charge)'
          --(CONVERT(NVARCHAR(50),dbo.tblCostCentre.Id) LIKE ISNULL(@distributorId, N'%'))AND
          --(CONVERT(NVARCHAR(50),dbo.tblProduct.id) LIKE ISNULL(@productId, N'%')) AND
     --     (convert(nvarchar(50),DocumentDateIssued) BETWEEN @startDate AND @endDate)
          
     