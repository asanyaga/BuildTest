IF NOT EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'sp_A_CommodityReceptionByTransaction')
   exec('CREATE PROCEDURE [sp_A_CommodityReceptionByTransaction] AS BEGIN SET NOCOUNT ON; END')
GO
Alter PROCEDURE sp_A_CommodityReceptionByTransaction
    @startDate varchar(50)=NULL,
    @endDate varchar(50)=NULL ,
    @hubId varchar(50)=NULL ,
    @routeId varchar(50)=NULL ,
    @centreId varchar(50)=NULL ,
    @ownerId varchar(50)=NULL
AS
BEGIN
    
    IF(@hubId='ALL' or @hubId='')	SET @hubId=Null;
    IF(@routeId='ALL' or @routeId='')	SET @routeId=Null;
    IF(@centreId='ALL' or @centreId='')	SET @centreId=Null;
    IF(@ownerId='ALL' or @ownerId='')	SET @ownerId=Null;
	IF(isdate(@startDate)=0) SET @startDate=null;
	IF(isdate(@endDate)=0)	SET @endDate=null;
			
	select convert(varchar(50),doc.id) as documentId, 
	doc.documentReference,
	sum(item.weight) as TotalWeight
	from tblcostcentre cc 
	left JOIN tblSourcingDocument  doc ON  cc.id= doc.DocumentRecipientCostCentreId or cc.id =doc.[DocumentIssuerCostCentreId]
	left JOIN tblSourcingLineItem  item ON doc.Id = item.DocumentId 
	where cc.costcentretype= 8 and doc.DocumentTypeId = 17 	
	and  ((@startDate is null or @endDate is null) or doc.DocumentDate between @startDate and  @endDate)
	and  (@routeId is null  or doc.RouteId=@routeId)
	and  (@hubId is null  or cc.id=@hubId)
	and  (@centreId is null  or doc.centreid=@centreId)
	and  (@ownerId is null  or doc.commodityOwnerId=@ownerId)
	Group by  doc.id,doc.documentReference
END;
--EXEC sp_A_CommodityReceptionByTransaction @startDate = '10-Jan-2013', @endDate = '10-Dec-2013',@routeid='    '
--EXEC sp_A_CommodityReceptionByTransaction @startDate = '     ', @endDate = '',@routeid='    '
--EXEC sp_A_CommodityReceptionByTransaction 
--EXEC sp_A_CommodityReceptionByTransaction @startDate = '10-Jan-2013', @endDate = '10-Dec-2013' ,@routeid='9E67C9B7-0660-406D-B17D-9E227C58F424'
--EXEC sp_A_CommodityReceptionByTransaction @hubId='E6834108-4BA7-4A1A-98CA-9DD4DF8D68E2'