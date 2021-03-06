IF NOT EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'sp_A_CommodityPurchaseByFactory')
   exec('CREATE PROCEDURE [sp_A_CommodityPurchaseByFactory] AS BEGIN SET NOCOUNT ON; END')
GO
Alter PROCEDURE sp_A_CommodityPurchaseByFactory
    @startDate varchar(50)=NULL,
    @endDate varchar(50)=NULL ,
    @hubId varchar(50)=NULL 
AS
BEGIN
    PRINT 'startDate ='+ isnull(@startdate,'NULL');
    PRINT 'endDate ='+ isnull(@enddate,'NULL');
    PRINT 'hubid ='+ isnull(@hubid,'NULL');
    IF(@hubId='ALL' or @hubId='')	SET @hubId=Null;
	IF(isdate(@startDate)=0) SET @startDate=null;
	IF(isdate(@endDate)=0)	SET @endDate=null;
		
	select convert(varchar(50),cc.id) as HubId, cc.name as Hub ,sum(item.weight) as TotalWeight
	from tblcostcentre cc 
	left JOIN tblSourcingDocument  doc ON cc.Id = doc.DocumentIssuerCostCentreId or  cc.Id =DocumentRecipientCostCentreId
	left JOIN tblSourcingLineItem  item ON doc.Id = item.DocumentId 
	where cc.costcentretype= 8 and doc.DocumentTypeId = 13	
	and  ((@startDate is null or @endDate is null) or doc.DocumentDate between @startDate and  @endDate)
	and  (@hubId is null  or cc.id=@hubId )
	group by doc.DocumentDate,cc.id, cc.name
	ORDER BY doc.DocumentDate ASC	
END;
--EXEC sp_A_CommodityPurchaseByFactory @startDate = '10-Jan-2013', @endDate = '10-Dec-2013',@hubId='E6834108-4BA7-4A1A-98CA-9DD4DF8D68E2'
--EXEC sp_A_CommodityPurchaseByFactory @hubId='ss'
--EXEC sp_A_CommodityPurchaseByFactory @startDate = '10-Jan-2013', @endDate = '10-Dec-2011'
