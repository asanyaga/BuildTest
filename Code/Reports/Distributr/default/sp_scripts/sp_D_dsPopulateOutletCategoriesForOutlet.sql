DROP PROCEDURE [dbo].[sp_D_dsPopulateOutletCategoriesForOutlet]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_D_dsPopulateOutletCategoriesForOutlet]
(
@outletId AS NVARCHAR(50)
)
as
if  ltrim(rtrim(@outletId))='ALL'  begin set @outletId='%' end

SELECT    ' ALL' AS OutletCategoryId, 
          ' ALL' AS OutletCategory
UNION ALL
SELECT  DISTINCT CONVERT(nvarchar(50), oc.id) AS OutletCategoryId, 
                                   oc.Name AS OutletCategory
FROM tblOutletCategory AS oc
JOIN tblCostCentre outlet on oc.id = outlet.Outlet_Category_Id
WHERE oc.IM_Status = 1
AND CONVERT(NVARCHAR(50),outlet.Id)LIKE ISNULL(@outletId,'%')

-- exec sp_D_dsPopulateOutletCategoriesForOutlet @outletId='ALL'

GO
