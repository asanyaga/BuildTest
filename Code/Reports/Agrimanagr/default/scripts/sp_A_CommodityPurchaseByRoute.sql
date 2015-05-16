

IF NOT EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'sp_A_CommodityPurchaseByRoute')
   exec('CREATE PROCEDURE [sp_A_CommodityPurchaseByRoute] AS BEGIN SET NOCOUNT ON; END')
GO
Alter PROCEDURE sp_A_CommodityPurchaseByRoute
    @startDate varchar(50)=NULL,
    @endDate varchar(50)=NULL ,
    @hubId varchar(50)=NULL ,
    @routeId varchar(50)=NULL 
AS
BEGIN
    
    IF(@routeId='ALL' or @routeId='')	SET @routeId=Null;
    IF(@hubId='ALL' or @hubId='')	SET @hubId=Null;
	IF(isdate(@startDate)=0) SET @startDate=null;
	IF(isdate(@endDate)=0)	SET @endDate=null;
		
	select convert(varchar(50),doc.Routeid) as RouteId, 
	convert(varchar(50),cc.id) AS hubId,
	isnull((select name from tblroutes where RouteId= doc.RouteId),'Default Route') as RouteName,
	sum(item.weight) as TotalWeight
	from tblcostcentre cc 
	left JOIN tblSourcingDocument  doc ON cc.Id = doc.DocumentIssuerCostCentreId or  cc.Id =DocumentRecipientCostCentreId
	left JOIN tblSourcingLineItem  item ON doc.Id = item.DocumentId 
	where cc.costcentretype= 8 and doc.DocumentTypeId = 13 	
	and  ((@startDate is null or @endDate is null) or doc.DocumentDate between @startDate and  @endDate)
	and  (@routeId is null  or doc.RouteId=@routeId)
	and  (@hubId is null  or cc.id=@hubId)
	Group by  doc.DocumentDate,cc.id, doc.RouteId
	ORDER BY doc.DocumentDate ASC
END;
--EXEC sp_A_CommodityPurchaseByRoute @startDate = '10-Jan-2013', @endDate = '10-Dec-2013',@routeid='    '
--EXEC sp_A_CommodityPurchaseByRoute @startDate = '     ', @endDate = '',@routeid='    '
--EXEC sp_A_CommodityPurchaseByRoute 
--EXEC sp_A_CommodityPurchaseByRoute @startDate = '10-Jan-2013', @endDate = '10-Dec-2013' ,@routeid='9E67C9B7-0660-406D-B17D-9E227C58F424'
--EXEC sp_A_CommodityPurchaseByRoute @startDate = '10-Jan-2013', @endDate = '10-Dec-2013' ,@hubid='E6834108-4BA7-4A1A-98CA-9DD4DF8D68E3'
