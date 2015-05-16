DROP FUNCTION dbo.udf_ListOutletsToVisit
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION dbo.udf_ListOutletsToVisit(@salesmanId nvarchar(100),@dayofweek nvarchar(100))
RETURNS TABLE
AS
RETURN(
select  ov.OutletId,ov.OutletName
from v_D_OutletVisits ov  
where VisitDay LIKE @dayofweek  --'Monday'
  and ov.OutletId in (select o.Id from  tblCostCentre d
							 join tblCostCentre s on d.id = s.ParentCostCentreId
													 join tblSalemanRoute sr on sr.SalemanId = s.Id 
													 join tblRoutes rt on sr.RouteId = rt.RouteID
													 join tblCostCentre o on rt.RouteID = o.RouteId
													where d.CostCentreType = 2 
													  and s.CostCentreType = 4
													  and s.id like @salesmanId -- 'B2985017-ADB5-405B-987B-68D606B6D212'
													group by o.Id)
 );
 
GO


--   select * from dbo.udf_ListOutletsToVisit('B2985017-ADB5-405B-987B-68D606B6D212','Friday')













-- select  * FROM dbo.udf_ListOutletsToVisit('B2985017-ADB5-405B-987B-68D606B6D212','Monday') as Planned