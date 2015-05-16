DROP VIEW [dbo].[v_D_CallProtocol_Sales]
GO
CREATE VIEW [dbo].[v_D_CallProtocol_Sales]
AS
SELECT  cntry.id AS CountryId, 
        cntry.Name AS CountryName, 
        reg.id AS RegionId, 
        reg.Name AS RegionName, 
        dist.Id AS DistributorId, 
        dist.Name AS DistributorName, 
        salesman.Id AS SalesmanId, 
        salesman.Name AS SalesmanName, 
        sale.Id AS SaleId, 
        sale.DocumentReference AS SaleRef, 
        sale.DocumentDateIssued AS SalesDate,
        sale.VisitId as sVisitId, 
        dbo.tblProduct.id sProductId, 
        dbo.tblProduct.Description sProductName, 
        saleItems.Quantity sQty, 
        dbo.tblProductBrand.id sBrandId, 
        dbo.tblProductBrand.name sBrandName
FROM    dbo.tblDocument AS sale 
 JOIN   dbo.tblLineItems AS saleItems ON sale.Id = saleItems.DocumentID 
 JOIN   dbo.tblCostCentre AS dist ON sale.DocumentIssuerCostCentreId = dist.Id OR sale.DocumentRecipientCostCentre = dist.Id 
 JOIN   dbo.tblCostCentre AS salesman ON sale.DocumentIssuerCostCentreId = salesman.Id OR sale.DocumentRecipientCostCentre = salesman.Id 
 JOIN   dbo.tblRegion AS reg ON dist.Distributor_RegionId = reg.id 
 JOIN   dbo.tblCountry AS cntry ON reg.Country = cntry.id 
 JOIN   dbo.tblProduct ON saleItems.ProductID = dbo.tblProduct.id 
 JOIN   dbo.tblProductBrand ON dbo.tblProduct.BrandId = dbo.tblProductBrand.id
WHERE (sale.OrderOrderTypeId = 1) 
  AND ((sale.DocumentTypeId = 1)OR(sale.OrderOrderTypeId = 3) AND (sale.DocumentStatusId = 99))
  AND dist.CostCentreType = 2
  AND salesman.CostCentreType = 4
GROUP BY cntry.id, cntry.Name, reg.id, reg.Name, dist.Id, dist.Name, salesman.Id, salesman.Name, sale.Id, sale.DocumentReference, sale.DocumentDateIssued, dbo.tblProduct.id, 
         dbo.tblProduct.Description, saleItems.Quantity, dbo.tblProductBrand.id, dbo.tblProductBrand.name,sale.VisitId