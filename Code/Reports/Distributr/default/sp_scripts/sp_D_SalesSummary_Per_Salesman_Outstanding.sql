
DROP PROCEDURE sp_D_SalesSummary_Per_Salesman_Outstanding
GO
SET ANSI_NULLS ON
GO
SET ANSI_WARNINGS OFF
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure sp_D_SalesSummary_Per_Salesman_Outstanding
(
@startDate as datetime, 
@endDate datetime,
@salesmanId AS NVARCHAR(50),
@distributorID AS NVARCHAR(50),
@outletId AS NVARCHAR(50),
@routeId as nvarchar(50)
)
AS 
if ltrim(rtrim(@outletId))='ALL'  begin set @outletId='%' end
if ltrim(rtrim(@distributorID))='ALL'  begin set @distributorID='%' end
if ltrim(rtrim(@salesmanId))='ALL'  begin set @salesmanId='%' end
if ltrim(rtrim(@routeId))='ALL'  begin set @routeId='%' end




--;WITH sale_cte AS(
--SELECT	sales.DocumentReference SalesRef,
--		sales.ExtDocumentReference,
--		sm.Name AS SalesmanNames,
--		dbo.tblRoutes.Name AS RouteNames,
--		outlet.Cost_Centre_Code AS OutletCode,
--		outlet.Name AS OutletName, 
--		sales.DocumentDateIssued SalesDate,
--		sales.SaleDiscount AS discount,
--		case when (invoice.DocumentTypeId = 5)  then ((invoiceItems.Value + invoiceItems.vat) * invoiceItems.Quantity) else 0 end as InvoiceValue,
--		case when invoice.DocumentTypeId = 8 then invoiceItems.Value else 0 end as ReceiptValue,
--		case when (invoice.DocumentTypeId = 8 AND invoiceItems.Receipt_PaymentTypeId= 1) then invoiceItems.Value else 0 end as CashPaid,
--		case when (invoice.DocumentTypeId = 8 AND invoiceItems.Receipt_PaymentTypeId= 2) then invoiceItems.Value else 0 end as Cheque,
--		case when (invoice.DocumentTypeId = 8 AND invoiceItems.Receipt_PaymentTypeId= 3) then invoiceItems.Value else 0 end as Mmoney

--FROM	dbo.tblDocument AS sales INNER JOIN
--        dbo.tblDocument AS invoice ON sales.Id = invoice.DocumentParentId INNER JOIN
--        dbo.tblLineItems AS saleItems ON sales.Id = saleItems.DocumentID INNER JOIN
--        dbo.tblLineItems AS invoiceItems ON invoice.Id = invoiceItems.DocumentID
--JOIN    dbo.tblCostCentre AS sm ON sales.DocumentRecipientCostCentre = sm.Id OR sales.DocumentIssuerCostCentreId = sm.Id
--JOIN    dbo.tblCostCentre AS dist ON sales.DocumentRecipientCostCentre = dist.Id OR sales.DocumentIssuerCostCentreId = dist.Id
--JOIN    dbo.tblCostCentre AS outlet ON sales.OrderIssuedOnBehalfOfCC = outlet.Id INNER JOIN
--        dbo.tblRoutes ON outlet.RouteId = dbo.tblRoutes.RouteID

--WHERE	sales.DocumentTypeId = 1
--		AND (sales.OrderOrderTypeId = 1 or (sales.OrderOrderTypeId = 3 and sales.DocumentStatusId = 99))
--		AND (invoice.DocumentTypeId = 5 or invoice.DocumentTypeId = 8)
--		AND	sm.CostCentreType = 4
--	    AND	dist.CostCentreType = 2
--		AND CONVERT(NVARCHAR(26),sales.DocumentDateIssued,23) <= @endDate --Added on 2april2015 as requsted by CTO and CSO for FCL
--		--AND CONVERT(NVARCHAR(26),sales.DocumentDateIssued,23) between @startDate and @endDate --ommmited on 2april2015 as requsted by CTO and CSO for FCL
--		AND (convert(nvarchar(50),sm.Id) like ISNULL(@salesmanId,'%') )
--		AND (convert(nvarchar(50),sales.DocumentIssuerCostCentreId) like ISNULL(@distributorID,'%') OR convert(nvarchar(50),sales.DocumentRecipientCostCentre) like ISNULL(@distributorID,'%'))
--		AND  convert(nvarchar(50),outlet.Id) like ISNULL(@outletId,'%')
--		AND  convert(nvarchar(50),dbo.tblRoutes.RouteID) like ISNULL(@routeId,'%')

--GROUP BY	sales.DocumentReference ,sales.ExtDocumentReference,sm.Name,dbo.tblRoutes.Name,outlet.Cost_Centre_Code,outlet.Name, sales.DocumentDateIssued ,
--			invoice.DocumentReference ,invoice.DocumentDateIssued ,invoiceItems.Receipt_PaymentTypeId,invoice.DocumentTypeId,invoiceItems.Quantity,invoiceItems.Value,invoiceItems.Vat,sales.SaleDiscount 

--)

--SELECT	SalesRef,
--		ExtDocumentReference,
--		SalesmanNames,
--		RouteNames,
--		OutletCode,
--		OutletName,
--		SalesDate,
--		discount,
--		sum(InvoiceValue) TotalInvoiceValue,
--		SUM(CashPaid) AS CashPaid,
--		SUM(Cheque) AS Cheque,
--		SUM(Mmoney) AS Mmoney,
--		--sum(InvoiceValue) - sum(ReceiptValue) as TotalDue
--		case when (sum(InvoiceValue) - (discount + sum(ReceiptValue))) % 1 < 0.04 then floor(sum(InvoiceValue) - (discount + sum(ReceiptValue))) else ceiling(sum(InvoiceValue) - (discount + sum(ReceiptValue))) end  AS Balance

--FROM	sale_cte

--GROUP BY SalesDate,SalesRef,ExtDocumentReference,SalesmanNames,RouteNames,OutletCode,OutletName,discount

--HAVING case when (sum(InvoiceValue) - (discount + sum(ReceiptValue))) % 1 < 0.04 then floor(sum(InvoiceValue) - (discount + sum(ReceiptValue))) else ceiling(sum(InvoiceValue) - (discount + sum(ReceiptValue))) end  > 1

--GO

---- Exec sp_D_SalesSummary_Per_Salesman_Outstanding @startDate = '2014-02-25',@endDate='2015-2-25', @salesmanId = 'ALL',@outletId = 'ALL',@distributorID='ALL',@routeId='ALL'

---- Exec sp_D_SalesSummary_Per_Salesman_Outstanding @endDate='2015-3-31',@salesmanId = 'ALL',@outletId = 'ALL',@distributorID='ALL',@routeId='ALL'




BEGIN
SET ANSI_WARNINGS OFF
DECLARE @OutPaymentTable TABLE(
  SaleId nvarchar(50),
  SalesmanNames nvarchar(50),
  RouteNames nvarchar(50),
  SalesRef nvarchar(50),
  ExtDocumentReference nvarchar(50),
  SaleDate date,
  OutletCode nvarchar(50),
  OutletName  nvarchar(50),
  SaleValueDiscount decimal(18,2),
  NetValue decimal(18,2),
  OrderValue decimal(18,2),
  TotalInvoiceValue decimal(18,2),
  ReceivedValue decimal(18,2),
  CreditValue decimal(18,2),
  CashPaid decimal(18,2),
  Cheque decimal(18,2),
  Mmoney decimal(18,2)
  );
  
  INSERT @OutPaymentTable
  SELECT  sales.Id,
					sm.Name AS SalesmanNames,
					dbo.tblRoutes.Name AS RouteNames,
                   sales.DocumentReference  as SalesRef,
				   sales.ExtDocumentReference as ExtDocumentReference, 
                   sales.DocumentDateIssued as SaleDate,
                   outlet.Cost_Centre_Code as OutletCode ,
                   outlet.Name as OutletName,  
                   (coalesce(sale.SaleDiscount,0)) as SaleValueDiscount,
          (case when sale.DocumentTypeId = 1 then SUM(Quantity * (Value + Vat)) else 0 end - (coalesce(sale.SaleDiscount,0))) as NetValue,
           case when sale.DocumentTypeId = 1 then SUM(Quantity * (Value + Vat)) else 0  end as OrderValue,
           case when sale.DocumentTypeId = 5 then dbo.udf_D_RoundOff(SUM(Quantity * (Value + Vat))) else 0 end as TotalInvoiceValue,
           case when sale.DocumentTypeId = 8 then SUM(coalesce(Value ,0))  else 0 end as ReceivedValue,
           case when sale.DocumentTypeId = 10 then SUM(coalesce(Value ,0)) else 0 end as CreditValue,
           case when sale.DocumentTypeId = 8 and saleItem.Receipt_PaymentTypeId = 1 then SUM(coalesce(Value ,0)) else 0 end as CashPaid,
           case when sale.DocumentTypeId = 8 and saleItem.Receipt_PaymentTypeId = 2 then SUM(coalesce(Value ,0)) else 0 end as Cheque,
           case when sale.DocumentTypeId = 8 and saleItem.Receipt_PaymentTypeId = 3 then SUM(coalesce(Value ,0)) else 0 end as Mmoney
FROM       dbo.tblDocument sale 
           INNER JOIN dbo.tblLineItems saleItem ON sale.Id = saleItem.DocumentID
           INNER JOIN tblDocument sales on sale.DocumentParentId = sales.Id
		   INNER JOIN dbo.tblCostCentre AS dist ON sale.DocumentIssuerCostCentreId = dist.Id
		   INNER JOIN dbo.tblCostCentre AS sm ON sales.DocumentRecipientCostCentre = sm.Id OR sales.DocumentIssuerCostCentreId = sm.Id
           INNER JOIN tblCostCentre outlet on sales.OrderIssuedOnBehalfOfCC = outlet.Id
		   INNER JOIN dbo.tblRoutes ON outlet.RouteId = dbo.tblRoutes.RouteID

WHERE  CONVERT(NVARCHAR(26),sales.DocumentDateIssued,23) between @startDate and @endDate ----<<REVISED 24FEB2015 Date Filter Included
          --CONVERT(NVARCHAR(26),sales.DocumentDateIssued,23) <= @endDate  --<<REVISED 3MARCH2015 Date Filter
	AND    (convert(nvarchar(50),sales.DocumentIssuerCostCentreId) like ISNULL(@salesmanId,'%') or convert(nvarchar(50),sales.DocumentRecipientCostCentre) like ISNULL(@salesmanId,'%'))
    AND (convert(nvarchar(50),sales.DocumentIssuerCostCentreId) like ISNULL(@distributorId,'%') or convert(nvarchar(50),sales.DocumentRecipientCostCentre) like ISNULL(@distributorId,'%'))
    AND  convert(nvarchar(50),sales.OrderIssuedOnBehalfOfCC) like ISNULL(@outletId,'%')
	AND  convert(nvarchar(50),dbo.tblRoutes.RouteID) like ISNULL(@routeId,'%')
	AND	sm.CostCentreType = 4
    AND  (sales.DocumentTypeId = 1) 
    AND ((sales.OrderOrderTypeId = 1) OR (sales.OrderOrderTypeId = 3 AND sales.DocumentStatusId = 99)) 
    AND(((sale.DocumentTypeId = 1)OR
              (sale.DocumentTypeId = 5) OR
              (sale.DocumentTypeId = 8) OR
              (sale.DocumentTypeId = 10)))
  
GROUP BY sales.Id,sm.Name,dbo.tblRoutes.Name,sales.DocumentReference,sales.ExtDocumentReference, sales.DocumentDateIssued,outlet.Cost_Centre_Code,outlet.Name ,sale.DocumentReference,sale.DocumentTypeId,saleItem.Receipt_PaymentTypeId ,sale.SaleDiscount
ORDER BY sales.DocumentDateIssued desc

END;
SELECT SaleId,SalesmanNames,RouteNames,SalesRef,ExtDocumentReference,SaleDate,OutletCode,OutletName,
      max(SaleValueDiscount) as SaleValueDiscount,
  	 
	  -- SaleValueDiscount SaleValueDiscount,
      --dbo.udf_D_RoundOff(SUM(NetValue)) as NetValue,
	  dbo.udf_D_RoundOff(SUM(OrderValue) - max(SaleValueDiscount)) as NetValue,
      SUM(OrderValue) as OrderValue,
      SUM(TotalInvoiceValue) as TotalInvoiceValue,
      SUM(ReceivedValue) as ReceivedValue,
      SUM(CreditValue) as CreditValue,
      SUM(CashPaid) as CashPaid,
      SUM(Cheque) as Cheque,
      SUM(Mmoney) as Mmoney
       
FROM @OutPaymentTable
GROUP BY SaleId,SalesmanNames,RouteNames,SalesRef,ExtDocumentReference,SaleDate,OutletCode,OutletName
HAVING (dbo.udf_D_RoundOff(SUM(NetValue)) > SUM(ReceivedValue))
ORDER BY SaleDate DESC

-- Exec sp_D_SalesSummary_Per_Salesman_Outstanding @startDate = '2015-03-01',@endDate = '2015-03-31', @salesmanId = 'ALL',@outletId = 'ALL',@distributorId='ALL',@routeId='ALL'


GO
 