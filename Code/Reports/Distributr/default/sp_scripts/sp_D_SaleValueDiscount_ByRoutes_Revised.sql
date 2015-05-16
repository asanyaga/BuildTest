DROP PROCEDURE [dbo].[sp_D_SaleValueDiscount_ByRoutes_Revised]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[sp_D_SaleValueDiscount_ByRoutes_Revised]
(
@startDate as datetime,
@endDate as datetime,
@distributorID AS NVARCHAR(50),
@salesManID AS NVARCHAR(50),
@outletID AS NVARCHAR(50),
@routeId AS NVARCHAR(50)
)
as 
if  RTRIM(LTRIM(@distributorID))='ALL'  begin set @distributorID='%' end
if  RTRIM(LTRIM(@salesManID))='ALL'  begin set @salesManID='%' end
if  RTRIM(LTRIM(@outletID))='ALL'  begin set @outletID='%' end
if  RTRIM(LTRIM(@routeId))='ALL'  begin set @routeId='%' end

select rt.Name as RouteName, 
       outlet.Name as OutletName,
       sales.SaleDiscount
from tblRoutes rt
inner join tblCostCentre outlet on rt.RouteID = outlet.RouteId
inner join tblDocument sales on sales.OrderIssuedOnBehalfOfCC = outlet.Id
where   (DocumentTypeId = 1) 
      AND ((OrderOrderTypeId = 1) OR(OrderOrderTypeId = 3 AND DocumentStatusId = 99)) 
      AND (CONVERT(NVARCHAR(26),DocumentDateIssued ,23)) between @startDate  and @endDate
      AND  CONVERT(nvarchar(50),sales.OrderIssuedOnBehalfOfCC) like ISNULL(@outletID,'%')
      AND  CONVERT(nvarchar(50),outlet.RouteId) like ISNULL(@routeId,'%')
      AND((CONVERT(NVARCHAR(50),sales.DocumentIssuerCostCentreId) LIKE ISNULL(@distributorID,'%') and CONVERT(NVARCHAR(50),sales.DocumentRecipientCostCentre) LIKE ISNULL(@salesManID,'%')) 
       OR (CONVERT(NVARCHAR(50),sales.DocumentIssuerCostCentreId) LIKE ISNULL(@salesManID,'%') and CONVERT(NVARCHAR(50),sales.DocumentRecipientCostCentre) LIKE ISNULL(@distributorID,'%'))) 
           
 --  Exec [dbo].[sp_D_SaleValueDiscount_ByRoutes_Revised] @startDate ='2014-5-1',@endDate ='2014-5-15',@distributorID = 'ALL',@salesManID = 'ALL' ,@outletID = 'ALL',@routeId = 'ALL'
 
 
 GO