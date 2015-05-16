DROP VIEW [dbo].[v_D_StockReturnsToHQ]
GO
CREATE VIEW [dbo].[v_D_StockReturnsToHQ]
AS
SELECT     cntry.id as CountryId,cntry.Name as CountryName,
           reg.id as RegionId,reg.Name as RegionName,
           dist.Id AS DistributorId, 
           dist.Name AS DistributorName,
           d.DocumentDateIssued as ReturnDate, 
           d.DocumentReference as ReturnRef, 
           d.Id  as ReturnId, 
           li.ReturnsNoteReason ReturnReason, 
           p.id AS ProductId, 
           p.Description AS ProductName,
		   p.ProductCode, 
           SUM(li.Quantity-li.IAN_Actual) Qty
FROM       dbo.tblDocument d 
 JOIN      dbo.tblLineItems li ON d.Id = li.DocumentID 
 JOIN      dbo.tblProduct p ON li.ProductID = p.id 
 JOIN      dbo.tblCostCentre dist ON d.DocumentIssuerCostCentreId = dist.Id or d.DocumentRecipientCostCentre = dist.Id
 JOIN      dbo.tblRegion reg ON dist.Distributor_RegionId = reg.id 
 JOIN      dbo.tblCountry cntry ON reg.Country = cntry.id

WHERE     (d.DocumentTypeId = 9)
AND       (d.OrderOrderTypeId = 1)
AND       (dist.CostCentreType = 2)
GROUP BY  cntry.id,cntry.Name,
          reg.id,reg.Name,
		  dist.Id,dist.Name,
		  d.DocumentDateIssued,d.DocumentReference,d.Id,li.ReturnsNoteReason,
		  p.id,p.Description,p.ProductCode



--  SELECT * FROM  [dbo].[v_D_StockReturnsToHQ]