DROP PROCEDURE [dbo].[sp_D_SaleValueDiscount_Routes]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[sp_D_SaleValueDiscount_Routes]
(
@startDate as datetime,
@endDate as datetime,
@distributorID AS NVARCHAR(50),
@salesManID AS NVARCHAR(50),
@outletId AS NVARCHAR(50),
@RouteID AS NVARCHAR(50)
)
as 
if  @distributorID='ALL'  begin set @distributorID='%' end
if  @salesManID='ALL'  begin set @salesManID='%' end
if  @outletId='ALL'  begin set @outletId='%' end
if  @RouteID='ALL'  begin set @RouteID='%' end

select rt.Name, 
       outlet.Name,
       sales.SaleDiscount,
       convert(nvarchar(26),sales.DocumentDateIssued,23)
from tblRoutes rt
inner join tblCostCentre outlet on rt.RouteID = outlet.RouteId
inner join tblDocument sales on sales.OrderIssuedOnBehalfOfCC = outlet.Id
where (CONVERT(NVARCHAR(26),DocumentDateIssued ,23)) between @startDate  and @endDate
      AND  CONVERT(nvarchar(50),sales.OrderIssuedOnBehalfOfCC) like ISNULL(@outletId,'%')
      AND  CONVERT(nvarchar(50),outlet.RouteId) like ISNULL(@RouteID,'%')
      AND((CONVERT(NVARCHAR(50),sales.DocumentIssuerCostCentreId) LIKE ISNULL(@distributorID,'%') and CONVERT(NVARCHAR(50),sales.DocumentRecipientCostCentre) LIKE ISNULL(@salesManID,'%')) 
       OR (CONVERT(NVARCHAR(50),sales.DocumentIssuerCostCentreId) LIKE ISNULL(@salesManID,'%') and CONVERT(NVARCHAR(50),sales.DocumentRecipientCostCentre) LIKE ISNULL(@distributorID,'%'))) 
           
 --  Exec [dbo].[sp_D_SaleValueDiscount_Routes] @startDate ='2013-11-26',@endDate ='2013-11-26',@distributorID = 'ALL',@salesManID = 'ALL' ,@outletId = 'ALL',@RouteID = 'ALL'
 
 
 GO