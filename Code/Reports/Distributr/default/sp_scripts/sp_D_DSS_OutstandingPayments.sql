DROP PROCEDURE [dbo].[sp_D_DSS_Outstanding]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[sp_D_DSS_Outstanding]
(
@TransactionDate as datetime,
@salesmanId AS NVARCHAR(50),
@distributorId AS NVARCHAR(50),
@outletId AS NVARCHAR(50)
)
as 

if ltrim(rtrim(@outletId))='ALL'  begin set @outletId='%' end
if ltrim(rtrim(@distributorId))='ALL'  begin set @distributorId='%' end
if ltrim(rtrim(@salesmanId))='ALL'  begin set @salesmanId='%' end

SELECT distinct  
sales.Id,
sales.DocumentReference  as SaleRef, 
sales.DocumentDateIssued as SaleDate,
outlet.Cost_Centre_Code as CustomerCode ,
outlet.Name as CustomerName,  
sum(coalesce(sale.SaleDiscount,0)) as SaleValueDiscount,
                     --sale.DocumentReference,
          (case when sale.DocumentTypeId = 1 then SUM(Quantity * (Value + Vat)) else 0 end - sum(coalesce(sale.SaleDiscount,0))) as NetValue,
           case when sale.DocumentTypeId = 1 then SUM(Quantity * (Value + Vat)) else 0  end as OrderValue,
           case when sale.DocumentTypeId = 5 then SUM(Quantity * (Value + Vat)) else 0 end as InvoiceValue,
           case when sale.DocumentTypeId = 8 then SUM(coalesce(Value ,0))  else 0 end as ReceivedValue,
           case when sale.DocumentTypeId = 10 then SUM(coalesce(Value ,0)) else 0 end as CreditValue,
           case when sale.DocumentTypeId = 8 and saleItem.Receipt_PaymentTypeId = 1 then SUM(coalesce(Value ,0)) else 0 end as CashPaid,
           case when sale.DocumentTypeId = 8 and saleItem.Receipt_PaymentTypeId = 2 then SUM(coalesce(Value ,0)) else 0 end as ChequePaid,
           case when sale.DocumentTypeId = 8 and saleItem.Receipt_PaymentTypeId = 3 then SUM(coalesce(Value ,0)) else 0 end as MmoneyPaid
           --dbo.fn_D_GetSaleValueDiscount(@salesmanId,@salesmanId,@TransactionDate)
          -- [dbo].[fn_D_GetSaleValueDiscount] (NULL, @salesmanId,'2013-11-18') as TotalNetValue 
FROM       dbo.tblDocument sale 
           INNER JOIN dbo.tblLineItems saleItem ON sale.Id = saleItem.DocumentID
           INNER JOIN tblDocument sales on sale.DocumentParentId = sales.Id
           INNER JOIN tblCostCentre outlet on sales.OrderIssuedOnBehalfOfCC = outlet.Id

WHERE CONVERT(NVARCHAR(26),sales.DocumentDateIssued,23) BETWEEN @TransactionDate AND @TransactionDate
    AND (convert(nvarchar(50),sales.DocumentIssuerCostCentreId) like ISNULL(@salesmanId,'%') or convert(nvarchar(50),sales.DocumentIssuerCostCentreId) like ISNULL(@salesmanId,'%'))
    AND (convert(nvarchar(50),sales.DocumentIssuerCostCentreId) like ISNULL(@distributorId,'%') or convert(nvarchar(50),sales.DocumentIssuerCostCentreId) like ISNULL(@distributorId,'%'))
    
    AND convert(nvarchar(50),sales.OrderIssuedOnBehalfOfCC) like ISNULL(@outletId,'%')
    AND  (sales.DocumentTypeId = 1) 
    AND ((sales.OrderOrderTypeId = 1) OR (sales.OrderOrderTypeId = 3 AND sales.DocumentStatusId = 99)) 
    AND(((sale.DocumentTypeId = 1)OR
              (sale.DocumentTypeId = 5) OR
              (sale.DocumentTypeId = 8) OR
              (sale.DocumentTypeId = 10)))
  
GROUP BY sales.Id,sales.DocumentReference,sales.DocumentDateIssued,outlet.Cost_Centre_Code,outlet.Name ,sale.DocumentReference,sale.DocumentTypeId,saleItem.Receipt_PaymentTypeId 

--HAVING ((case when sale.DocumentTypeId = 1 then SUM(Quantity * (Value + Vat)) else 0 end - coalesce(sale.SaleDiscount,0)) - (coalesce(sale.SaleDiscount,0))) > ((case when sale.DocumentTypeId = 8 then SUM(coalesce(Value ,0)) else 0 end))
--HAVING ((case when sale.DocumentTypeId = 1 then SUM(Quantity * (Value + Vat)) end - (sale.SaleDiscount))) < ((case when sale.DocumentTypeId = 8 then SUM(Value) end)) -- NetSaleValue - ReceivedValue

  
                      
-- Exec [dbo].[sp_D_DSS_Outstanding] @TransactionDate = '2013-10-31', @salesmanId = '382acae3-1d88-4765-b998-122d199243f2',@outletId = 'a6f4ff50-c931-4a98-b7bd-a8705fda6b8e'

-- Exec [dbo].[sp_D_DSS_Outstanding] @TransactionDate = '2013-11-29', @salesmanId = 'ALL',@outletId = 'ALL',@distributorId='ALL'


GO
