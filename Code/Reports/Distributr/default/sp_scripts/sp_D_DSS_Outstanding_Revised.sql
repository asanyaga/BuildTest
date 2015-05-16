DROP PROCEDURE [dbo].[sp_D_DSS_Outstanding_Revised]
GO
SET ANSI_NULLS ON
GO
SET ANSI_WARNINGS OFF
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[sp_D_DSS_Outstanding_Revised]
(
@TransactionDate as datetime,
@salesmanId AS NVARCHAR(50),
@distributorId AS NVARCHAR(50),
@outletId AS NVARCHAR(50)
)
AS 
if ltrim(rtrim(@outletId))='ALL'  begin set @outletId='%' end
if ltrim(rtrim(@distributorId))='ALL'  begin set @distributorId='%' end
if ltrim(rtrim(@salesmanId))='ALL'  begin set @salesmanId='%' end


BEGIN
SET ANSI_WARNINGS OFF
DECLARE @OutPaymentTable TABLE(
  SaleId nvarchar(50),
  SaleRef nvarchar(50),
  ExtDocRef nvarchar(50),
  SaleDate datetime,
  PaymentDate datetime,
  CustomerCode nvarchar(50),
  CustomerName  nvarchar(50),
  SaleValueDiscount decimal(18,2),
  NetValue decimal(18,2),
  OrderValue decimal(18,2),
  InvoiceValue decimal(18,2),
  ReceivedValue decimal(18,2),
  CreditValue decimal(18,2),
  CashPaid decimal(18,2),
  ChequePaid decimal(18,2),
  MmoneyPaid decimal(18,2)
  );
  
  INSERT @OutPaymentTable
  SELECT   sales.Id,
                   sales.DocumentReference  as SaleRef,
				   sales.ExtDocumentReference as ExtDocRef, 
                   case when sale.DocumentTypeId = 5 then sales.DocumentDateIssued else 0 end as SaleDate,
				   case when sale.DocumentTypeId = 8 then sale.DocumentDateIssued else 0 end as PaymentDate,
                   outlet.Cost_Centre_Code as CustomerCode ,
                   outlet.Name as CustomerName,  
                   (coalesce(sale.SaleDiscount,0)) as SaleValueDiscount,
          (case when sale.DocumentTypeId = 1 then SUM(Quantity * (Value + Vat)) else 0 end - (coalesce(sale.SaleDiscount,0))) as NetValue,
           case when sale.DocumentTypeId = 1 then SUM(Quantity * (Value + Vat)) else 0 end as OrderValue,
           case when sale.DocumentTypeId = 5 then dbo.udf_D_RoundOff(SUM(Quantity * (Value + Vat))) else 0 end as InvoiceValue,
           case when sale.DocumentTypeId = 8 then SUM(coalesce(Value ,0))  else 0 end as ReceivedValue,
           case when sale.DocumentTypeId = 10 then SUM(coalesce(Value ,0)) else 0 end as CreditValue,
           case when sale.DocumentTypeId = 8 and saleItem.Receipt_PaymentTypeId = 1 then SUM(coalesce(Value ,0)) else 0 end as CashPaid,
           case when sale.DocumentTypeId = 8 and saleItem.Receipt_PaymentTypeId = 2 then SUM(coalesce(Value ,0)) else 0 end as ChequePaid,
           case when sale.DocumentTypeId = 8 and saleItem.Receipt_PaymentTypeId = 3 then SUM(coalesce(Value ,0)) else 0 end as MmoneyPaid
FROM       dbo.tblDocument sale 
           INNER JOIN dbo.tblLineItems saleItem ON sale.Id = saleItem.DocumentID
           INNER JOIN tblDocument sales on sale.DocumentParentId = sales.Id
           INNER JOIN tblCostCentre outlet on sales.OrderIssuedOnBehalfOfCC = outlet.Id

WHERE  -- CONVERT(NVARCHAR(26),sales.DocumentDateIssued,23) between @TransactionDate and @TransactionDate --<<REVISED 24FEB2015 Date Filter Included
          CONVERT(NVARCHAR(26),sale.DocumentDateIssued,23) <= @TransactionDate  --<<REVISED 3MARCH2015 Date Filter
	AND (convert(nvarchar(50),sales.DocumentIssuerCostCentreId) like ISNULL(@salesmanId,'%') or convert(nvarchar(50),sales.DocumentRecipientCostCentre) like ISNULL(@salesmanId,'%'))
    AND (convert(nvarchar(50),sales.DocumentIssuerCostCentreId) like ISNULL(@distributorId,'%') or convert(nvarchar(50),sales.DocumentRecipientCostCentre) like ISNULL(@distributorId,'%'))
    AND  convert(nvarchar(50),sales.OrderIssuedOnBehalfOfCC) like ISNULL(@outletId,'%')
    AND  (sales.DocumentTypeId = 1) 
    AND ((sales.OrderOrderTypeId = 1) OR (sales.OrderOrderTypeId = 3 AND sales.DocumentStatusId = 99)) 
    AND(((sale.DocumentTypeId = 1)OR (sale.DocumentTypeId = 5) OR  (sale.DocumentTypeId = 8) OR    (sale.DocumentTypeId = 10)))
  
GROUP BY sales.Id,sales.DocumentReference,sales.ExtDocumentReference, sales.DocumentDateIssued,sale.DocumentDateIssued,outlet.Cost_Centre_Code,outlet.Name ,sale.DocumentReference,sale.DocumentTypeId,saleItem.Receipt_PaymentTypeId ,sale.SaleDiscount
ORDER BY sales.DocumentDateIssued desc

END;
SELECT SaleId,SaleRef,ExtDocRef,MAX(SaleDate) SaleDate,MAX(PaymentDate) PaymentDate,CustomerCode,CustomerName,
      max(SaleValueDiscount) as SaleValueDiscount,
  	 
	  -- SaleValueDiscount SaleValueDiscount,
      --dbo.udf_D_RoundOff(SUM(NetValue)) as NetValue,
	  dbo.udf_D_RoundOff(SUM(OrderValue) - max(SaleValueDiscount)) as NetValue,
      SUM(OrderValue) as OrderValue,
      SUM(InvoiceValue) as InvoiceValue,
      SUM(ReceivedValue) as ReceivedValue,
      SUM(CreditValue) as CreditValue,
      SUM(CashPaid) as CashPaid,
      SUM(ChequePaid) as ChequePaid,
      SUM(MmoneyPaid) as MmoneyPaid,
	  SUM(InvoiceValue)-(SUM(ReceivedValue)+MAX(SaleValueDiscount)) as Balance
       
FROM @OutPaymentTable
GROUP BY SaleId,SaleRef,ExtDocRef,CustomerCode,CustomerName
HAVING 	--(CONVERT(NVARCHAR(26),MAX(SaleDate),23) < @TransactionDate) AND (CONVERT(NVARCHAR(26),MAX(PaymentDate),23) < @TransactionDate)
		dbo.udf_D_RoundOff(SUM(NetValue)) >SUM(ReceivedValue)
		--AND (MAX(PaymentDate)>MAX(SaleDate) OR (dbo.udf_D_RoundOff(SUM(NetValue)) > SUM(ReceivedValue)))
ORDER BY MAX(SaleDate) DESC

-- Exec [dbo].[sp_D_DSS_Outstanding_Revised] @TransactionDate = '2015-04-20', @salesmanId = 'ALL',@outletId = 'ALL',@distributorId='ALL'

-- Exec [dbo].[sp_D_DSS_Outstanding_Revised] @TransactionDate = '2015-04-20', @salesmanId = 'e0701f7c-97c4-48e5-9631-525be25d4e17',@outletId = 'ALL',@distributorId='ALL'

--
GO

 