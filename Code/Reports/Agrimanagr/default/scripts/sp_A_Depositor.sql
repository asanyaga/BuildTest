IF NOT EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'sp_A_Depositor')
   exec('CREATE PROCEDURE [sp_A_Depositor] AS BEGIN SET NOCOUNT ON; END')
GO

ALTER PROCEDURE sp_A_Depositor  

AS

SELECT  'ALL' AS Id, 'ALL' AS Name
UNION
SELECT DISTINCT CONVERT(NVARCHAR(50),Id),(dbo.tblCommodityOwner.FirstName + ' ' + dbo.tblCommodityOwner.Surname) as FarmerName
FROM tblCommodityOwner
ORDER BY Name

GO

-- EXEC sp_A_Depositor


