IF NOT EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'sp_A_CommodityGrade')
   exec('CREATE PROCEDURE [sp_A_CommodityGrade] AS BEGIN SET NOCOUNT ON; END')
GO

Alter PROCEDURE sp_A_CommodityGrade   
 @commodityId varchar(50)=null
AS
BEGIN	
	 IF(@commodityId='ALL' or @commodityId='')	SET @commodityId=Null;	
	SELECT  'ALL' AS Id, 'ALL' AS Name
	UNION
	select convert(varchar(50),id) as Id , name  from tblCommodityGrade
	where 1=1
	and  (@commodityId is null  or commodityId=@commodityId )	
	order by name	
END;

--EXEC sp_A_CommodityGrade @commodityId='E6834108-4BA7-4A1A-98CA-9DD4DF8D68E2'
--EXEC sp_A_CommodityGrade 
