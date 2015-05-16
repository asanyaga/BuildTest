/****** Object:  StoredProcedure [dbo].[sp_D_dsPopulaterRoutesForDistributorSalesman_ForSalesRpts]    Script Date: 07/24/2013 08:51:32 ******/
DROP PROCEDURE [dbo].[sp_D_dsPopulaterRoutesForDistributorSalesman_ForSalesRpts]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_D_dsPopulaterRoutesForDistributorSalesman_ForSalesRpts]
(
@distributorID nvarchar(50),
@salesManID nvarchar(50)

)
AS
if ltrim(rtrim(@distributorID))='ALL'  begin set @distributorID='%' end
if ltrim(rtrim(@salesManID))='ALL'  begin set @salesManID='%' end

SELECT        ' ALL' AS RouteId, 
              ' ALL' AS Route
       

UNION ALL

SELECT  DISTINCT      LOWER (CONVERT(nvarchar(50), rt.RouteID)) AS RouteId, 
                                          rt.Name AS RouteName
FROM tblRoutes AS rt
join tblCostCentre outlet on rt.RouteID = outlet.RouteId
join tblCostCentre dist on outlet.ParentCostCentreId = dist.Id
join tblSalemanRoute sr on rt.RouteID = sr.RouteId
WHERE  rt.IM_Status = 1
AND CONVERT(nvarchar(50),dist.Id) like ISNULL(@distributorID,'%')
AND CONVERT(nvarchar(50),sr.SalemanId) like ISNULL(@salesManID,'%')

ORDER BY Route

-- Exec sp_D_dsPopulaterRoutesForDistributorSalesman_ForSalesRpts @distributorID='ALL',@salesManID='ALL'

GO
