IF NOT EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'sp_A_Commodity')
   exec('CREATE PROCEDURE [sp_A_Commodity] AS BEGIN SET NOCOUNT ON; END')
GO

Alter PROCEDURE sp_A_Commodity   
 
AS
BEGIN		
	SELECT  'ALL' AS Id, 'ALL' AS Name
	UNION
	select convert(varchar(50),id) as Id , name  from tblCommodity	
	order by name	
END;

--EXEC sp_A_Commodity @hubId='E6834108-4BA7-4A1A-98CA-9DD4DF8D68E2'
--EXEC sp_A_Commodity 
