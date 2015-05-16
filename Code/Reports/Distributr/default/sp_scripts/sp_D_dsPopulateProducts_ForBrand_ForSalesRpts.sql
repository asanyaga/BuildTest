DROP PROCEDURE [dbo].[sp_D_dsPopulateProducts_ForBrand_ForSalesRpts]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[sp_D_dsPopulateProducts_ForBrand_ForSalesRpts]
(
@Brand nvarchar(50)
)
as 
if ltrim(rtrim(@Brand))='ALL'  begin set @Brand='%' end

SELECT        ' ALL' AS ProductId, 
              ' ALL' AS ProductName
UNION ALL
SELECT        LOWER(CONVERT(NVARCHAR(50), p.id)) AS ProductId, 
                                   p.Description AS ProductName
FROM            tblProduct AS p
WHERE p.IM_Status = 1
AND CONVERT(NVARCHAR(50),p.BrandId) like ISNULL(@Brand,'%')
ORDER BY ProductName
GO
