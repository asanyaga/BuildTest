-- CALL PROTOCOL: NON PRODUCTIVE CALLS
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_D_CallProtocol_NonProductiveCalls]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[sp_D_CallProtocol_NonProductiveCalls]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[sp_D_CallProtocol_NonProductiveCalls]
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

SELECT  d.Id DistributorId,
        d.Name DistributorName,
        sm.Id SalesmanId,
        sm.Name SalesmanName,
        convert(nvarchar(26),cp.stDate,23) AS VisitDate, 
        cp.stQty, 
		cp.stProduct,
		dbo.tblOutletVisitReasonType.Name AS VisitReason, 
        dbo.tblCostCentre.Id AS OutletId, 
		dbo.tblCostCentre.Name AS OutletName, 
		dbo.tblRoutes.RouteID AS RouteId, 
		dbo.tblRoutes.Name AS RouteName, 
		dbo.tblOutletType.id AS OutletTypeId, 
        dbo.tblOutletType.Name AS OutletTypeName
FROM    dbo.v_D_CallProtocol_StockTake cp
 JOIN   dbo.tblDocument st ON cp.stVisitId = st.Id 
 JOIN   dbo.tblCostCentre sm ON (st.DocumentIssuerCostCentreId = sm.Id OR st.DocumentRecipientCostCentre = sm.Id)
 JOIN   dbo.tblCostCentre d ON (st.DocumentIssuerCostCentreId = d.Id OR st.DocumentRecipientCostCentre = d.Id)
 JOIN   dbo.tblOutletVisitReasonType ON st.PaymentDocId = dbo.tblOutletVisitReasonType.id 
 JOIN   dbo.tblCostCentre ON st.OrderIssuedOnBehalfOfCC = dbo.tblCostCentre.Id 
 JOIN   dbo.tblRoutes ON dbo.tblCostCentre.RouteId = dbo.tblRoutes.RouteID 
 JOIN   dbo.tblOutletType ON dbo.tblCostCentre.Outlet_Type_Id = dbo.tblOutletType.id
WHERE sm.CostCentreType = 4 and d.CostCentreType = 2
 AND  convert(nvarchar(26),cp.stDate,23) between @startDate and @endDate
 AND  convert(nvarchar(50),d.Id) like  isnull(@distributorId,'%')
 AND  convert(nvarchar(50),sm.Id) like  isnull(@salesmanId,'%')
 AND  convert(nvarchar(50),dbo.tblRoutes.RouteID) like  isnull(@routeId,'%')
 AND  convert(nvarchar(50),dbo.tblCostCentre.Id) like  isnull(@outletId,'%')

 ORDER BY cp.stDate DESC

 -- exec  [dbo].[sp_D_CallProtocol_NonProductiveCalls] @startDate ='2014-11-18',@endDate ='2014-11-18',@distributorId ='ALL',@salesmanId ='ALL',@routeId ='ALL',@outletId ='ALL'
