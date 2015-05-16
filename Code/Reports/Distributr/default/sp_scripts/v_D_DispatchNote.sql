IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[v_D_DispatchNote]'))
DROP VIEW [dbo].[v_D_DispatchNote]
GO
CREATE VIEW [dbo].[v_D_DispatchNote]
AS
select dist.Id DistributorId,
       dist.Name DistributorName,
	   sm.Id SalesmanId,
	   sm.Name SalesmanName,
       dn.DocumentReference ,
       dn.DocumentDateIssued ,
       prod.Description ProductName,
       prod.ProductCode,
       prod.id ProductId,
       sum(dnLi.Value) Value,
       sum(dnLi.Vat) Vat,
       sum(dnLi.Quantity) Qty,
       dn.DocumentStatusId,
       dn.DocumentTypeId,
       dn.OrderOrderTypeId
       
from   tblDocument dn
  join tblLineItems dnLi on dn.Id = dnLi.DocumentID
  join tblProduct prod on dnLi.ProductID = prod.id
 -- join tblCostCentre dist on (dn.DocumentIssuerCostCentreId = dist.Id or dn.DocumentRecipientCostCentre = dist.Id)
  join tblCostCentre sm on (dn.DocumentIssuerCostCentreId = sm.Id or dn.DocumentRecipientCostCentre = sm.Id)
  join tblCostCentre dist on sm.ParentCostCentreId = dist.Id

where  dn.DocumentTypeId = 2 
and dn.OrderOrderTypeId = 1
and sm.CostCentreType = 4
and dist.CostCentreType = 2
group by dn.DocumentReference,dn.DocumentDateIssued,dn.DocumentStatusId,dn.DocumentTypeId,dn.OrderOrderTypeId,
         prod.Description,prod.ProductCode,prod.id,
		 dist.Id,dist.Name,
		 sm.Id,sm.Name

