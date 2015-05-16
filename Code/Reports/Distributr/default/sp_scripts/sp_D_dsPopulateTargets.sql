DROP PROCEDURE [dbo].[sp_D_dsPopulateTargets]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[sp_D_dsPopulateTargets]
as 
SELECT   ' ALL' AS TargetId, 
         ' ALL' AS TargetName
UNION ALL
SELECT      CONVERT(nvarchar(50), Id)AS TargetId, 
                                Name AS TargetName
FROM         dbo.tblTargetPeriod
WHERE  dbo.tblTargetPeriod.IM_Status = 1