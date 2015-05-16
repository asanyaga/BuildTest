/****** Object:  StoredProcedure [dbo].[sp_D_dsPopulateDistributors]    Script Date: 07/24/2013 08:51:32 ******/
DROP PROCEDURE [dbo].[sp_D_dsPopulateDistributors]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_D_dsPopulateDistributors]
AS

SELECT  ' ALL' AS DistributorId,
        ' ALL' AS Distributor 

UNION ALL

SELECT    LOWER(CONVERT(nvarchar(50), dist.Id)) AS DistributorId,
                                      dist.Name AS Distributor 
FROM        tblCostCentre dist
WHERE  dist.CostCentreType = 2 
AND    dist.IM_Status = 1
ORDER BY Distributor

--   Exec  [dbo].[sp_D_dsPopulateDistributors]




GO
