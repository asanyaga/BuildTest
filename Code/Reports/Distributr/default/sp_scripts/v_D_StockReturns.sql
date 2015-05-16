DROP VIEW [dbo].[v_D_StockReturns]
GO
CREATE VIEW [dbo].[v_D_StockReturns]
AS
SELECT     cntry.id as CountryId,cntry.Name as CountryName,
           reg.id as RegionId,reg.Name as RegionName,
           dist.Id AS DistributorId, 
           dist.Name AS DistributorName,
           rn.DocumentDateIssued as ReturnDate, 
           rn.DocumentReference as ReturnRef, 
           rn.Id  as ReturnId, 
           rnItems.ReturnsNoteReason ReturnReason, 
           prod.id AS ProductId, 
           prod.Description AS ProductName,
		   prod.ProductCode, 
           SUM(rnItems.Quantity) Qty
FROM       dbo.tblDocument rn 
 JOIN      dbo.tblLineItems rnItems ON rn.Id = rnItems.DocumentID 
 JOIN      dbo.tblProduct prod ON rnItems.ProductID = prod.id 
 JOIN      dbo.tblCostCentre dist ON rn.DocumentIssuerCostCentreId = dist.Id or rn.DocumentRecipientCostCentre = dist.Id
 JOIN      dbo.tblRegion reg ON dist.Distributor_RegionId = reg.id 
 JOIN      dbo.tblCountry cntry ON reg.Country = cntry.id

WHERE     (rn.DocumentTypeId = 7)
AND       (rn.OrderOrderTypeId = 2)
AND       (dist.CostCentreType = 2)
GROUP BY  cntry.id,cntry.Name,
          reg.id,reg.Name,
		  dist.Id,dist.Name,
		  rn.DocumentDateIssued,rn.DocumentReference,rn.Id,rnItems.ReturnsNoteReason,
		  prod.id,prod.Description,prod.ProductCode
           
           
         


