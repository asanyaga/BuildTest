IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[v_D_InvTransfer]'))
DROP VIEW [dbo].[v_D_InvTransfer]
GO
CREATE VIEW [dbo].[v_D_InvTransfer]
AS
SELECT cntry.id CountryId,cntry.Name CountryName,
       reg.id RegionId, reg.Name RegionName,
       dist.id DistributorId, dist.Name DistributorName,
       salesman.Id SalesmanId,
	   salesman.Name Salesman,
       it.Id ITid,
       it.DocumentReference ITRef,
       it.DocumentDateIssued ItDate, 
       SUM(dbo.tblLineItems.Quantity) Qty,
	   prod.id ProductId,
	   prod.Description ProductName,
	   prod.ProductCode
FROM   dbo.tblDocument it 
 JOIN  dbo.tblLineItems ON it.Id = dbo.tblLineItems.DocumentID
 JOIN  dbo.tblProduct prod ON dbo.tblLineItems.ProductID = prod.id 
 JOIN  dbo.tblCostCentre dist on it.DocumentIssuerCostCentreId = dist.Id or it.DocumentRecipientCostCentre = dist.Id
 JOIN  dbo.tblCostCentre salesman  on it.DocumentIssuerCostCentreId = salesman.Id or it.DocumentRecipientCostCentre = salesman.Id
 JOIN  dbo.tblRegion reg ON dist.Distributor_RegionId = reg.id 
 JOIN  dbo.tblCountry cntry ON reg.Country = cntry.id

WHERE (it.DocumentTypeId = 4)
and dist.CostCentreType = 2
and salesman.CostCentreType = 4
GROUP BY cntry.id,cntry.Name,
         reg.id, reg.Name,
		 dist.id,dist.Name,
		 it.Id,it.DocumentReference,it.DocumentDateIssued,
		 salesman.Id,salesman.Name ,
		 prod.id,prod.Description, prod.ProductCode

-- SELECT * FROM [dbo].[v_D_DispatchNote]

