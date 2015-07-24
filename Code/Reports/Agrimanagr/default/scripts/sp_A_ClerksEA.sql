IF NOT EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'sp_A_ClerksEA')
   exec('CREATE PROCEDURE [sp_A_ClerksEA] AS BEGIN SET NOCOUNT ON; END')
GO


ALTER PROCEDURE sp_A_ClerksEA  
 @HubId nvarchar(50)
AS

if  @HubId='ALL'  begin set @HubId='%' end

SELECT  'ALL' AS Id, 'ALL' AS Name
UNION
SELECT DISTINCT CONVERT(NVARCHAR(50),tblUsers.CostCenterId),UserName
FROM tblUsers 
	INNER JOIN tblCostCentre ON tblUsers.CostCenterId = tblCostCentre.Id

WHERE (CONVERT(NVARCHAR(50),dbo.tblCostCentre.Id) LIKE ISNULL(@HubId, N'%'))
ORDER BY Name
GO

-- EXEC sp_A_ClerksEA @HubId='ALL'

