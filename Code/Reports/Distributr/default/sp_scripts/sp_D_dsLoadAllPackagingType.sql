DROP PROCEDURE [dbo].[sp_D_dsLoadAllPackagingType]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_D_dsLoadAllPackagingType]
AS

select   ' ALL' as PackagingTypeId,
         ' ALL' as PackagingTypeName
union
select     lower(convert(nvarchar(50),id)) AS PackagingTypeId, name as PackagingTypeName
from         dbo.tblProductPackagingType
where dbo.tblProductPackagingType.IM_Status = 1

GO