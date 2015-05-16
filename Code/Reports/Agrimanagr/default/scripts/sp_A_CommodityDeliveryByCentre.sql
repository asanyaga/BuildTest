IF NOT EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'sp_A_CommodityDeliveryByCentre')
   exec('CREATE PROCEDURE [sp_A_CommodityDeliveryByCentre] AS BEGIN SET NOCOUNT ON; END')
GO
Alter PROCEDURE sp_A_CommodityDeliveryByCentre
    @startDate varchar(50)=NULL,
    @endDate varchar(50)=NULL ,
    @hubId varchar(50)=NULL ,
    @routeId varchar(50)=NULL ,
    @centreId varchar(50)=NULL 
AS
BEGIN
    IF(@hubId='ALL' OR @hubId='')	SET @hubId=NULL;
    IF(@routeId='ALL' OR @routeId='')	SET @routeId=NULL;
    IF(@centreId='ALL' OR @centreId='')	SET @centreId=NULL;
	IF(isdate(@startDate)=0) SET @startDate=NULL;
	IF(isdate(@endDate)=0)	SET @endDate=NULL;
	IF(@centreId='ALL' OR @centreId='')	SET @centreId=NULL;
		
	SELECT convert(varchar(50),doc.Routeid) AS routeId, 
	convert(varchar(50),cc.id) AS hubId,
	(SELECT name FROM tblroutes WHERE RouteID = doc.RouteId) AS RouteName,
	(SELECT name FROM tblcentre WHERE id= doc.centreid) AS centreName, 	
	convert(varchar(50),doc.CentreId) AS centreId,	
	sum(item.weight) AS TotalWeight
	FROM tblcostcentre cc 
	LEFT JOIN tblSourcingDocument  doc ON cc.Id = doc.DocumentISsuerCostCentreId OR  cc.Id =DocumentRecipientCostCentreId
	LEFT JOIN tblSourcingLineItem  item ON doc.Id = item.DocumentId 
	LEFT JOIN tblroutes rt ON cc.RouteId = rt.RouteID 
	WHERE cc.costcentretype= 8 
	AND doc.DocumentTypeId = 16 	
	AND  ((@startDate IS NULL OR @endDate IS NULL) OR doc.DocumentDate between @startDate AND  @endDate)
	AND  (@routeId IS NULL  OR doc.RouteId = @routeId)
	AND  (@hubId IS NULL  OR cc.id=@hubId)
	AND  (@centreId IS NULL  OR doc.centreid=@centreId)
	Group by  cc.id, doc.RouteId, doc.centreid
END;
--EXEC sp_A_CommodityDeliveryByCentre @centreId='B4E16809-E97F-4E77-BC0F-E77A0946B941'