
DROP PROCEDURE sp_D_PaymentSummary_Per_Salesman
GO
SET ANSI_NULLS ON
GO
SET ANSI_WARNINGS OFF
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure sp_D_PaymentSummary_Per_Salesman
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



;WITH Payment_CTE as (
SELECT	sales.Id, 
		sm.Name AS SalesmanName,
		sales.DocumentReference AS saleReference,
		sales.ExtDocumentReference,
		dbo.tblRoutes.Name AS RouteName, 
		sales.DocumentDateIssued AS SaleDate, 
		purchases.DocumentDateIssued AS PaymentDate, 
		sales.DocumentTypeId,
		purchases.DocumentTypeId AS DoctypeId,
		outlet.Cost_Centre_Code AS OutletCode,
		outlet.Name AS OutletName,  
		SUM(saleItems.Quantity)Quantity, 
		SUM(saleItems.Value)Value, 
		SUM(saleItems.Vat)Vat,
		dbo.udf_D_RoundOff(SUM(saleItems.Quantity * (saleItems.Value + saleItems.Vat))) AS InvoiceValue, 
		sales.SaleDiscount AS discount,
		case when purchaseItems.Receipt_PaymentTypeId = 1 then (COALESCE(purchaseItems.value,0)) else 0 end as CashPaid,
        case when purchaseItems.Receipt_PaymentTypeId = 2 then (COALESCE(purchaseItems.value,0)) else 0 end as ChequePaid,
        case when purchaseItems.Receipt_PaymentTypeId = 3 then (COALESCE(purchaseItems.value,0)) else 0 end as MmoneyPaid,
		(SUM(saleItems.Quantity * (saleItems.Value + saleItems.Vat)))-(purchaseItems.Value + sales.SaleDiscount) AS balance
				

FROM	dbo.tblDocument AS sales INNER JOIN
        dbo.tblDocument AS purchases ON sales.Id = purchases.DocumentParentId INNER JOIN
        dbo.tblLineItems AS saleItems ON sales.Id = saleItems.DocumentID INNER JOIN
        dbo.tblLineItems AS purchaseItems ON purchases.Id = purchaseItems.DocumentID
JOIN    dbo.tblCostCentre AS sm ON sales.DocumentRecipientCostCentre = sm.Id OR sales.DocumentIssuerCostCentreId = sm.Id
JOIN    dbo.tblCostCentre AS dist ON sales.DocumentRecipientCostCentre = dist.Id OR sales.DocumentIssuerCostCentreId = dist.Id
JOIN    dbo.tblCostCentre AS outlet ON sales.OrderIssuedOnBehalfOfCC = outlet.Id INNER JOIN
        dbo.tblRoutes ON outlet.RouteId = dbo.tblRoutes.RouteID

WHERE		sales.DocumentTypeId = 1 
		AND purchases.DocumentTypeId = 8
		AND	sm.CostCentreType = 4
	    AND	dist.CostCentreType = 2
		AND CONVERT(NVARCHAR(26),purchases.DocumentDateIssued,23) between @startDate and @endDate
		AND (convert(nvarchar(50),sm.Id) like ISNULL(@salesmanId,'%') )
		AND (convert(nvarchar(50),sales.DocumentIssuerCostCentreId) like ISNULL(@distributorId,'%') OR convert(nvarchar(50),sales.DocumentRecipientCostCentre) like ISNULL(@distributorID,'%'))
		AND  convert(nvarchar(50),outlet.Id) like ISNULL(@outletId,'%')
		AND  convert(nvarchar(50),dbo.tblRoutes.RouteID) like ISNULL(@routeId,'%')
		  

GROUP BY sales.Id,
		sm.Name, 
		sales.DocumentReference,
		sales.ExtDocumentReference,
		dbo.tblRoutes.Name, 
		sales.DocumentDateIssued,
		purchases.DocumentDateIssued,
		sales.DocumentTypeId,
		purchases.DocumentTypeId,
		outlet.Cost_Centre_Code,
		outlet.Name, 
		purchaseItems.Receipt_PaymentTypeId, 
		purchaseItems.Value, 
		sales.SaleDiscount,
		sm.Name

--HAVING (SUM(saleItems.Quantity * (saleItems.Value + saleItems.Vat)))-(purchaseItems.Value + sales.SaleDiscount) <= 0
		

)
SELECT	SalesmanName,
		RouteName,
		saleReference,
		ExtDocumentReference,
		MAX(paymentdate) AS paymentDate,
		SaleDate,
		OutletCode,
		OutletName,
		InvoiceValue,
		discount,
		SUM(CashPaid) CashPaid,
		SUM(ChequePaid) ChequePaid,
		SUM(MmoneyPaid) MmoneyPaid,	   
		SUM(CashPaid) + SUM(ChequePaid) + SUM(MmoneyPaid) AS TotalPaid,
		--InvoiceValue - (SUM(CashPaid) + SUM(ChequePaid) + SUM(MmoneyPaid)) as Balance
		case when (InvoiceValue - (discount + SUM(CashPaid) + SUM(ChequePaid) + SUM(MmoneyPaid))) % 1 < 0.04 then floor(InvoiceValue - (discount + SUM(CashPaid) + SUM(ChequePaid) + SUM(MmoneyPaid))) else ceiling(InvoiceValue - (discount + SUM(CashPaid) + SUM(ChequePaid) + SUM(MmoneyPaid))) end  AS Balance
FROM  Payment_CTE
GROUP BY	SalesmanName,
			RouteName,
			saleReference,
			ExtDocumentReference,
			--paymentdate,
			SaleDate,
			OutletCode,
			OutletName,
			InvoiceValue,
			discount
HAVING case when (InvoiceValue - (discount + SUM(CashPaid) + SUM(ChequePaid) + SUM(MmoneyPaid))) % 1 < 0.04 then floor(InvoiceValue - (discount + SUM(CashPaid) + SUM(ChequePaid) + SUM(MmoneyPaid))) else ceiling(InvoiceValue - (discount + SUM(CashPaid) + SUM(ChequePaid) + SUM(MmoneyPaid))) end <= 1

GO

-- Exec sp_D_PaymentSummary_Per_Salesman @startDate = '2015-03-24',@endDate='2015-3-24', @salesmanId = 'ALL',@outletId = 'ALL',@distributorId='ALL',@routeId='ALL'

