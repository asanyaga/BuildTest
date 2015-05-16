DROP PROCEDURE [dbo].[sp_D_dsPopulateOutletTypesForOutlets]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_D_dsPopulateOutletTypesForOutlets]
(
@outletId nvarchar(50)
)
AS
if ltrim(rtrim(@outletId))='ALL'  begin set @outletId='%' end

SELECT  ' ALL' AS OutletTypeId, 
        ' ALL' AS OutletType
UNION ALL
SELECT        CONVERT(nvarchar(50), ot.id) AS OutletTypeId, 
                                   ot.Name AS OutletType
FROM tblOutletType AS ot
join tblCostCentre outlet on ot.id =  outlet.Outlet_Type_Id
WHERE ot.IM_Status = 1
  AND convert(nvarchar(50),outlet.Id) like ISNULL(@outletId,'%')
  
-- EXEC sp_D_dsPopulateOutletTypesForOutlets   @outletId='ALL'
  
GO
