IF NOT EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'sp_A_CommodityPurchaseByCentre')
   exec('CREATE PROCEDURE [sp_A_CommodityPurchaseByCentre] AS BEGIN SET NOCOUNT ON; END')
GO
Alter PROCEDURE sp_A_CommodityPurchaseByCentre
    @startDate varchar(50)=NULL,
    @endDate varchar(50)=NULL ,
    @hubId varchar(50)=NULL ,
    @routeId varchar(50)=NULL ,
    @centreId varchar(50)=NULL 
AS
BEGIN
    
    IF(@hubId='ALL' or @hubId='')	SET @hubId=Null;
    IF(@routeId='ALL' or @routeId='')	SET @routeId=Null;
    IF(@centreId='ALL' or @centreId='')	SET @centreId=Null;
	IF(isdate(@startDate)=0) SET @startDate=null;
	IF(isdate(@endDate)=0)	SET @endDate=null;
	IF(@centreId='ALL' or @centreId='')	SET @centreId=Null;
		
	select convert(varchar(50),doc.Routeid) as RouteId, 
	convert(varchar(50),cc.id) AS hubId,
	(select name from tblroutes where RouteId= doc.RouteId) as RouteName,
	(select name from tblcentre where id= doc.centreid) as centreName, 
	convert(varchar(50),doc.centreid) as centreid,	
	sum(item.weight) as TotalWeight
	from tblcostcentre cc 
	left JOIN tblSourcingDocument  doc ON cc.Id = doc.DocumentIssuerCostCentreId or  cc.Id =DocumentRecipientCostCentreId
	left JOIN tblSourcingLineItem  item ON doc.Id = item.DocumentId 
	where cc.costcentretype= 8 and doc.DocumentTypeId = 13 	
	and  ((@startDate is null or @endDate is null) or doc.DocumentDate between @startDate and  @endDate)
	and  (@routeId is null  or doc.RouteId=@routeId)
	and  (@hubId is null  or cc.id=@hubId)
	and  (@centreId is null  or doc.centreid=@centreId)
	Group by  doc.DocumentDate,cc.id,doc.RouteId, doc.centreid
	ORDER BY doc.DocumentDate ASC
END;
--EXEC sp_A_CommodityPurchaseByCentre @startDate = '10-Jan-2013', @endDate = '10-Dec-2013',@routeid='    '
--EXEC sp_A_CommodityPurchaseByCentre @startDate = '     ', @endDate = '',@routeid='    '
--EXEC sp_A_CommodityPurchaseByCentre 
--EXEC sp_A_CommodityPurchaseByCentre @startDate = '10-Jan-2013', @endDate = '10-Dec-2013' ,@routeid='9E67C9B7-0660-406D-B17D-9E227C58F424'
--EXEC sp_A_CommodityPurchaseByCentre @hubId='E6834108-4BA7-4A1A-98CA-9DD4DF8D68E2'