IF NOT EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'sp_A_PurchasingClerks')
   exec('CREATE PROCEDURE [sp_A_PurchasingClerks] AS BEGIN SET NOCOUNT ON; END')
GO
Alter PROCEDURE sp_A_PurchasingClerks	 
	 @hubId varchar(50)=NULL 	 
AS
BEGIN
	declare @costcentreid table(ids varchar(50));
	 
    if(@hubId='' or @hubId='ALL') set @hubId=null;
        
	SELECT  'ALL' AS PurchasingClerkId, 'ALL' AS Name
	UNION
	SELECT DISTINCT convert(varchar(50),clerk.Id) as id, clerk.UserName  AS Name
	FROM tblUsers AS clerk 
	INNER JOIN  dbo.tblCostCentre AS cc ON clerk.CostCenterId = cc.Id 
	where (cc.CostCentreType=10)	
	and(@hubId is null or  cc.parentCostCentreId = @hubId)
	order by name
END;

--  EXEC sp_A_PurchasingClerks  @hubId='ALL'

--  EXEC sp_A_PurchasingClerks  @hubId='E6834108-4BA7-4A1A-98CA-9DD4DF8D68E3'

