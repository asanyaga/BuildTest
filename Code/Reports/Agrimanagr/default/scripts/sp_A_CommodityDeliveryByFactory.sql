IF NOT EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'sp_A_CommodityDeliveryByFactory')
   exec('CREATE PROCEDURE [sp_A_CommodityDeliveryByFactory] AS BEGIN SET NOCOUNT ON; END')
GO
Alter PROCEDURE sp_A_CommodityDeliveryByFactory
    @startDate varchar(50)=NULL,
    @endDate varchar(50)=NULL ,
    @hubId varchar(50)=NULL 
AS
BEGIN
    PRINT 'startDate ='+ isnull(@startdate,'NULL');
    PRINT 'endDate ='+ isnull(@enddate,'NULL');
    PRINT 'hubid ='+ isnull(@hubid,'NULL');
    IF(@hubId='ALL' or @hubId='')	SET @hubId=NULL;
	IF(isdate(@startDate)=0) SET @startDate=NULL;
	IF(isdate(@endDate)=0)	SET @endDate=NULL;
		
	SELECT convert(varchar(50), cc.id) AS HubId, cc.name AS Hub ,SUM(item.weight) AS TotalWeight
	FROM tblcostcentre cc 
	LEFT JOIN tblSourcingDocument  doc ON cc.Id = doc.DocumentIssuerCostCentreId OR  cc.Id =DocumentRecipientCostCentreId
	LEFT JOIN tblSourcingLineItem  item ON doc.Id = item.DocumentId 
	WHERE cc.costcentretype= 8 AND doc.DocumentTypeId = 16	
	AND  ((@startDate IS NULL or @endDate IS NULL) OR doc.DocumentDate BETWEEN @startDate AND  @endDate)
	AND  (@hubId IS NULL  OR cc.id=@hubId )
	GROUP BY cc.id, cc.name	
END;
--EXEC sp_A_CommodityDeliveryByFactory @hubId='F27D497A-6E16-467A-9435-9473B1BB3CC4'
