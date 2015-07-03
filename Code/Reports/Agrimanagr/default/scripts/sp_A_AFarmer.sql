IF NOT EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'sp_A_AFarmer')
   exec('CREATE PROCEDURE [sp_A_AFarmer] AS BEGIN SET NOCOUNT ON; END')
GO
Alter PROCEDURE sp_A_AFarmer   

AS

SELECT  'ALL' AS Id, 'ALL' AS Name
UNION
SELECT DISTINCT CONVERT(NVARCHAR(50),Id),(dbo.tblCommodityOwner.FirstName + ' ' + dbo.tblCommodityOwner.Surname) as FarmerName
FROM tblCommodityOwner
ORDER BY Name

-- EXEC sp_A_AFarmer