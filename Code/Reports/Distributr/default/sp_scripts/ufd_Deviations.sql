DROP FUNCTION dbo.udf_CountVisitDeviations
GO
CREATE  FUNCTION dbo.udf_CountVisitDeviations(@visitday nvarchar(50),@startDate nvarchar(50),@endDate nvarchar(50),@salesmanId nvarchar(50))
RETURNS int
AS
BEGIN
declare @deviations int
;with ov_cte as
(SELECT  DISTINCT       
		outlets.Id AS OutletId,
		outlets.Name AS OutletVisited
FROM    dbo.tblDocument AS sale 
 JOIN   dbo.tblLineItems AS saleItems ON sale.Id = saleItems.DocumentID 
 JOIN   dbo.tblCostCentre AS dist ON sale.DocumentIssuerCostCentreId = dist.Id OR sale.DocumentRecipientCostCentre = dist.Id 
 JOIN   dbo.tblCostCentre AS salesman ON sale.DocumentIssuerCostCentreId = salesman.Id OR sale.DocumentRecipientCostCentre = salesman.Id 
 JOIN   dbo.tblCostCentre outlets ON sale.OrderIssuedOnBehalfOfCC = outlets.id
 JOIN   dbo.tblRegion AS reg ON dist.Distributor_RegionId = reg.id 
 JOIN   dbo.tblCountry AS cntry ON reg.Country = cntry.id 
 JOIN   dbo.tblRoutes rt ON outlets.RouteId = rt.RouteID
WHERE 
   convert(nvarchar(26),sale.DocumentDateIssued,23)  BETWEEN @startDate AND @endDate -- like  convert(nvarchar(26),'2014-10-01',23)
  AND salesman.Id LIKE ISNULL(@salesmanId,'%') --'B2985017-ADB5-405B-987B-68D606B6D212'
  AND outlets.Id NOT IN (select  ov.OutletId
							from v_D_OutletVisits ov  
							where VisitDay LIKE @visitday
						    and ov.OutletId in (select o.Id 
												  from  tblCostCentre d
												   join tblCostCentre s on d.id = s.ParentCostCentreId
												   join tblSalemanRoute sr on sr.SalemanId = s.Id 
												   join tblRoutes rt on sr.RouteId = rt.RouteID
												   join tblCostCentre o on rt.RouteID = o.RouteId
													where d.CostCentreType = 2 
													  and s.CostCentreType = 4
													  and s.id like  @salesmanId
													group by o.Id)))

	select  @deviations = count(OutletId) from ov_cte
return @deviations
end ;
go

--SELECT dbo.udf_CountVisitDeviations('Friday','2014-10-03','2014-10-03','B2985017-ADB5-405B-987B-68D606B6D212')