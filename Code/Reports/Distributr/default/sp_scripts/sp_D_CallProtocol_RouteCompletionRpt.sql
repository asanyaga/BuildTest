-- CALL PROTOCOL: ROUTE COMPLETION REPORT
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_D_CallProtocol_RouteCompletionRpt]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[sp_D_CallProtocol_RouteCompletionRpt]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[sp_D_CallProtocol_RouteCompletionRpt]
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

;WITH CallPlan_CTE as (
	SELECT  tblCostCentre_1.Name AS SalesmanName, 
			dbo.v_D_OutletVisits.VisitDay, 
			dbo.tblRoutes.Name AS Route, 
			dbo.v_D_OutletVisits.OutletName, 
			dbo.tblRoutes.RouteID AS RouteId, 
			tblCostCentre_1.Id AS SalesmanId, 
			dbo.v_D_OutletVisits.OutletId
	FROM    dbo.v_D_OutletVisits 
	JOIN    dbo.tblCostCentre ON dbo.v_D_OutletVisits.OutletId = dbo.tblCostCentre.Id 
	JOIN    dbo.tblRoutes ON dbo.tblCostCentre.RouteId = dbo.tblRoutes.RouteID 
	JOIN    dbo.tblSalemanRoute ON dbo.tblRoutes.RouteID = dbo.tblSalemanRoute.RouteId AND dbo.tblRoutes.RouteID = dbo.tblSalemanRoute.RouteId 
	JOIN    dbo.tblCostCentre AS tblCostCentre_1 ON dbo.tblSalemanRoute.SalemanId = tblCostCentre_1.Id
	--WHERE  (tblCostCentre_1.Id = '01469fba-9693-450f-b2dc-38d98a220554')
--	ORDER BY SalesmanName, dbo.v_D_OutletVisits.VisitDay, Route, dbo.v_D_OutletVisits.OutletName
),

CallsMade_CTE as (
	SELECT tblDocument_1.DocumentReference AS DocReference, 
		   tblDocument_1.DocumentTypeId AS DocType, 
		   dbo.tblCostCentre.Id AS OutletId, 
		   dbo.tblCostCentre.Name AS OutletName, 
		  tblDocument_1.DocumentDateIssued VisitDate, 
		   dbo.v_D_OutletVisits.VisitDay
	FROM   dbo.tblDocument 
	JOIN   dbo.tblDocument AS tblDocument_1 ON dbo.tblDocument.Id = tblDocument_1.VisitId
	JOIN   dbo.tblCostCentre ON tblDocument_1.OrderIssuedOnBehalfOfCC = dbo.tblCostCentre.Id 
	JOIN   dbo.v_D_OutletVisits ON dbo.tblCostCentre.Id = dbo.v_D_OutletVisits.OutletId
	WHERE (dbo.tblDocument.DocumentTypeId = 20)
	and DATENAME(dw,tblDocument_1.DocumentDateIssued) like dbo.v_D_OutletVisits.VisitDay
	--ORDER BY VisitDate DESC
)

SELECT cp.* ,
  case when cp.OutletId = cm.OutletId  and cp.VisitDay = cm.VisitDay then 1 else 0 end as Activity
  --cm.VisitDate,
  --cm.VisitDay,
  --cm.OutletName
--case when cp.OutletId in (select cm.OutletId from CallsMade_CTE cm ) and cp.VisitDay = cm.VisitDay then 1 else 0 end as Activity
--CASE WHEN OutletId IN (SELECT OutletId FROM CallsMade_CTE) and cp.VisitDay in (SELECT VisitDay FROM CallsMade_CTE) THEN 1 ELSE 0 END AS Ativity
FROM CallPlan_CTE cp,CallsMade_CTE cm
--WHERE cp.SalesmanId ='01469FBA-9693-450F-B2DC-38D98A220554'


-- EXEC [dbo].[sp_D_CallProtocol_RouteCompletionRpt] @startDate ='2014-11-01',@endDate ='2014-11-30',@distributorId ='ALL',@salesmanId ='ALL',@routeId ='ALL',@outletId ='ALL'

     