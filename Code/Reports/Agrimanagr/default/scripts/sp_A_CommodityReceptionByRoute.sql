IF NOT EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'sp_A_CommodityReceptionByRoute')
   exec('CREATE PROCEDURE [sp_A_CommodityReceptionByRoute] AS BEGIN SET NOCOUNT ON; END')
GO
Alter PROCEDURE sp_A_CommodityReceptionByRoute
    @startDate varchar(50)=NULL,
    @endDate varchar(50)=NULL ,
    @hubId varchar(50)=NULL ,
    @routeId varchar(50)=NULL 
AS
BEGIN    
    IF(@routeId='ALL' or @routeId='')	SET @routeId=Null;
    IF(@hubId='ALL' or @hubId='')	SET @hubId=null;
	IF(isdate(@startDate)=0) SET @startDate=null;
	IF(isdate(@endDate)=0)	SET @endDate=null;		
	select convert(varchar(50),doc.Routeid) as RouteId, 
	convert(varchar(50),cc.id) AS hubId,	
	isnull((select name from tblroutes where RouteId= doc.RouteId),'Default Route') as RouteName,
	sum(item.weight) as TotalWeight
	from tblcostcentre cc 
	left JOIN tblSourcingDocument  doc on  cc.id= doc.DocumentRecipientCostCentreId or cc.id =doc.[DocumentIssuerCostCentreId]
	left JOIN tblSourcingLineItem  item on doc.Id = item.DocumentId 
	where cc.costcentretype= 8 and doc.DocumentTypeId =17 	
	and  ((@startDate is null or @endDate is null) or doc.DocumentDate between @startDate and  @endDate)
	and  (@routeId is null  or doc.RouteId=@routeId)
	and  (@hubId is null  or cc.id=@hubId)
	group by  cc.id, doc.RouteId
END;

--EXEC sp_A_CommodityReceptionByRoute @startDate = '10-Jan-2013', @endDate = '12-Jan-2013',@routeid='34AEB6E0-3785-4FAB-9D5D-F99D049F3441'
----EXEC sp_A_CommodityReceptionByRoute 
--EXEC sp_A_CommodityReceptionByRoute @startDate = '10-Jan-2013', @endDate = '10-Dec-2013' ,@routeid='9E67C9B7-0660-406D-B17D-9E227C58F424'
--EXEC sp_A_CommodityReceptionByRoute @startDate = '10-Jan-2013', @endDate = '10-Dec-2013' ,@hubid='b29aafc9-777c-486c-9d4b-7cc0a0089370'