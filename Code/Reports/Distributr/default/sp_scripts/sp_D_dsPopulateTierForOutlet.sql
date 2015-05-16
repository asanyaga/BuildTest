DROP PROCEDURE [dbo].[sp_D_dsPopulateTierForOutlet]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_D_dsPopulateTierForOutlet]
(
@outletId AS NVARCHAR(50)
)
as
if  ltrim(rtrim(@outletId))='ALL'  begin set @outletId='%' end

SELECT  ' ALL' AS TierId, 
                ' ALL' AS TierName
UNION ALL
SELECT  Distinct convert(nvarchar(50),tier.id) AS TierId, 
                                  tier.Name AS TierName
FROM  tblPricingTier tier
join  tblCostCentre outlet on tier.id = outlet.Tier_Id
WHERE tier.IM_Status = 1
AND CONVERT(NVARCHAR(50),outlet.Tier_Id) LIKE ISNULL(@outletId,'%')
order by TierName

--  EXEC sp_D_dsPopulateTierForOutlet @outletId='ALL'
GO
