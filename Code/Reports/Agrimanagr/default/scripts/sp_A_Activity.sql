IF NOT EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'sp_A_Activity')
   exec('CREATE PROCEDURE [sp_A_Activity] AS BEGIN SET NOCOUNT ON; END')
GO
Alter PROCEDURE sp_A_Activity   

AS

SELECT  'ALL' AS Id, 'ALL' AS Name
UNION
SELECT DISTINCT CONVERT(NVARCHAR(50),Id),Name
FROM dbo.tblActivityType 
ORDER BY Name

-- EXEC sp_A_Activity