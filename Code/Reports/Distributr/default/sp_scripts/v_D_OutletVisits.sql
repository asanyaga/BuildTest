DROP VIEW [dbo].[v_D_OutletVisits]
GO
CREATE VIEW [dbo].[v_D_OutletVisits]
AS
SELECT ov.EffectiveDate, 
	   ov.OutletId ,
	   dbo.tblCostCentre.Name OutletName,
	   dbo.ufd_GetDayOfWeek(ov.VistDay) VisitDay
	   --DATENAME(DW,ov.VistDay) VisitDay
FROM   dbo.tblOutletVisitDay ov
 JOIN  dbo.tblCostCentre ON ov.OutletId = dbo.tblCostCentre.Id

