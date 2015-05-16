IF NOT EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'sp_A_DailyTotalsByRoute')
   exec('CREATE PROCEDURE [sp_A_DailyTotalsByRoute] AS BEGIN SET NOCOUNT ON; END')
GO
Alter PROCEDURE sp_A_DailyTotalsByRoute
    @startDate VARCHAR(50)=NULL,
    @endDate VARCHAR(50)=NULL
AS
BEGIN
    DECLARE @table1 TABLE(
	 RouteId VARCHAR(50),
	 RouteName VARCHAR(100),
	 DateIssued VARCHAR(100),
	 DeliveryDate VARCHAR(50),
	 CentreName VARCHAR(100),
	 RouteWeight DECIMAL(18,2),
	 LorryWeight DECIMAL(18,2),
	 FactoryWeight DECIMAL(18,2)
	);
    PRINT 'startDate ='+ ISNULL(@startdate,'NULL');
    PRINT 'endDate ='+ ISNULL(@enddate,'NULL');   
	IF(ISDATE(@startDate)=0) SET @startDate=NULL;
	IF(ISDATE(@endDate)=0)	SET @endDate=NULL;
	INSERT @table1 
	SELECT DISTINCT r.routeid AS RouteId, name AS RouteName,
	DateIssued,
	CONVERT(DATE,DateIssued, 101) AS DeliveryDate,
	(SELECT name FROM tblCentre where id =doc.centreid) as CentreName,
	ISNULL((SELECT SUM(item.weight) 
		FROM tblSourcingDocument doc 
		INNER JOIN tblSourcingLineItem item ON doc.id = item.DocumentId
		WHERE doc.DocumentTypeId=13 
		AND  ((@startDate IS NULL OR @endDate IS NULL) OR doc.DocumentDate BETWEEN @startDate AND  @endDate)
		AND  (doc.routeid=r.routeid)
	),0) AS RouteWeight,
	ISNULL((SELECT SUM(item.weight)  
		FROM tblSourcingDocument doc 
		INNER JOIN tblSourcingLineItem item on doc.id = item.DocumentId
		where doc.DocumentTypeId=16 
		AND  ((@startDate IS NULL OR @endDate is NULL) OR doc.DocumentDate BETWEEN @startDate AND  @endDate)
		),0) AS LorryWeight,
	ISNULL((SELECT SUM(item.weight) 
		FROM tblSourcingDocument doc 
		INNER JOIN tblSourcingLineItem item on doc.id = item.DocumentId
		where doc.DocumentTypeId=17 
		AND  ((@startDate IS NULL OR @endDate IS NULL) OR doc.DocumentDate BETWEEN @startDate AND  @endDate)
		),0) AS FactoryWeight	

    FROM tblRoutes r
    INNER JOIN tblSourcingDocument doc on doc.routeid = r.routeid
    AND ((@startDate IS NULL OR @endDate IS NULL) OR doc.DocumentDate BETWEEN @startDate AND  @endDate)
    AND (doc.DocumentTypeId = 13)
    END;
  
	SELECT DateIssued, DeliveryDate, RouteId, RouteWeight, LorryWeight, FactoryWeight, RouteName, CentreName
		,(LorryWeight-RouteWeight) AS VarianceBA 
		,(FactoryWeight-RouteWeight) AS VarianceCA 
		,((FactoryWeight-RouteWeight)/NULLIF(RouteWeight,0)) AS [CA/A]
	FROM @table1
--EXEC sp_A_DailyTotalsByRoute 
