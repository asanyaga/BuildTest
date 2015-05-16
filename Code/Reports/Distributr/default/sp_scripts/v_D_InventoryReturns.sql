IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[v_D_InventoryReturns]'))
DROP VIEW [dbo].[v_D_InventoryReturns]
GO
CREATE VIEW [dbo].[v_D_InventoryReturns]
AS

select rt.DocumentDateIssued,
       dist.Id DistributorId,
	   dist.Name DistributorName,
	   sm.Id SalesmanId,
	   sm.Name SalesmanName,
       prod.id ProductId,
       prod.Description ProductName,
	   prod.ProductCode,
	   sum(rtLi.Quantity) Qty,
	   sum(rtLi.Value) Value,
	   sum(rtLi.Vat) Vat
from tblDocument rt
join tblLineItems rtLi on rt.Id = rtLi.DocumentID
join tblProduct prod on rtLi.ProductID = prod.id
join tblCostCentre dist on (rt.DocumentIssuerCostCentreId = dist.id or rt.DocumentRecipientCostCentre = dist.Id)
join tblCostCentre sm on (rt.DocumentIssuerCostCentreId = sm.id or rt.DocumentRecipientCostCentre = sm.Id)

where rt.DocumentTypeId = 7
and dist.CostCentreType = 2
and sm.CostCentreType = 4
and rtLi.OrderLineItemType = 0
group by rt.DocumentDateIssued,dist.Id ,dist.Name ,sm.Id ,sm.Name ,prod.id ,prod.Description ,prod.ProductCode

-- SELECT * FROM [dbo].[v_D_InventoryReturns]