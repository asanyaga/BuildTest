DROP PROCEDURE [dbo].[sp_D_DSS_PrevTransOutstanding]
GO
SET ANSI_NULLS ON
GO
SET ANSI_WARNINGS OFF
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[sp_D_DSS_PrevTransOutstanding]
(
@TransactionDate as datetime,
@distributorId AS NVARCHAR(50),
@salesmanId AS NVARCHAR(50),
@outletId AS NVARCHAR(50)
)
as 
if ltrim(rtrim(@outletId))='ALL'  begin set @outletId='%' end
if ltrim(rtrim(@distributorId))='ALL'  begin set @distributorId='%' end
if ltrim(rtrim(@salesmanId))='ALL'  begin set @salesmanId='%' end


BEGIN
SET ANSI_WARNINGS OFF
DECLARE @OutPaymentTable TABLE(
  DocRef nvarchar(50),
  NetValue decimal(18,2),
  ReceivedValue decimal(18,2)
 -- DocDate date
  );
  
INSERT @OutPaymentTable
SELECT 
                sales.DocumentReference as SalesRef,
                (case when sale.DocumentTypeId = 1 then SUM(Quantity * (Value + Vat)) else 0 end - (case when sale.DocumentTypeId = 1 then (coalesce(sale.SaleDiscount,0)) else 0 end)) as NetValue,
                 case when sale.DocumentTypeId = 8 then SUM(coalesce(Value ,0))  else 0 end as ReceivedValue
FROM       dbo.tblDocument sale 
           INNER JOIN dbo.tblLineItems saleItem ON sale.Id = saleItem.DocumentID
           INNER JOIN tblDocument sales on sale.DocumentParentId = sales.Id
           INNER JOIN tblCostCentre outlet on sales.OrderIssuedOnBehalfOfCC = outlet.Id

WHERE   --CONVERT(NVARCHAR(26),sales.DocumentDateIssued,23) between @TransactionDate and @TransactionDate
        (convert(nvarchar(50),sales.DocumentIssuerCostCentreId) like ISNULL(@salesmanId,'%') or convert(nvarchar(50),sales.DocumentRecipientCostCentre) like ISNULL(@salesmanId,'%'))
     AND(convert(nvarchar(50),sales.DocumentIssuerCostCentreId) like ISNULL(@distributorId,'%') or convert(nvarchar(50),sales.DocumentRecipientCostCentre) like ISNULL(@distributorId,'%'))
     AND convert(nvarchar(50),sales.OrderIssuedOnBehalfOfCC) like ISNULL(@outletId,'%')
     AND  (sales.DocumentTypeId = 1) 
     AND ((sales.OrderOrderTypeId = 1) OR (sales.OrderOrderTypeId = 3 AND sales.DocumentStatusId = 99)) 
     AND(((sale.DocumentTypeId = 1)OR(sale.DocumentTypeId = 5)OR(sale.DocumentTypeId = 8)OR(sale.DocumentTypeId = 10)))
     AND CONVERT(NVARCHAR(26),sale.DocumentDateIssued,23) < CONVERT(NVARCHAR(26),@TransactionDate,23)
GROUP BY sales.DocumentDateIssued, sale.DocumentTypeId,sale.SaleDiscount,sale.Id,sales.DocumentReference

ORDER BY sales.DocumentDateIssued desc

END;
SELECT DocRef,
      SUM(NetValue) as NetValue,
      SUM(ReceivedValue) as ReceivedValue,
      dbo.udf_D_RoundOff(SUM(NetValue)- SUM(ReceivedValue)) as OutstandingPaymentValue
	  
       
FROM @OutPaymentTable
GROUP BY DocRef
HAVING (SUM(NetValue) > SUM(ReceivedValue))
AND SUM(NetValue)- SUM(ReceivedValue)>1
--AND CONVERT(NVARCHAR(26),DocDate,23) < CONVERT(NVARCHAR(26),GETDATE(),23)
               



                      

-- Exec [dbo].[sp_D_DSS_PrevTransOutstanding] @TransactionDate = '2015-3-28', @salesmanId = 'ALL',@outletId = 'ALL',@distributorId='ALL'

-- Exec [dbo].[sp_D_DSS_PrevTransOutstanding] @salesmanId = 'ALL',@outletId = 'ALL',@distributorId='ALL'

-- Exec [dbo].[sp_D_DSS_PrevTransOutstanding] @TransactionDate = '2015-4-13', @salesmanId = 'ALL',@outletId = 'ALL',@distributorId='ALL'

-- Exec [dbo].[sp_D_DSS_PrevTransOutstanding] @TransactionDate = '2015-4-22', @salesmanId = 'e0701f7c-97c4-48e5-9631-525be25d4e17',@outletId = 'ALL',@distributorId='ALL'

GO
