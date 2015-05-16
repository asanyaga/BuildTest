IF NOT EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'sp_A_GainsLossesPerFieldClerk')
   exec('CREATE PROCEDURE [sp_A_GainsLossesPerFieldClerk] AS BEGIN SET NOCOUNT ON; END')
GO
Alter PROCEDURE sp_A_GainsLossesPerFieldClerk
    @startDate VARCHAR(50)=NULL,
    @endDate VARCHAR(50)=NULL,
    @routeId VARCHAR (50)= NULL,
	@clerkId VARCHAR(50)=NULL
AS
-- check start and end date
BEGIN
    DECLARE @table1 TABLE(
	 RouteId VARCHAR(50),
	 RouteName VARCHAR(100),
	 ClerkId VARCHAR(50),
	 ClerkName VARCHAR(100),
	 ClerkCode VARCHAR (50),
	 DocumentDate VARCHAR(50),
	 DeliveryDate VARCHAR(50),
	 ReceiptWeight DECIMAL(18,2),
	 LorryWeight DECIMAL(18,2),
	 FactoryWeight DECIMAL(18,2)
	);
    PRINT 'startDate ='+ ISNULL(@startdate,'NULL');
    PRINT 'endDate ='+ ISNULL(@enddate,'NULL');   
	IF(ISDATE(@startDate)=0)SET @startDate=NULL;
	IF(ISDATE(@endDate)=0)SET @endDate=NULL;
	IF(@routeId='ALL' or @routeId='') SET @routeId=NULL;
	IF LTRIM(RTRIM(@clerkId))='ALL'  begin set @clerkId='%' end
	INSERT @table1 
	SELECT DISTINCT   doc.routeid, 
	                  r.name, 
	                  cc.id AS ClerkId, 
	                  cc.name AS ClerkName, 
	                  cc.Cost_Centre_Code AS ClerkCode, 
	                  doc.DateIssued AS DocumentDate,
	CONVERT(DATE,DateIssued,101) AS DeliveryDate,
	
	ISNULL((SELECT SUM(item.weight) 
		    FROM tblSourcingDocument doc 
		    INNER JOIN tblSourcingLineItem item ON doc.id = item.DocumentId
		    WHERE doc.DocumentTypeId=13 
		    AND  ((@startDate IS NULL OR @endDate IS NULL) OR doc.DocumentDate BETWEEN @startDate AND  @endDate)
		    --AND (cc.CostCentreType = 10)
		    AND (cc.CostCentreType = 8)
	       ),0)   AS ReceiptWeight,
	       
	ISNULL((SELECT SUM(item.weight)  
		    FROM tblSourcingDocument doc 
		    INNER JOIN tblSourcingLineItem item ON doc.id = item.DocumentId
		    WHERE doc.DocumentTypeId=16 
		    AND ((@startDate IS NULL OR @endDate IS NULL) OR doc.DocumentDate BETWEEN @startDate AND  @endDate)
		    AND (cc.CostCentreType = 10)
	        ),0) AS LorryWeight,
	        
	ISNULL((SELECT SUM(item.weight) 
		    FROM tblSourcingDocument doc 
		    INNER JOIN tblSourcingLineItem item ON doc.id = item.DocumentId
		    WHERE doc.DocumentTypeId=17 
		    AND  ((@startDate IS NULL OR @endDate IS NULL) OR doc.DocumentDate BETWEEN @startDate AND  @endDate)
		    AND (cc.CostCentreType = 10)
	       ),0)	AS FactoryWeight
	       	
	FROM tblSourcingDocument doc
    INNER JOIN tblCostCentre cc ON cc.id = doc.DocumentIssuerCostCentreId OR cc.id=doc.DocumentRecipientCostCentreId
    INNER JOIN tblRoutes r ON r.routeid = doc.routeid
    AND  ((@startDate IS NULL OR @endDate IS NULL) OR doc.DocumentDate BETWEEN @startDate AND  @endDate)
    AND  (@routeId IS NULL OR doc.RouteId=@routeId)
    END;  
	SELECT RouteId, 
	       RouteName, 
	       DeliveryDate, 
		   SUM(ReceiptWeight) AS ReceiptWeight, 
		   SUM(LorryWeight) AS LorryWeight, 
		   SUM(FactoryWeight) AS FactoryWeight, 
		   ClerkId,
		   ClerkName,
		   ClerkCode,
		   SUM((LorryWeight-ReceiptWeight)) AS VarianceBA, 
		   SUM((FactoryWeight-ReceiptWeight)) AS VarianceCA,
		   SUM(((LorryWeight-ReceiptWeight)/NULLIF(ReceiptWeight,0))) AS [BA/A],
		   SUM(((FactoryWeight-ReceiptWeight)/NULLIF(ReceiptWeight,0))) AS [CA/A]	
	FROM @table1	
	WHERE FactoryWeight != 0  
	  AND LorryWeight != 0 
	  AND  ReceiptWeight != 0
	  AND CONVERT(nvarchar(50),ClerkId) LIKE ISNULL(@clerkId,'%') 
	GROUP BY DeliveryDate, RouteId,RouteName, ClerkId, ClerkName, ClerkCode;	
	
-- EXEC sp_A_GainsLossesPerFieldClerk 
