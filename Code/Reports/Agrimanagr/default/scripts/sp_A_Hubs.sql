
IF NOT EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'sp_A_Hubs')
   exec('CREATE PROCEDURE [sp_A_Hubs] AS BEGIN SET NOCOUNT ON; END')
GO
Alter PROCEDURE sp_A_Hubs	     
AS
BEGIN	
	SELECT  'ALL' AS hubId, 'ALL' AS hub
	UNION
	select LOWER(convert(varchar(50),id)) as hubId, Name as hub from tblCostCentre 
	where  costcentretype=8	
	order by Hub
END;
--EXEC sp_A_Hubs