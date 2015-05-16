DROP PROCEDURE [dbo].[sp_D_Reconciliation_SalesAndGrossProfit_PerDistributor]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[sp_D_Reconciliation_SalesAndGrossProfit_PerDistributor]
(
@startDate as datetime,
@endDate as datetime,
@distributorId as nvarchar(50),
@productId as nvarchar(50)
)
as
if LTRIM(RTRIM(@distributorId))='ALL' begin set @distributorId='%' end
if LTRIM(RTRIM(@productId))='ALL' begin set @productId='%' end
select distributor.Name as DistributorName,
       salesman.Name as SalesmanName,
       sales.DocumentReference,
       prod.Description as ProductName,
       saleItems.Description,
       saleItems.Quantity as StockQuantity,
       saleItems.Quantity * prod.ExFactoryPrice as StockValue,
       saleItems.Quantity *(saleItems.Value + saleItems.Vat) as GrossSaleValue,
       saleItems.Quantity * saleItems.Value  as NetSaleValue,
       sales.SaleDiscount as SaleValueDiscount,
       saleItems.ProductDiscount as ProductDiscount,
      (saleItems.ProductDiscount * saleItems.Quantity) as TotalProductDiscount
       
from tblDocument sales
inner join tblLineItems saleItems on sales.Id = saleItems.DocumentID
inner join tblProduct prod on saleItems.ProductID = prod.id
inner join tblCostCentre salesman on (sales.DocumentRecipientCostCentre = salesman.Id or sales.DocumentRecipientCostCentre = salesman.Id)
inner join tblCostCentre distributor on salesman.ParentCostCentreId = distributor.Id
where sales.DocumentTypeId = 1
  and salesman.CostCentreType = 4
  and (sales.OrderOrderTypeId = 1 or (sales.OrderOrderTypeId = 3 and sales.DocumentStatusId = 99))
  and convert(nvarchar(26),sales.DocumentDateIssued,23) between @startDate and @endDate 
  and convert(nvarchar(50),distributor.Id)like ISNULL(@distributorId,'%')
  and convert(nvarchar(50),prod.id)like ISNULL(@productId,'%')

  --and convert(nvarchar(50),salesman.Id)like ISNULL(salesman,'%')
  
 --  Exec [dbo].[sp_D_Reconciliation_SalesAndGrossProfit_PerDistributor] @startDate='2013-01-01',@endDate='2013-12-12',@distributorId='ALL',@productId='ALL'
  
  go