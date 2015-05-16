
DROP PROCEDURE [dbo].[sp_D_LoadAllSubBrand]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_D_LoadAllSubBrand]
AS
select     ' ALL' as SubBrandId, 
           ' ALL' as SubBrandName 
union
select lower(convert(nvarchar(50),id)) as SubBrandId, 
                                   name as SubBrandName
from  dbo.tblProductFlavour
WHERE dbo.tblProductFlavour.IM_Status = 1
order by SubBrandName
Go