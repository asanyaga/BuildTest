IF NOT EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'sp_A_Routes')
   exec('CREATE PROCEDURE [sp_A_Routes] AS BEGIN SET NOCOUNT ON; END')
GO

Alter PROCEDURE sp_A_Routes   
 @HubId varchar(50)=NULL  
AS
BEGIN
	DECLARE  @regionId varchar(50)=null;		 		 
	PRINT 'hub id = ' +isNull(@HubId,'NULL');			
	 IF(@HubId!='ALL' and @HubId!='')
		 BEGIN		    
		   select @RegionId=convert(varchar(50),[Distributor_RegionId]) from tblcostcentre where id=@HubId
		   PRINT 'Region id ='+isNull(@RegionId,'NULL')	;	  
		 END;
	SELECT  'ALL' AS routeId, 'ALL' AS name
	UNION
	select LOWER(convert(varchar(50),routeId)) routeId, name from tblroutes
	where 1=1
	and (@RegionId is null or RegionId= @RegionId)
	order by name
	
END;

--EXEC sp_A_Routes @hubId='E6834108-4BA7-4A1A-98CA-9DD4DF8D68E2'
--EXEC sp_A_Routes 
