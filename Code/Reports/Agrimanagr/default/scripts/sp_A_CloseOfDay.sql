IF NOT EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'sp_A_CloseOfDay')
   exec('CREATE PROCEDURE [sp_A_CloseOfDay] AS BEGIN SET NOCOUNT ON; END')
GO
Alter PROCEDURE sp_A_CloseOfDay
    @startDate varchar(50)=NULL,
    @endDate varchar(50)=NULL ,
    @hubId varchar(50)=NULL ,
    @routeId varchar(50)=NULL ,
    @centreId varchar(50)=NULL 
AS
BEGIN    
    IF(@hubId='ALL' OR @hubId='') SET @hubId=NULL;
    IF(@routeId='ALL' OR @routeId='') SET @routeId=NULL;
    IF(@centreId='ALL' OR @centreId='')	SET @centreId=NULL;
	IF(isdate(@startDate)=0) SET @startDate=NULL;
	IF(isdate(@endDate)=0) SET @endDate=NULL;
	
	SELECT convert(varchar(50),doc.id) AS DocId, 
	(SELECT isnull(surname,'')+' '+isnull(firstname,'')+' '+isnull(lastname,'') FROM tblCommodityOwner WHERE id=doc.CommodityOwnerId ) AS FarmerName,
	(SELECT name from tblRoutes WHERE routeid =doc.routeid) AS RouteName,
	(SELECT name from tblCentre WHERE id =doc.centreid) AS CentreName,
	(SELECT name from tblCommodity WHERE id =item.CommodityId) AS CommodityName,
	(SELECT name from tblCommodityGrade WHERE id =item.GradeId) AS GradeName,
	item.TareWeight, item.Weight
	FROM tblcostcentre cc 
	INNER JOIN tblSourcingDocument  doc ON cc.Id = doc.DocumentIssuerCostCentreId OR  cc.Id =DocumentRecipientCostCentreId
	INNER JOIN tblSourcingLineItem  item ON doc.Id = item.DocumentId 
	WHERE cc.costcentretype= 8 AND doc.DocumentTypeId = 13 	
	AND  ((@startDate IS NULL OR @endDate IS NULL) or doc.DocumentDate BETWEEN @startDate AND  @endDate)
	AND  (@routeId IS NULL  OR doc.RouteId=@routeId)
	AND  (@hubId IS NULL  OR cc.id=@hubId)
	AND  (@centreId IS NULL  OR doc.centreid=@centreId)		
END;
--EXEC sp_A_CloseOfDay 
