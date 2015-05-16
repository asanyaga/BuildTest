DROP PROCEDURE [dbo].[sp_D_dsPopulatePackaging_ForProducts]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[sp_D_dsPopulatePackaging_ForProducts]
(
@productId AS NVARCHAR(50)
)
as
if ltrim(rtrim(@productId))='ALL'  begin set @productId='%' end

SELECT TOP (1)  ' ALL' AS PackagingId, 
                ' ALL' AS PackagingName
UNION ALL
SELECT DISTINCT lower(CONVERT(NVARCHAR(50),prodPack.Id)) as PackagingId,
                                  prodPack.Name as PackagingName
FROM     tblProductPackaging AS prodPack
INNER JOIN tblProduct prod on prodPack.Id = prod.PackagingId
WHERE CONVERT(nvarchar(50),prod.id) like ISNULL(@productId,'%')
AND prodPack.IM_Status = 1
ORDER BY PackagingName

-- Exec [dbo].[sp_D_dsPopulatePackaging_ForProducts] @productId='ac2786d7-e998-47c5-b030-03d99944e002'

GO
