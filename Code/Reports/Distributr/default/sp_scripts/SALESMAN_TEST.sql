--USE [DistributrTrunk]
--GO
/****** Object:  StoredProcedure [dbo].[SALESMAN_TEST]    Script Date: 09/13/2013 10:18:27 ******/
--SET ANSI_NULLS ON
--GO
--SET QUOTED_IDENTIFIER ON
--GO
--ALTER proc [dbo].[SALESMAN_TEST]
DROP PROCEDURE [dbo].[SALESMAN_TEST]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[SALESMAN_TEST]
 (
 
 @distributorId as nvarchar(50)
 
 )
 
as

if  @distributorId='ALL'  begin set @distributorId='%' end

 


SELECT  TOP (1) 'ALL' AS SalesmanId, 
                'ALL' AS Salesman, 
                    0 AS CostCentreType
UNION ALL
SELECT        CONVERT(nvarchar(50), Id) AS SalesmanId, 
                                   Name AS Salesman,
                                      CostCentreType
FROM       tblCostCentre AS tblCostCentre_1
WHERE        (CostCentreType = 4)
           AND (convert(varchar(50),tblCostCentre_1.ParentCostCentreId) like ISNULL(@distributorId,'%'))
         
ORDER BY Salesman
