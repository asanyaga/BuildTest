
DROP PROCEDURE sp_D_SalesSummary_Per_Salesman
GO
SET ANSI_NULLS ON
GO
SET ANSI_WARNINGS OFF
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure sp_D_SalesSummary_Per_Salesman
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



;WITH Payment_CTE as (
SELECT	sales.Id, 
		sm.Name AS SalesmanNames,
		dist.id DistributorId,
		dist.Name DistributorName,
		sales.DocumentReference AS saleReference,
		sales.ExtDocumentReference,
		dbo.tblRoutes.Name AS RouteNames, 
		sales.DocumentDateIssued AS SaleDate, 
		sales.DocumentTypeId,
		receipts.DocumentTypeId AS DoctypeId,
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
        dbo.tblDocument AS receipts ON sales.Id = receipts.DocumentParentId INNER JOIN
        dbo.tblLineItems AS saleItems ON sales.Id = saleItems.DocumentID INNER JOIN
        dbo.tblLineItems AS purchaseItems ON receipts.Id = purchaseItems.DocumentID 
JOIN    dbo.tblCostCentre AS sm ON sales.DocumentRecipientCostCentre = sm.Id OR sales.DocumentIssuerCostCentreId = sm.Id
JOIN    dbo.tblCostCentre AS dist ON sales.DocumentRecipientCostCentre = dist.Id OR sales.DocumentIssuerCostCentreId = dist.Id
JOIN    dbo.tblCostCentre AS outlet ON sales.OrderIssuedOnBehalfOfCC = outlet.Id INNER JOIN
        dbo.tblRoutes ON outlet.RouteId = dbo.tblRoutes.RouteID

WHERE		sales.DocumentTypeId = 1 
		AND (sales.DocumentTypeId = 1) 
        AND (sales.OrderOrderTypeId = 1 OR( sales.OrderOrderTypeId = 3 AND sales.DocumentStatusId = 99))
		AND (receipts.DocumentTypeId = 8) 
	    AND	sm.CostCentreType = 4
	    AND	dist.CostCentreType = 2
		AND CONVERT(NVARCHAR(26),sales.DocumentDateIssued,23) between @startDate and @endDate
		AND (convert(nvarchar(50),sm.Id) like ISNULL(@salesmanId,'%') )
		AND (convert(nvarchar(50),sales.DocumentIssuerCostCentreId) like ISNULL(@distributorID,'%') OR convert(nvarchar(50),sales.DocumentRecipientCostCentre) like ISNULL(@distributorID,'%'))
		AND  convert(nvarchar(50),outlet.Id) like ISNULL(@outletId,'%')
		AND  convert(nvarchar(50),dbo.tblRoutes.RouteID) like ISNULL(@routeId,'%')
		  

GROUP BY sales.Id,
		sm.Name, 
		dist.id ,
		dist.Name,
		sales.DocumentReference,
		sales.ExtDocumentReference,
		dbo.tblRoutes.Name, 
		sales.DocumentDateIssued,
		sales.DocumentTypeId,
		receipts.DocumentTypeId,
		outlet.Cost_Centre_Code,
		outlet.Name, 
		purchaseItems.Receipt_PaymentTypeId, 
		purchaseItems.Value, 
		sales.SaleDiscount,
		sm.Name

)
SELECT	SalesmanNames,
		RouteNames,
		saleReference,
		ExtDocumentReference,
		SaleDate,
		OutletCode,
		OutletName,
		InvoiceValue,
		discount,
		SUM(CashPaid) CashPaid,
		SUM(ChequePaid) ChequePaid,
		SUM(MmoneyPaid) MmoneyPaid,	   
		SUM(CashPaid) + SUM(ChequePaid) + SUM(MmoneyPaid) AS TotalPaid,
		case when (InvoiceValue - (discount + SUM(CashPaid) + SUM(ChequePaid) + SUM(MmoneyPaid))) % 1 < 0.04 then floor(InvoiceValue - (discount + SUM(CashPaid) + SUM(ChequePaid) + SUM(MmoneyPaid))) else ceiling(InvoiceValue - (discount + SUM(CashPaid) + SUM(ChequePaid) + SUM(MmoneyPaid))) end  AS Balance
FROM  Payment_CTE
GROUP BY	SalesmanNames,
			RouteNames,
			saleReference,
			ExtDocumentReference,
			SaleDate,
			OutletCode,
			OutletName,
			InvoiceValue,
			discount
--HAVING case when (InvoiceValue - (discount + SUM(CashPaid) + SUM(ChequePaid) + SUM(MmoneyPaid))) % 1 < 0.04 then floor(InvoiceValue - (discount + SUM(CashPaid) + SUM(ChequePaid) + SUM(MmoneyPaid))) else ceiling(InvoiceValue - (discount + SUM(CashPaid) + SUM(ChequePaid) + SUM(MmoneyPaid))) end <= 0 --ommmited on 2april2015 as requsted by CTO and CSO for FCL

GO

-- Exec sp_D_SalesSummary_Per_Salesman @startDate = '2014-02-25',@endDate='2015-2-25', @salesmanId = 'ALL',@outletId = 'ALL',@distributorID='ALL',@routeId='ALL'

