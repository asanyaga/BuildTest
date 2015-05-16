IF NOT EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'sp_A_TransactionsByCentre')
   exec('CREATE PROCEDURE [sp_A_TransactionsByCentre] AS BEGIN SET NOCOUNT ON; END')
GO
Alter PROCEDURE sp_A_TransactionsByCentre
	@startDate datetime,
    @endDate datetime ,
    @hubId varchar(50)=NULL ,
    @routeId varchar(50)=NULL ,
    @centreId varchar(50)=NULL,
	@farmerId varchar(50)=NULL
AS
BEGIN    
    IF(@hubId='ALL' OR @hubId='')	SET @hubId=NULL;
    IF(@routeId='ALL' OR @routeId='')	SET @routeId=NULL;
    IF(@centreId='ALL' OR @centreId='')	SET @centreId=NULL;
    IF(@farmerId='ALL' OR @farmerId='')	SET @farmerId=NULL;

	SELECT convert(varchar(50),doc.id) AS DocId, 
	cc.Cost_Centre_Code AS ClerkCode,
	doc.documentReference AS DocRef,	
	doc.documentDate AS DocDate,
	cc.name AS Factory,
    doc.DocumentIssuerCostCentreId,
	(SELECT id FROM tblCommodityOwner WHERE id=doc.CommodityOwnerId ) AS FarmerId,
	(SELECT surname+' '+firstname+' '+isnull(lastname,'') FROM tblCommodityOwner WHERE id=doc.CommodityOwnerId ) AS FarmerName,
    (SELECT name FROM tblCostCentre WHERE id = doc.DocumentIssuerCostCentreId) As ClerkName,
	(SELECT Code FROM tblCommodityOwner WHERE id=doc.CommodityOwnerId ) AS FarmerCode,
	(SELECT name FROM tblRoutes WHERE routeid =doc.routeid) AS RouteName,
	(SELECT name FROM tblCentre WHERE id =doc.centreid) AS CentreName,
	(SELECT name FROM tblCommodity WHERE id =item.CommodityId) AS Commodity,
	(SELECT name FROM tblCommodityGrade WHERE id =item.GradeId) AS Grade,
	item.weight AS Weight,
	item.TareWeight
	FROM tblcostcentre cc 
	INNER JOIN tblSourcingDocument  doc ON cc.Id = doc.DocumentIssuerCostCentreId or  cc.Id =doc.DocumentRecipientCostCentreId
	INNER JOIN tblSourcingLineItem  item ON doc.Id = item.DocumentId 
	WHERE cc.costcentretype= 8 AND doc.DocumentTypeId = 13 
	AND convert(nvarchar(26),doc.DocumentDate,23) between @startDate and @endDate
	--AND ((@startDate is null or @endDate is null) or doc.DocumentDate between @startDate and  @endDate)
	AND (@centreId IS NULL  OR doc.centreid=@centreId)
	AND (@routeId IS NULL  OR doc.RouteId=@routeId)
	AND (@hubId IS NULL  OR cc.id=@hubId)
	AND (@farmerId IS NULL OR doc.CommodityOwnerId=@farmerId)
END;
--EXEC sp_A_TransactionsByCentre @startDate = '     ', @endDate = ''
