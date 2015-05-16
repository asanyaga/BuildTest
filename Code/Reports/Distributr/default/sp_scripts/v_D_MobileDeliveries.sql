IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[v_D_MobileDeliveries]'))
DROP VIEW [dbo].[v_D_MobileDeliveries]
GO
CREATE VIEW [dbo].[v_D_MobileDeliveries]
AS
select  do.DocumentReference, 
        do.DocumentDateIssued,
		prod.id ProductId,
		prod.ProductCode,
		prod.Description ProductName,
		dist.Id DistributorId,
		dist.Name DistributorName,
		sm.Id SalesmanId,
		sm.Name SalesmanName,
		SUM(doLi.Value) Value,
		SUM(doLi.Vat) Vat,
		SUM(doLi.Quantity) Qty
        
from tblDocument do
join tblLineItems doLi on do.Id = doLi.DocumentID
join tblProduct prod on prod.id = doLi.ProductID
join tblCostCentre sm on (do.DocumentIssuerCostCentreId = sm.Id or do.DocumentRecipientCostCentre = sm.Id)
join tblCostCentre dist on sm.ParentCostCentreId = dist.Id

where do.DocumentTypeId =2
 and do.OrderOrderTypeId = 2
 and dist.CostCentreType = 2
 and sm.CostCentreType = 4
group by  do.DocumentReference,do.DocumentDateIssued,
          prod.id,prod.ProductCode,prod.Description,
		  sm.Id,sm.Name,
		  dist.id,dist.Name
