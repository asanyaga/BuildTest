DROP PROCEDURE [dbo].[sp_D_SaleValueDiscount_ByRoutes]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[sp_D_SaleValueDiscount_ByRoutes]
(
@startDate as datetime,
@endDate as datetime,
@distributorID AS NVARCHAR(50),
@salesManID AS NVARCHAR(50),
@outletId AS NVARCHAR(50),
@RouteID AS NVARCHAR(50)
)
as 
if  RTRIM(LTRIM(@distributorID))='ALL'  begin set @distributorID='%' end
if  RTRIM(LTRIM(@salesManID))='ALL'  begin set @salesManID='%' end
if  RTRIM(LTRIM(@outletId))='ALL'  begin set @outletId='%' end
if  RTRIM(LTRIM(@RouteID))='ALL'  begin set @RouteID='%' end

select rt.Name as RouteName, 
       outlet.Name as OutletName,
       sales.SaleDiscount
from tblRoutes rt
inner join tblCostCentre outlet on rt.RouteID = outlet.RouteId
inner join tblDocument sales on sales.OrderIssuedOnBehalfOfCC = outlet.Id
where   (DocumentTypeId = 1) 
      AND ((OrderOrderTypeId = 1) OR(OrderOrderTypeId = 3 AND DocumentStatusId = 99)) 
      AND (CONVERT(NVARCHAR(26),DocumentDateIssued ,23)) between @startDate  and @endDate
      AND  CONVERT(nvarchar(50),sales.OrderIssuedOnBehalfOfCC) like ISNULL(@outletId,'%')
      AND  CONVERT(nvarchar(50),outlet.RouteId) like ISNULL(@RouteID,'%')
      AND((CONVERT(NVARCHAR(50),sales.DocumentIssuerCostCentreId) LIKE ISNULL(@distributorID,'%') and CONVERT(NVARCHAR(50),sales.DocumentRecipientCostCentre) LIKE ISNULL(@salesManID,'%')) 
       OR (CONVERT(NVARCHAR(50),sales.DocumentIssuerCostCentreId) LIKE ISNULL(@salesManID,'%') and CONVERT(NVARCHAR(50),sales.DocumentRecipientCostCentre) LIKE ISNULL(@distributorID,'%'))) 
           
 --  Exec [dbo].[sp_D_SaleValueDiscount_ByRoutes] @startDate ='2014-5-1',@endDate ='2014-5-15',@distributorID = 'ALL',@salesManID = 'ALL' ,@outletId = 'ALL',@RouteID = 'ALL'
 
 
 GO