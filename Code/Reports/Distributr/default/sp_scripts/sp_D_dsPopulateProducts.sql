DROP PROCEDURE [dbo].[sp_D_dsPopulateProducts]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[sp_D_dsPopulateProducts]
as
SELECT        ' ALL' AS ProductId, 
              ' ALL' AS ProductName
UNION ALL
SELECT        LOWER(CONVERT(NVARCHAR(50), p.id)) AS ProductId, 
                                   p.Description AS ProductName
FROM            tblProduct AS p
WHERE p.IM_Status = 1
ORDER BY ProductName
GO
