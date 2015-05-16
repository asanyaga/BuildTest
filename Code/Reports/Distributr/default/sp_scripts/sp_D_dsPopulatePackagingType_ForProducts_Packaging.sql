DROP PROCEDURE [dbo].[sp_D_dsPopulatePackagingType_ForProducts_Packaging]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_D_dsPopulatePackagingType_ForProducts_Packaging]
(
@productId AS NVARCHAR(50),
@packagingId AS NVARCHAR(50)
)
as
if ltrim(rtrim(@productId))='ALL'  begin set @productId='%' end
if ltrim(rtrim(@packagingId))='ALL'  begin set @packagingId='%' end

SELECT   ' ALL' as PackagingTypeId,
         ' ALL' as PackagingTypeName
UNION ALL
SELECT DISTINCT  LOWER(CONVERT(nvarchar(50),ppt.id)) AS PackagingTypeId, 
                                            ppt.name AS PackagingTypeName
FROM       dbo.tblProductPackagingType ppt
RIGHT OUTER JOIN dbo.tblProduct p ON ppt.id = p.PackagingTypeId
WHERE      ppt.IM_Status = 1
AND CONVERT(NVARCHAR(50),p.id) LIKE ISNULL(@productId,'%')
AND CONVERT(NVARCHAR(50),p.PackagingId) LIKE ISNULL(@packagingId,'%')
ORDER BY PackagingTypeName

-- Exec [sp_D_dsPopulatePackagingType_ForProducts_Packaging] @productId='ALL' ,@packagingId ='ALL'


GO