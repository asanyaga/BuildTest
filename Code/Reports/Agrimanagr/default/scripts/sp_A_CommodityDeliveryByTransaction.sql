IF NOT EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'sp_A_CommodityDeliveryByTransaction')
   exec('CREATE PROCEDURE [sp_A_CommodityDeliveryByTransaction] AS BEGIN SET NOCOUNT ON; END')
GO
Alter PROCEDURE sp_A_CommodityDeliveryByTransaction
    @startDate varchar(50)=NULL,
    @endDate varchar(50)=NULL ,
    @hubId varchar(50)=NULL ,
    @routeId varchar(50)=NULL ,
    @centreId varchar(50)=NULL ,
    @driverId varchar(50)=NULL
AS
BEGIN
    
    IF(@hubId='ALL' or @hubId='')	SET @hubId=Null;
    IF(@routeId='ALL' or @routeId='')	SET @routeId=Null;
    IF(@centreId='ALL' or @centreId='')	SET @centreId=Null;
    IF(@driverId='ALL' or @driverId='')	SET @driverId=Null;
	IF(isdate(@startDate)=0) SET @startDate=null;
	IF(isdate(@endDate)=0)	SET @endDate=null;
			
	select convert(varchar(50),doc.id) as documentId, doc.documentReference,
	sum(item.weight) as TotalWeight
	from tblcostcentre cc 
	INNER JOIN tblSourcingDocument  doc ON cc.Id = doc.DocumentIssuerCostCentreId or  cc.Id =DocumentRecipientCostCentreId
	INNER JOIN tblSourcingLineItem  item ON doc.Id = item.DocumentId 
	INNER JOIN tblUsers usr on doc.DriverName=usr.UserName
	where cc.costcentretype= 8 and doc.DocumentTypeId = 16
	AND  ((@startDate is null or @endDate is null) or doc.DocumentDate between @startDate and  @endDate)
	AND  (@routeId is null  or doc.RouteId=@routeId)
	AND  (@hubId is null  or cc.id=@hubId)
	AND  (@centreId is null  or doc.centreid=@centreId)
	AND  (@driverId is null  or usr.Id=@driverId)
	Group by  doc.id, doc.documentReference
END;
--EXEC sp_A_CommodityDeliveryByTransaction @startDate = '10-Jan-2013', @endDate = '10-Dec-2013',@routeid='    '
--EXEC sp_A_CommodityDeliveryByTransaction @startDate = '     ', @endDate = '',@routeid='    '
--EXEC sp_A_CommodityDeliveryByTransaction @hubId='all', @routeId='all', @centreId='all', @driverId='all'
--EXEC sp_A_CommodityDeliveryByTransaction @startDate = '10-Jan-2013', @endDate = '10-Dec-2013' ,@routeid='9E67C9B7-0660-406D-B17D-9E227C58F424'
--EXEC sp_A_CommodityDeliveryByTransaction @hubId='E6834108-4BA7-4A1A-98CA-9DD4DF8D68E2'