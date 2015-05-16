IF NOT EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'sp_A_Stores')
   exec('CREATE PROCEDURE [sp_A_Stores] AS BEGIN SET NOCOUNT ON; END')
GO

Alter PROCEDURE sp_A_Stores   
 @hubId varchar(50)=NULL  
AS
BEGIN
			 		 
	PRINT 'hub id = ' +isNull(@hubId,'NULL');			
	 IF(@hubId='ALL' or @hubId='') set @hubId=null;		
	SELECT  'ALL' AS storeId, 'ALL' AS store
	UNION
	select LOWER(convert(varchar(50),id)) as storeId , name as store from tblcostcentre
	where costcentretype=11
	and (@hubId is null or parentCostCentreId= @hubId)
	order by store
	
END;

--EXEC sp_A_Stores @hubId='E6834108-4BA7-4A1A-98CA-9DD4DF8D68E2'
--EXEC sp_A_Stores @hubId='ALL'
