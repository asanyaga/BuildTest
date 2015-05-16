-- CALL PROTOCOL: SALE CALL REPORT
DROP PROCEDURE [dbo].[sp_D_CallProtocol_SalesCallReport]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[sp_D_CallProtocol_SalesCallReport]
(
@startDate as datetime,
@endDate as datetime,
@distributorId as nvarchar(50),
@salesmanId as nvarchar(50)
)
as
if ltrim(rtrim(@distributorId))='ALL' begin set @distributorId='%' end
if ltrim(rtrim(@salesmanId))='ALL' begin set @salesmanId='%' end

;WITH cp_CTE as (
SELECT  DISTINCT 
        cntry.id AS CountryId, 
        cntry.Name AS CountryName, 
        reg.id AS RegionId, 
        reg.Name AS RegionName, 
        dist.Id AS DistributorId, 
        dist.Name AS DistributorName, 
        salesman.Id AS SalesmanId, 
        salesman.Name AS SalesmanName, 
		outlets.Id AS OutletId,
		outlets.Name AS OutletVisited,
		rt.RouteID AS RouteId,
		rt.Name AS RouteName,
        sale.Id AS SaleId, 
        sale.DocumentReference AS SaleRef, 
		CONVERT(nvarchar(26),sale.DocumentDateIssued,23) TransactionDate,        
        sale.VisitId as sVisitId,
		ov.VisitDay
FROM    dbo.tblDocument AS sale 
 JOIN   dbo.tblLineItems AS saleItems ON sale.Id = saleItems.DocumentID 
 JOIN   dbo.tblCostCentre AS dist ON sale.DocumentIssuerCostCentreId = dist.Id OR sale.DocumentRecipientCostCentre = dist.Id 
 JOIN   dbo.tblCostCentre AS salesman ON sale.DocumentIssuerCostCentreId = salesman.Id OR sale.DocumentRecipientCostCentre = salesman.Id 
 JOIN   dbo.tblCostCentre outlets ON sale.OrderIssuedOnBehalfOfCC = outlets.id
 JOIN   dbo.v_D_OutletVisits ov ON outlets.Id = ov.OutletId
 JOIN   dbo.tblRegion AS reg ON dist.Distributor_RegionId = reg.id 
 JOIN   dbo.tblCountry AS cntry ON reg.Country = cntry.id 
 JOIN   dbo.tblRoutes rt ON outlets.RouteId = rt.RouteID
WHERE  --(sale.OrderOrderTypeId = 1) 
 -- AND ((sale.DocumentTypeId = 1)OR(sale.OrderOrderTypeId = 3) AND (sale.DocumentStatusId = 99))
  salesman.CostCentreType = 4 and dist.CostCentreType = 2
  AND  ov.VisitDay LIKE  datename(dw,sale.DocumentDateIssued) 
  AND convert(nvarchar(26),sale.DocumentDateIssued,23)  BETWEEN @startDate AND @endDate -- like  convert(nvarchar(26),'2014-10-01',23)
  AND dist.Id LIKE ISNULL(@distributorId,'%')
  AND salesman.Id LIKE ISNULL(@salesmanId,'%') --'B2985017-ADB5-405B-987B-68D606B6D212'
		)

SELECT DISTINCT
      TransactionDate,
      COUNT(SaleRef) TotalTransactions,
	  CountryId,CountryName,
	  RegionId,RegionName,
	  DistributorId,DistributorName,
	  SalesmanId,SalesmanName,
	  RouteId,RouteName,
	  COUNT(OutletVisited) +  dbo.udf_CountVisitDeviations(VisitDay,TransactionDate,TransactionDate,SalesmanId) CallsAchieved,
	  dbo.ufd_CountOutletsToVisit(SalesmanId,VisitDay) as CallsPlanned,
	  dbo.udf_CountVisitDeviations(VisitDay,TransactionDate,TransactionDate,SalesmanId) as CountDeviations
	
 FROM cp_CTE	
 GROUP BY CountryId,CountryName,
	      RegionId,RegionName,
	      DistributorId,DistributorName,
	      SalesmanId,SalesmanName,
	      --OutletId,OutletVisited,
		  RouteId,RouteName,
		  VisitDay,TransactionDate

-- EXEC [dbo].[sp_D_CallProtocol_SalesCallReport] @startDate = '2015-03-13',@endDate = '2015-03-13',@distributorId = 'ALL',@salesmanId = 'D2CE6838-A5FF-44D6-8B65-1616E6B666CC'

GO

