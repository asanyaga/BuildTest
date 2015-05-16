IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[v_D_GoodsReceivedNote]'))
DROP VIEW [dbo].[v_D_GoodsReceivedNote]
GO
CREATE VIEW [dbo].[v_D_GoodsReceivedNote]
AS

select dist.Id DistributorId,
       dist.Name DistributorName,
	   sm.Id SalesmanId,
	   sm.Name SalesmanName,
       po.DocumentReference GRNReference,
	   po.DocumentDateIssued TransDate,
       po.DocumentDateIssued,
	   prod.id ProductId,
       prod.Description ProductName,
	   prod.ProductCode,
       sum(poLi.Value) Value ,
       sum(poLi.Vat) Vat,
       sum(poLi.Quantity) Qty
from tblDocument po
join tblLineItems poLi on po.Id = poLi.DocumentID
join tblProduct prod on poLi.ProductID = prod.id
join tblCostCentre dist on ( po.DocumentIssuerCostCentreId = dist.Id OR po.DocumentRecipientCostCentre = dist.Id )
join tblCostCentre sm on ( po.DocumentIssuerCostCentreId = sm.Id OR po.DocumentRecipientCostCentre = sm.Id )

where po.DocumentTypeId = 3
 and  dist.CostCentreType = 2
group by po.DocumentReference,po.DocumentDateIssued,prod.Description,
		 dist.Id,dist.Name,
		 sm.Id,sm.Name,
		 prod.id,prod.Description,prod.ProductCode

	--	 SELECT * FROM [dbo].[v_D_GoodsReceivedNote]