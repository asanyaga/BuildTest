IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[v_D_MobileSales]'))
DROP VIEW [dbo].[v_D_MobileSales]
GO
CREATE VIEW [dbo].[v_D_MobileSales]
AS
select dist.Id DistributorId,
       dist.Name DistributorName,
	   sm.Id SalesmanId,
	   sm.Name SalesmanName,
       s.DocumentReference , 
       s.DocumentDateIssued, 
	   prod.id ProductId,
	   prod.Description ProductName,
	   prod.ProductCode ,
       sum(sLi.Value) Value,
       sum(sLi.Vat) Vat,
       sum(sLi.Quantity) Qty

from tblDocument s
join tblLineItems sLi on s.Id = sLi.DocumentID
join tblCostCentre dist on (s.DocumentIssuerCostCentreId = dist.Id or s.DocumentRecipientCostCentre = dist.Id)
join tblCostCentre sm on (s.DocumentIssuerCostCentreId = sm.Id or s.DocumentRecipientCostCentre = sm.Id)
join tblProduct prod on sli.ProductID = prod.id
where s.DocumentTypeId = 1 
  and s.OrderOrderTypeId = 1 
  and sLi.OrderLineItemType = 1 
  and s.DocumentStatusId = 99
  and dist.CostCentreType = 2 and sm.CostCentreType = 4
group by dist.id,dist.Name,sm.Id,sm.Name,
         s.DocumentReference,s.DocumentDateIssued, 
	     prod.id,prod.Description,prod.ProductCode

