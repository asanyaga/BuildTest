/****** Object:  StoredProcedure [dbo].[sp_D_Targets_CountryTargets]    Script Date: 07/24/2013 08:51:32 ******/
DROP PROCEDURE [dbo].[sp_D_Targets_CountryTargets]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[sp_D_Targets_CountryTargets]
(
@targetId AS NVARCHAR(50),
@countryId AS NVARCHAR(50)
)

AS
if ltrim(rtrim(@targetId))='ALL'  begin set @targetId='%' end
if ltrim(rtrim(@countryId))='ALL'  begin set @countryId='%' end

SELECT     distributr.Id AS DistributorId, 
           distributr.Name AS DistributorName,
           distributr.CostCentreType AS DCCtype, 
           outlet.Id AS OutletId, 
           outlet.Name AS OutletName, 
           outlet.CostCentreType AS OutCCtype,
           rt.RouteID AS RouteId, 
           rt.Name AS RouteName, 
           reg.id AS RegionId,
           reg.Name AS RegionName, 
           cntry.id AS CountryId, 
           cntry.Name AS CountryName, 
           t.id AS TargetId,
           t.TargetValue, 
           CASE when t.IsQuantityTarget = 1 THEN 'Quantity Target' ELSE 'Sales Target' END AS TargetType,
           tp.Id AS TargetPeriodId, 
           tp.Name AS TargetPeriodName, 
           tp.StartDate,
           tp.EndDate
FROM         dbo.tblTargetPeriod tp
 JOIN   dbo.tblTarget  t ON tp.Id = t.PeriodId 
 JOIN   dbo.tblCostCentre distributr
 JOIN   dbo.tblCostCentre AS outlet ON distributr.Id = outlet.ParentCostCentreId 
 JOIN   dbo.tblRoutes rt ON outlet.RouteId = rt.RouteID 
 JOIN   dbo.tblRegion reg ON rt.RegionId = reg.id 
 JOIN   dbo.tblCountry cntry ON reg.Country = cntry.id ON t.CostCentreId = distributr.Id
WHERE     (distributr.CostCentreType = 2) 
           AND (outlet.CostCentreType = 5)
           AND(CONVERT(NVARCHAR(50), tp.Id)LIKE ISNULL(@targetId, N'%'))
           AND(CONVERT(NVARCHAR(50), cntry.Id)LIKE ISNULL(@countryId, N'%'))
           
--  exec   [dbo].[sp_D_Targets_CountryTargets]    @targetId='ALL', @countryId='ALL'  
           
GO
