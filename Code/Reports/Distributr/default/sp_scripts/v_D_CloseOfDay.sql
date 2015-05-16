IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[v_D_CloseOfDay]'))
DROP VIEW [dbo].[v_D_CloseOfDay]
GO
CREATE VIEW [dbo].[v_D_CloseOfDay]
AS
SELECT  dist.Id AS DistributorId,
        dist.Name AS Distributor, 
        dbo.tblCostCentre.Id as SalesmanId,
        dbo.tblCostCentre.Name as Salesman,
        SUM(saleItems.Quantity) Qty, 
        sale.DocumentDateIssued SaleDate, 
        dbo.tblProduct.Id AS ProductId, 
        dbo.tblProduct.Description AS ProductName, 
		dbo.tblProduct.ProductCode,
        dbo.tblProductBrand.Id AS BrandId,
        dbo.tblProductBrand.name AS Brand

FROM    dbo.tblDocument  sale
 JOIN  dbo.tblLineItems saleItems ON sale.Id = saleItems.DocumentID 
 JOIN  dbo.tblProduct ON saleItems.ProductID = dbo.tblProduct.id 
 JOIN  dbo.tblProductBrand ON dbo.tblProduct.BrandId = dbo.tblProductBrand.id 
 JOIN  dbo.tblCostCentre ON (sale.DocumentIssuerCostCentreId = dbo.tblCostCentre.Id or sale.DocumentRecipientCostCentre = dbo.tblCostCentre.Id )
 JOIN  dbo.tblCostCentre AS dist ON dbo.tblCostCentre.ParentCostCentreId = dist.Id
WHERE (sale.DocumentTypeId = 7) 
 AND (dbo.tblCostCentre.CostCentreType = 4) 
 AND (sale.OrderOrderTypeId = 1)  
--AND (CONVERT(VARCHAR(26), sale.DocumentDateIssued, 23) BETWEEN @startDate AND @endDate) 
AND (saleItems.OrderLineItemType = 0)
GROUP BY  dist.Id,dist.Name,dbo.tblCostCentre.Id,dbo.tblCostCentre.Name,saleItems.Quantity, 
          sale.DocumentDateIssued,dbo.tblProduct.Id,dbo.tblProduct.Description,dbo.tblProduct.ProductCode,dbo.tblProductBrand.Id,
          dbo.tblProductBrand.name 
          

		  -- select * from [dbo].[v_D_CloseOfDay]

          
         
