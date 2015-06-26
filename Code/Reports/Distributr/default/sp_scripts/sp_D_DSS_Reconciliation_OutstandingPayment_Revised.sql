DROP PROCEDURE [dbo].[sp_D_DSS_Reconciliation_OutstandingPayment_Revised]
GO
SET ANSI_NULLS ON
GO
SET ANSI_WARNINGS OFF
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[sp_D_DSS_Reconciliation_OutstandingPayment_Revised]
(
@startDate as datetime,
@endDate datetime,
@salesmanId AS NVARCHAR(50),
@distributorId AS NVARCHAR(50),
@outletId AS NVARCHAR(50),
@routeId as nvarchar(50)
)
AS 
if ltrim(rtrim(@outletId))='ALL'  begin set @outletId='%' end
if ltrim(rtrim(@distributorId))='ALL'  begin set @distributorId='%' end
if ltrim(rtrim(@salesmanId))='ALL'  begin set @salesmanId='%' end
if ltrim(rtrim(@routeId))='ALL'  begin set @routeId='%' end

BEGIN
SET ANSI_WARNINGS OFF
DECLARE @OutPaymentTable TABLE(
  SaleId nvarchar(50),
  SaleRef nvarchar(50),
  SaleDate date,
  CustomerCode nvarchar(50),
  CustomerName  nvarchar(50),
  DistributorName nvarchar(50),
  SalesmanName nvarchar(50),
  RouteName nvarchar(50),
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
                   sales.DocumentDateIssued as SaleDate,
                   outlet.Cost_Centre_Code as CustomerCode ,
                   outlet.Name as CustomerName, 
                   distributor.Name as DistributorName,
                   salesman.Name as SalesmanName,
                   route.Name as RouteName,
                   (coalesce(sale.SaleDiscount,0)) as SaleValueDiscount,
          (case when sale.DocumentTypeId = 1 then SUM(Quantity * (Value + Vat)) else 0 end - (coalesce(sale.SaleDiscount,0))) as NetValue,
           case when sale.DocumentTypeId = 1 then SUM(Quantity * (Value + Vat)) else 0  end as OrderValue,
           case when sale.DocumentTypeId = 5 then SUM(Quantity * (Value + Vat)) else 0 end as InvoiceValue,
           case when sale.DocumentTypeId = 8 then SUM(coalesce(Value ,0))  else 0 end as ReceivedValue,
           case when sale.DocumentTypeId = 10 then SUM(coalesce(Value ,0)) else 0 end as CreditValue,
           case when sale.DocumentTypeId = 8 and saleItem.Receipt_PaymentTypeId = 1 then SUM(coalesce(Value ,0)) else 0 end as CashPaid,
           case when sale.DocumentTypeId = 8 and saleItem.Receipt_PaymentTypeId = 2 then SUM(coalesce(Value ,0)) else 0 end as ChequePaid,
           case when sale.DocumentTypeId = 8 and saleItem.Receipt_PaymentTypeId = 3 then SUM(coalesce(Value ,0)) else 0 end as MmoneyPaid
FROM       dbo.tblDocument sale 
           INNER JOIN dbo.tblLineItems saleItem ON sale.Id = saleItem.DocumentID
           INNER JOIN tblDocument sales on sale.DocumentParentId = sales.Id
           INNER JOIN tblCostCentre salesman on (sales.DocumentIssuerCostCentreId = salesman.Id or sales.DocumentRecipientCostCentre = salesman.Id)
           INNER JOIN tblCostCentre distributor on (sales.DocumentIssuerCostCentreId = distributor.Id or sales.DocumentRecipientCostCentre = distributor.Id)
           INNER JOIN tblCostCentre outlet on sales.OrderIssuedOnBehalfOfCC = outlet.Id
           INNER JOIN tblRoutes route on outlet.RouteId = route.RouteID

WHERE    (sales.DocumentTypeId = 1) 
    AND ((sales.OrderOrderTypeId = 1) OR (sales.OrderOrderTypeId = 3 AND sales.DocumentStatusId = 99)) 
    AND salesman.CostCentreType = 4
    AND distributor.CostCentreType = 2
    AND(((sale.DocumentTypeId = 1)OR (sale.DocumentTypeId = 5) OR (sale.DocumentTypeId = 8) OR (sale.DocumentTypeId = 10)))
    AND CONVERT(NVARCHAR(26),sales.DocumentDateIssued,23) between @startDate and @endDate
    AND (convert(nvarchar(50),sales.DocumentIssuerCostCentreId) like ISNULL(@salesmanId,'%') or convert(nvarchar(50),sales.DocumentRecipientCostCentre) like ISNULL(@salesmanId,'%'))
    AND (convert(nvarchar(50),sales.DocumentIssuerCostCentreId) like ISNULL(@distributorId,'%') or convert(nvarchar(50),sales.DocumentRecipientCostCentre) like ISNULL(@distributorId,'%'))
    AND  convert(nvarchar(50),sales.OrderIssuedOnBehalfOfCC) like ISNULL(@outletId,'%')
    AND  convert(nvarchar(50),route.RouteID) like ISNULL(@routeId,'%')    
  
GROUP BY sales.Id,sales.DocumentReference,sales.DocumentDateIssued,outlet.Cost_Centre_Code,outlet.Name ,sale.DocumentReference,sale.DocumentTypeId,saleItem.Receipt_PaymentTypeId ,sale.SaleDiscount,salesman.Name,distributor.Name,route.Name 
--ORDER BY sales.DocumentDateIssued desc

END;
SELECT SaleId,SaleRef,SaleDate,CustomerCode,CustomerName,DistributorName,SalesmanName,RouteName,
      sum(SaleValueDiscount) as SaleValueDiscount,
      SUM(NetValue) as NetValue,
      SUM(OrderValue) as OrderValue,
      SUM(InvoiceValue) as InvoiceValue,
      SUM(ReceivedValue) as ReceivedValue,
      SUM(CreditValue) as CreditValue,
      SUM(CashPaid) as CashPaid,
      SUM(ChequePaid) as ChequePaid,
      SUM(MmoneyPaid) as MmoneyPaid
       
FROM @OutPaymentTable
WHERE CONVERT(NVARCHAR(26),SaleDate,23) between @startDate and @endDate
GROUP BY SaleId,SaleRef,SaleDate,CustomerCode,CustomerName,DistributorName,SalesmanName,RouteName
HAVING (SUM(NetValue) > SUM(ReceivedValue))
--ORDER BY SaleDate DESC

-- Exec [dbo].[sp_D_DSS_Reconciliation_OutstandingPayment_Revised] @startDate = '2014-01-01',@endDate='2015-12-12', @salesmanId = 'ALL',@outletId = 'ALL',@distributorId='ALL',@routeId='ALL'


GO

 