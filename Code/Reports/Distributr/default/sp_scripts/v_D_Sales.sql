DROP VIEW [dbo].[v_D_Sales]
GO
CREATE VIEW [dbo].[v_D_Sales]
AS
SELECT cntry.id as CountryId,cntry.Name as CountryName,
       reg.id as RegionId,reg.Name as RegionName,
       dist.Id as DistributorId,dist.Name as DistributorName,
       salesman.Id as SalesmanId,salesman.Name as SalesmanName,
       sale.Id AS SaleId, 
       sale.DocumentReference AS SaleRef, 
       sale.DocumentDateIssued as SalesDate,       
       SUM(saleItems.Quantity) as TotalSales,
	   prod.id ProductId,
	   prod.Description ProductName,
	   prod.ProductCode
FROM  dbo.tblDocument sale
 JOIN dbo.tblLineItems saleItems ON sale.Id = saleItems.DocumentID
 JOIN dbo.tblProduct prod ON saleItems.ProductID = prod.id 
 JOIN dbo.tblCostCentre dist on sale.DocumentIssuerCostCentreId = dist.Id or sale.DocumentRecipientCostCentre = dist.Id
 JOIN dbo.tblCostCentre salesman  on sale.DocumentIssuerCostCentreId = salesman.Id or sale.DocumentRecipientCostCentre = salesman.Id
 JOIN dbo.tblRegion reg ON dist.Distributor_RegionId = reg.id 
 JOIN dbo.tblCountry cntry ON reg.Country = cntry.id

WHERE (sale.OrderOrderTypeId = 1) 
  AND ((sale.DocumentTypeId = 1)OR(sale.OrderOrderTypeId = 3) AND (sale.DocumentStatusId = 99))
  AND dist.CostCentreType = 2
  AND salesman.CostCentreType = 4

GROUP BY cntry.id,cntry.Name,
         reg.id,reg.Name,
		 dist.Id,dist.Name,
		 salesman.Id,salesman.Name,
		 sale.Id,sale.DocumentReference,sale.DocumentDateIssued,
		 prod.id, prod.Description,prod.ProductCode
  
-- select * from [v_D_Sales]

