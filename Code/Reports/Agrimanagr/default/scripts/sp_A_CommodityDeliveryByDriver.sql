IF NOT EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'sp_A_CommodityDeliveryByDriver')
   exec('CREATE PROCEDURE [sp_A_CommodityDeliveryByDriver] AS BEGIN SET NOCOUNT ON; END')
GO
Alter PROCEDURE sp_A_CommodityDeliveryByDriver
    @startDate varchar(50)=NULL,
    @endDate varchar(50)=NULL,
    @hubId varchar(50)=NULL,
    @routeId varchar(50)=NULL,
    @centreId varchar(50)=NULL,
    @driverId varchar(50)=NULL
AS
BEGIN    
    IF(@hubId='ALL' or @hubId='')	SET @hubId=Null;
    IF(@routeId='ALL' or @routeId='')	SET @routeId=Null;
    IF(@centreId='ALL' or @centreId='')	SET @centreId=Null;
	IF(isdate(@startDate)=0) SET @startDate=null;
	IF(isdate(@endDate)=0)	SET @endDate=null;
	select  convert(varchar(50),doc.DriverName) as driverName, 
	convert(varchar(50),cc.id) AS hubId,
	convert(varchar(50),doc.Routeid) AS routeId, 
	convert(varchar(50),doc.CentreId) AS centreId,
	(select convert(varchar(50), Id) from tblUsers where UserName = doc.DriverName) as driverId,
	sum(item.weight) as TotalWeight
	from tblcostcentre cc 
	left JOIN tblSourcingDocument  doc ON cc.Id = doc.DocumentIssuerCostCentreId or  cc.Id =DocumentRecipientCostCentreId
	left JOIN tblSourcingLineItem  item ON doc.Id = item.DocumentId 
	left join tblUsers usr on usr.UserName = doc.drivername
	where cc.costcentretype= 8 and doc.DocumentTypeId = 16
	and  ((@startDate is null or @endDate is null) or doc.DocumentDate between @startDate and  @endDate)
	and  (@routeId is null  or doc.RouteId=@routeId)
	and  (@hubId is null  or cc.id=@hubId)
	and  (@centreId is null  or doc.centreid=@centreId)
	Group by  cc.id, doc.Routeid, doc.CentreId, doc.DriverName
END;
--EXEC sp_A_CommodityDeliveryByDriver @startDate = '     ', @endDate = '',@routeid='    '
