DROP PROCEDURE [dbo].[sp_D_Discounts_ProductDiscount]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[sp_D_Discounts_ProductDiscount]
(
@startDate as datetime,
@endDate as datetime,
@distributorId as nvarchar(50),
@productId as nvarchar(50)
)
as 
if  rtrim(ltrim(@distributorId))='ALL'  begin set @distributorId='%' end
if  rtrim(ltrim(@productId))='ALL'  begin set @productId='%' end


select cc.Name Distributor,
       doc.DocumentReference SaleReference,
       prod.Description ProductName,
       convert(nvarchar(26),doc.DocumentDateIssued,23) AS SaleDate,
       sum(discItem.Quantity) AS TargetQty,
       sum(docItems.Quantity) SoldQty,
       sum(docItems.ProductDiscount)* sum(docItems.Quantity) AS DiscountAmount,
       sum(discItem.DiscountRate) DiscountRate, 
       convert(nvarchar(26),discItem.EffectiveDate,23) AS StartDate, 
       convert(nvarchar(26),discItem.EndDate,23) AS EndDate,
       discItem.IsByQuantity        

from tblDocument doc
join tblLineItems docItems on doc.Id =  docItems. DocumentID
join tblCostCentre cc on ( doc.DocumentIssuerCostCentreId = cc. Id or doc.DocumentRecipientCostCentre = cc. Id )
join tblProduct prod on docItems.ProductID = prod.id
join tblDiscounts disc on prod.id = disc.ProductRef 
join tblDiscountItem discItem on disc.id =  discItem.DiscountId 
where doc. DocumentTypeId = 1
  and cc.CostCentreType = 2
  and doc.DocumentDateIssued between discItem.EffectiveDate and discItem.EndDate
  and (doc. OrderOrderTypeId = 1 or ( doc.OrderOrderTypeId = 3 and doc.DocumentStatusId = 99))
  and docItems.DiscountLineItemTypeId = 1  and (docItems.OrderLineItemType = 1 and docItems.ProductDiscount > 0)
  and discItem.IsByQuantity = 1 
  and (CONVERT(VARCHAR(26), doc.DocumentDateIssued,23) BETWEEN @startDate AND @endDate) 
  and (CONVERT(NVARCHAR(50), cc.Id) LIKE ISNULL(@distributorId, N'%'))
  and (CONVERT(NVARCHAR(50), prod.id) LIKE ISNULL(@productId, N'%'))
group by cc.Name,prod.Description,doc.DocumentReference,doc.DocumentDateIssued,discItem.EffectiveDate, discItem.EndDate, discItem.IsByQuantity

-- exec  [dbo].[sp_D_Discounts_ProductDiscount] @distributorId='ALL',@productId='ALL',@startDate='2014-01-01',@endDate='2014-06-06'

go