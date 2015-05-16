
DROP PROCEDURE [dbo].[sp_D_dsLoadAllProductTypes]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[sp_D_dsLoadAllProductTypes]
as 

select ' ALL' as ProductTypeId,
       ' ALL' as ProductTypeName
union
select 'EMPTY' as ProductTypeId,
       'Returnable ProductType' as ProductTypeName
union
select   lower(convert(nvarchar(50),id)) as ProductTypeId,name as ProductTypeName
from   dbo.tblProductType
WHERE dbo.tblProductType.IM_Status = 1
order by ProductTypeName
-- exec [dbo].[sp_D_dsLoadAllProductTypes]
GO