DROP FUNCTION dbo.ufd_CountOutletsToVisit
GO
CREATE  FUNCTION dbo.ufd_CountOutletsToVisit(@salesmanId nvarchar(100),@dayofweek nvarchar(100))
RETURNS int
AS
BEGIN
declare @OutletToVisitCount int
select @OutletToVisitCount =    count(ov.OutletName)
								from v_D_OutletVisits ov  
								where VisitDay LIKE @dayofweek  --'Monday'
								and ov.OutletId in (select o.Id
													from  tblCostCentre d
													 join tblCostCentre s on d.id = s.ParentCostCentreId
													 join tblSalemanRoute sr on sr.SalemanId = s.Id 
													 join tblRoutes rt on sr.RouteId = rt.RouteID
													 join tblCostCentre o on rt.RouteID = o.RouteId
													where d.CostCentreType = 2 
													  and s.CostCentreType = 4
													  and s.id like @salesmanId -- 'B2985017-ADB5-405B-987B-68D606B6D212'
													group by o.Id)


return @OutletToVisitCount
end ;
go

-- select dbo.ufd_CountOutletsToVisit('B2985017-ADB5-405B-987B-68D606B6D212','Monday') as Planned