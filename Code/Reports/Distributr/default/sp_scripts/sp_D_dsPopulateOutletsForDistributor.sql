DROP PROCEDURE [dbo].[sp_D_dsPopulateOutletsForDistributor]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_D_dsPopulateOutletsForDistributor]
(
@distributorId AS NVARCHAR(50)
)
as
if  ltrim(rtrim(@distributorId))='ALL'  begin set @distributorId='%' end
SELECT     ' ALL' as OutletId, 
           ' ALL' as OutletName
UNION ALL
SELECT CONVERT(NVARCHAR(50),outlet.Id) as OutletId,  
       outlet.Name as OutletName    
FROM       dbo.tblCostCentre outlet 
WHERE     (outlet.CostCentreType = 5)
AND CONVERT(NVARCHAR(50),outlet.ParentCostCentreId) LIKE ISNULL(@distributorId,'%')


-- exec sp_D_dsPopulateOutletsForDistributor @distributorId='ALL'

GO