DROP VIEW [dbo].[v_D_Losses]
GO
CREATE VIEW [dbo].[v_D_Losses]
AS
SELECT dist.Id DistributorId,
		dist.Name DistributorName,
       dbo.tblCostCentre.Id SalesmanId, 
       dbo.tblCostCentre.Name SalesmanName,
       tblDocument.Id ReturnId,
       tblDocument.DocumentReference ReturnRef,  
       SUM(dbo.tblLineItems.Quantity) LostQty, 
       dbo.tblDocument.DocumentDateIssued ReturnDate, 
       dbo.tblProduct.Id AS ProductId, 
       dbo.tblProduct.Description AS ProductName,
	   dbo.tblProduct.ProductCode,
       dbo.tblProductBrand.Id AS BrandId,  
       dbo.tblProductBrand.name AS Brand               
FROM   dbo.tblDocument 
  JOIN dbo.tblLineItems ON dbo.tblDocument.Id = dbo.tblLineItems.DocumentID 
  JOIN dbo.tblProduct ON dbo.tblLineItems.ProductID = dbo.tblProduct.id 
  JOIN dbo.tblProductBrand ON dbo.tblProduct.BrandId = dbo.tblProductBrand.id 
  JOIN dbo.tblCostCentre ON (dbo.tblDocument.DocumentIssuerCostCentreId = dbo.tblCostCentre.Id or dbo.tblDocument.DocumentRecipientCostCentre = dbo.tblCostCentre.Id)
  JOIN dbo.tblCostCentre dist ON (dbo.tblDocument.DocumentIssuerCostCentreId = dist.Id or dbo.tblDocument.DocumentRecipientCostCentre = dist.Id)

WHERE (dbo.tblDocument.DocumentTypeId = 7)   
  AND     dist.CostCentreType = 2
  AND (dbo.tblCostCentre.CostCentreType = 4)
  AND (dbo.tblLineItems.OrderLineItemType <> 0)
      --AND CONVERT(VARCHAR(26), dbo.tblDocument.DocumentDateIssued, 23) between @startDate and  @endDate
GROUP BY dbo.tblCostCentre.Id,dbo.tblCostCentre.Name,tblDocument.Id,tblDocument.DocumentReference,
         dbo.tblDocument.DocumentDateIssued,dbo.tblProduct.Id,dbo.tblProduct.Description,dbo.tblProduct.ProductCode,dbo.tblProductBrand.Id,dbo.tblProductBrand.name,dist.Id,dist.Name


-- select  * from v_D_Losses


