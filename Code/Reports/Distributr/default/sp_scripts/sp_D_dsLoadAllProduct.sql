DROP PROCEDURE [dbo].[sp_D_dsLoadAllProduct]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[sp_D_dsLoadAllProduct]
as 
SELECT        TOP (1) 'ALL' AS id, 'ALL' AS [Product Name]

UNION ALL
SELECT        Lower(CONVERT(NVARCHAR(50), id)) AS id, Description AS [Product Name]
FROM            tblProduct AS tblProduct_1
WHERE tblProduct_1.IM_Status = 1
ORDER BY [Product Name]
GO

-- EXEC  [dbo].[sp_D_dsLoadAllProduct] 
