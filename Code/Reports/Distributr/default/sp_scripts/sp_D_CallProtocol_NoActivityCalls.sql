-- CALL PROTOCOL: NO ACTIVITY CALL REPORT
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_D_CallProtocol_NoActivityCalls]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[sp_D_CallProtocol_NoActivityCalls]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[sp_D_CallProtocol_NoActivityCalls]
(
@startDate as datetime,
@endDate as datetime,
@distributorId as nvarchar(50),
@salesmanId as nvarchar(50),
@routeId as nvarchar(50),
@outletId as nvarchar(50)
)
as
if ltrim(rtrim(@distributorId))='ALL' begin set @distributorId='%' end
if ltrim(rtrim(@salesmanId))='ALL' begin set @salesmanId='%' end
if ltrim(rtrim(@routeId))='ALL' begin set @routeId='%' end
if ltrim(rtrim(@outletId))='ALL' begin set @outletId='%' end

SELECT d.Id DistributorId,
       d.Name DistributorName,
	   s.Id SalesmanId,
	   s.Name SalesmanName,
	   rt.RouteID AS RouteId, 
	   rt.Name AS RouteName,
   	   outlet.Id AS OutletId, 
       outlet.Name AS OutletName, 
	   ot.id OutletTypeId,
	   ot.Name OutletTypeName,
       cp.DocumentTypeId, 
       dbo.tblOutletVisitReasonType.Name AS Reason, 
	   convert(nvarchar(26),cp.DocumentDateIssued,23) AS VisitDate,
       right(convert(varchar(32),cp.DocumentDateIssued,100),8) AS VisitTime

FROM   dbo.tblDocument cp 
	 JOIN  dbo.tblCostCentre d ON (cp.DocumentIssuerCostCentreId = d.Id OR cp.DocumentRecipientCostCentre = d.Id)
	 JOIN  dbo.tblCostCentre s ON (cp.DocumentIssuerCostCentreId = s.Id OR cp.DocumentRecipientCostCentre = s.Id)
	 JOIN  dbo.tblOutletVisitReasonType ON cp.PaymentDocId = dbo.tblOutletVisitReasonType.id 
	 JOIN  dbo.tblCostCentre outlet ON cp.OrderIssuedOnBehalfOfCC = outlet.Id 
	 JOIN  dbo.tblRoutes rt ON outlet.RouteId = rt.RouteID
	 JOIN  dbo.tblOutletType ot ON outlet.Outlet_Type_Id = ot.id
WHERE (cp.DocumentTypeId = 20)
	AND s.CostCentreType = 4
	AND d.CostCentreType = 2
	AND convert(nvarchar(26),cp.DocumentDateIssued,23) between @startDate and @endDate
	AND convert(nvarchar(50),d.Id) like isnull(@distributorId,'%')
	AND convert(nvarchar(50),s.Id) like isnull(@salesmanId,'%')
	AND convert(nvarchar(50),rt.RouteID) like isnull(@routeId,'%')
	AND convert(nvarchar(50),outlet.Id ) like isnull(@outletId,'%')

ORDER BY VisitDate DESC ,VisitTime DESC

 -- exec  [dbo].[sp_D_CallProtocol_NoActivityCalls] @startDate ='2015-02-26',@endDate ='2015-02-26',@distributorId ='ALL',@salesmanId ='ALL',@routeId ='ALL',@outletId ='1DFB534A-6140-43EA-A2AE-4E3C5AE5BADA'
