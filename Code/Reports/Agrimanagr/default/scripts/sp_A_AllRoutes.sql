IF NOT EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'sp_A_AllRoutes')
   exec('CREATE PROCEDURE [sp_A_AllRoutes] AS BEGIN SET NOCOUNT ON; END')
GO

Alter PROCEDURE sp_A_AllRoutes   
 @hubId varchar(50)=NULL  
AS
BEGIN
	DECLARE  @regionId varchar(50)=null;		 		 
	PRINT 'hub id = ' +isNull(@hubId,'NULL');			
	 IF(@hubId!='ALL' and @hubId!='')
		 BEGIN		    
		   select @regionId=convert(varchar(50),[Distributor_RegionId]) from tblcostcentre where id=@hubId
		   PRINT 'Region id ='+isNull(@regionId,'NULL')	;	  
		 END;
	SELECT  'ALL' AS routeId, 'ALL' AS name
	UNION
	select LOWER(convert(varchar(50),routeId)) routeId, name from tblroutes
	where 1=1
	and (@regionId is null or RegionId= @regionId)
	order by name
	
END;

--EXEC sp_A_AllRoutes @hubId='E6834108-4BA7-4A1A-98CA-9DD4DF8D68E2'
--EXEC sp_A_AllRoutes 
