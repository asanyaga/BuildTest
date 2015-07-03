IF NOT EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'sp_A_Clerks')
   exec('CREATE PROCEDURE [sp_A_Clerks] AS BEGIN SET NOCOUNT ON; END')
GO
Alter PROCEDURE sp_A_Clerks   
 @HubId nvarchar(50)
AS

if  @HubId='ALL'  begin set @HubId='%' end

SELECT  'ALL' AS Id, 'ALL' AS Name
UNION
SELECT DISTINCT CONVERT(NVARCHAR(50),tblUsers.Id),UserName
FROM tblUsers 
	INNER JOIN tblCostCentre ON tblUsers.CostCenterId = tblCostCentre.Id

WHERE (CONVERT(NVARCHAR(50),dbo.tblCostCentre.Id) LIKE ISNULL(@HubId, N'%'))
ORDER BY Name

-- EXEC sp_A_Clerks @HubId='ALL'