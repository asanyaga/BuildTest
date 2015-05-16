DROP VIEW [dbo].[v_D_PurchaseOrders]
GO
CREATE VIEW [dbo].[v_D_PurchaseOrders]
AS
SELECT     purch.Id AS PurchaseId, 
           purch.DocumentReference AS PurchaseRef,
           purch.DocumentDateIssued PoDate,
           dist.Id AS DistributorId, 
           dist.Name AS Distributor, 
           SUM(purchItems.Quantity) as TotalPurchaseOrders, 
           dbo.tblProduct.id AS ProductId, 
           dbo.tblProduct.Description AS ProductName, 
           purch.DocumentReference, 
           dbo.tblProductBrand.id AS BrandId, 
           dbo.tblProductBrand.name AS Brand 

FROM      dbo.tblDocument purch 
 JOIN     dbo.tblLineItems purchItems ON purch.Id = purchItems.DocumentID 
 JOIN     dbo.tblCostCentre dist ON purch.DocumentIssuerCostCentreId = dist.Id 
 JOIN     dbo.tblProduct ON purchItems.ProductID = dbo.tblProduct.id 
 JOIN     dbo.tblProductBrand ON dbo.tblProduct.BrandId = dbo.tblProductBrand.id       
             
WHERE     (dist.CostCentreType = 2)
          AND (purch.OrderOrderTypeId = 2) 
          AND (purch.DocumentTypeId = 1) 
          AND (purch.DocumentStatusId = 99)
          AND (purch.Id = purch.OrderParentId)
GROUP BY  purch.Id,purch.DocumentReference,dist.Id,dist.Name,dbo.tblProduct.id,dbo.tblProduct.Description, 
           purch.DocumentReference,dbo.tblProductBrand.id,dbo.tblProductBrand.name,purch.DocumentDateIssued  
           
          
