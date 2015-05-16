IF NOT EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'sp_A_CommodityDeliveryByRoute')
   exec('CREATE PROCEDURE [sp_A_CommodityDeliveryByRoute] AS BEGIN SET NOCOUNT ON; END')
GO
Alter PROCEDURE sp_A_CommodityDeliveryByRoute
    @startDate varchar(50)=NULL,
    @endDate varchar(50)=NULL ,
    @hubId varchar(50)=NULL ,
    @routeId varchar(50)=NULL 
AS
BEGIN    
    IF(@routeId='ALL' or @routeId='') SET @routeId=NULL;
    IF(@hubId='ALL' or @hubId='')SET @hubId=NULL;
	IF(isdate(@startDate)=0) SET @startDate=NULL;
	IF(isdate(@endDate)=0)	SET @endDate=NULL;
	
	SELECT convert(varchar(50),doc.RouteId) AS routeId,
	convert(varchar(50),cc.id) AS hubId,
	isnull((select name from tblroutes where RouteId= doc.RouteId),'Default Route') as RouteName,
	sum(item.weight) AS TotalWeight
	FROM tblCostCentre cc 
	LEFT JOIN tblSourcingDocument  doc ON cc.Id = doc.DocumentIssuerCostCentreId OR cc.Id =DocumentRecipientCostCentreId
	LEFT JOIN tblSourcingLineItem  item ON doc.Id = item.DocumentId 
	where cc.costcentretype= 8 AND doc.DocumentTypeId = 16 	
	AND  ((@startDate IS NULL OR @endDate IS NULL) OR doc.DocumentDate BETWEEN @startDate AND  @endDate)
	AND  (@routeId IS NULL  OR doc.RouteId=@routeId)
	AND  (@hubId IS NULL  OR cc.id=@hubId)
	GROUP BY  cc.Id, doc.RouteId
END;
--EXEC sp_A_CommodityDeliveryByRoute @startDate = '1-Sep-2013', @endDate = '30-Sep-2013',@hubid='1180AE48-D288-4BCA-A998-C2D594AB45D9'
