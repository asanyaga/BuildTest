/****** Object:  StoredProcedure [dbo].[map_spGetPoints_DevZeroSales]    Script Date: 11/12/2014 10:39:45 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[map_spDev_ZS_GetPoints]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[map_spDev_ZS_GetPoints]
GO


CREATE procedure [dbo].[map_spDev_ZS_GetPoints](
	@sDate as datetime,
	@eDate as datetime,
	@uDistributor varchar(200),
	@uSalesman varchar(200),
	@uRoute varchar(200),
	@uOutlet varchar(200)
)
AS 
SELECT  distinct   dist.Id AS DistributorId, 
           dist.Name AS DistributorName,
		   sm.id SalesmanId,
		   sm.Name SalesmanName,
		   routes.RouteID AS RouteId, 
           routes.Name AS RouteName, 
           outlets.Id AS OutletId, 
           outlets.Name AS OutletName, 
           outlets.CostCentreType AS OutletCctype,
		   outlets.StandardWH_Latitude Latitude,
		   outlets.StandardWH_Longtitude Longitude 
		   --(select top 1 Longitude from tblDocument s where outlets.Id = s.OrderIssuedOnBehalfOfCC and ISNULL(Longitude,'') <> '') as Longitude,
		   --(select top 1 Latitude from tblDocument s where outlets.Id = s.OrderIssuedOnBehalfOfCC and ISNULL(Latitude,'') <> '') as Latitude
FROM    dbo.tblCostCentre outlets
 JOIN   dbo.tblRoutes routes ON outlets.RouteId = routes.RouteID 
 JOIN   dbo.tblSalemanRoute sr on routes.RouteID = sr.RouteId
 JOIN   dbo.tblCostCentre sm on sr.SalemanId = sm.Id
 JOIN   dbo.tblCostCentre AS dist ON outlets.ParentCostCentreId = dist.Id 
 JOIN   dbo.tblDocument s ON outlets.Id = s.OrderIssuedOnBehalfOfCC
WHERE (outlets.CostCentreType = 5)
    AND dist.CostCentreType = 2
    AND sm.CostCentreType = 4
	AND outlets.StandardWH_Latitude <> '' AND outlets.StandardWH_Latitude <> 0 AND outlets.StandardWH_Latitude IS NOT NULL
	AND outlets.StandardWH_Latitude <> '' AND outlets.StandardWH_Latitude <> 0 AND outlets.StandardWH_Latitude IS NOT NULL
	AND dist.Id = (case when @uDistributor = 'ALL' then dist.Id else @uDistributor end )
	AND sm.Id = (case when @uSalesman = 'ALL' then sm.Id else @uSalesman end )
	AND routes.RouteID = (case when @uRoute = 'ALL' then routes.RouteID else @uRoute end )
	AND outlets.Id = (case when @uOutlet = 'ALL' then outlets.Id else @uOutlet end )
	AND(outlets.Id NOT IN(
		SELECT dbo.tblDocument.OrderIssuedOnBehalfOfCC
		FROM dbo.tblDocument INNER JOIN
		dbo.tblLineItems as line ON dbo.tblDocument.Id = line.DocumentID
		WHERE (dbo.tblDocument.DocumentTypeId = 1)
		AND  convert(nvarchar(26),dbo.tblDocument.DocumentDateIssued,23) between @sDate and @eDate
		AND ((dbo.tblDocument.OrderOrderTypeId = 1)OR(dbo.tblDocument.OrderOrderTypeId = 3 AND dbo.tblDocument.DocumentStatusId = 99))))   
                        
GO


-- ===================================================================================================================================
-- ===================================================================================================================================


IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[map_spDev_ZS_GetDistributors]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[map_spDev_ZS_GetDistributors]
GO

CREATE procedure [dbo].[map_spDev_ZS_GetDistributors](
	@sDate as datetime,
	@eDate as datetime
)
as 

SELECT  distinct 
		tblCostCentre_1.Id AS DistributorId, 
        tblCostCentre_1.Name AS DistributorName
FROM
	dbo.tblCostCentre INNER JOIN
	dbo.tblRoutes ON dbo.tblCostCentre.RouteId = dbo.tblRoutes.RouteID INNER JOIN
	dbo.tblCostCentre AS tblCostCentre_1 ON dbo.tblCostCentre.ParentCostCentreId = tblCostCentre_1.Id INNER JOIN
	dbo.tblDocument s ON dbo.tblCostCentre.Id = s.OrderIssuedOnBehalfOfCC
WHERE     (dbo.tblCostCentre.CostCentreType = 5)
	AND s.Longitude is not null 
	AND s.Latitude is not null 
	AND (s.Longitude  <> 0 or s.Latitude <> 0)
AND(dbo.tblCostCentre.Id NOT IN(
	SELECT dbo.tblDocument.OrderIssuedOnBehalfOfCC
	FROM dbo.tblDocument INNER JOIN
	dbo.tblLineItems as line ON dbo.tblDocument.Id = line.DocumentID
	WHERE (dbo.tblDocument.DocumentTypeId = 1)
	AND ((dbo.tblDocument.OrderOrderTypeId = 1)OR(dbo.tblDocument.OrderOrderTypeId = 3 AND dbo.tblDocument.DocumentStatusId = 99)))
)   
                        
GO

-- ===================================================================================================================================
-- ===================================================================================================================================


IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[map_spDev_ZS_GetSalesMen]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[map_spDev_ZS_GetSalesMen]
GO

CREATE procedure [dbo].[map_spDev_ZS_GetSalesMen](
	@sDate as datetime,
	@eDate as datetime,
	@uDistributor varchar(200)
)
as 

SELECT  distinct
		   sm.id SalesmanId,
		   sm.Name SalesmanName
FROM    dbo.tblCostCentre outlets
 JOIN   dbo.tblRoutes routes ON outlets.RouteId = routes.RouteID 
 JOIN   dbo.tblSalemanRoute sr on routes.RouteID = sr.RouteId
 JOIN   dbo.tblCostCentre sm on sr.SalemanId = sm.Id
 JOIN   dbo.tblCostCentre AS dist ON outlets.ParentCostCentreId = dist.Id 
 JOIN   dbo.tblDocument s ON outlets.Id = s.OrderIssuedOnBehalfOfCC
WHERE (outlets.CostCentreType = 5)
	AND dist.Id = (case when @uDistributor = 'ALL' then dist.Id else @uDistributor end )
	AND s.Longitude is not null 
	AND s.Latitude is not null 
	AND (s.Longitude  <> 0 or s.Latitude <> 0)
	AND(outlets.Id NOT IN(
		SELECT dbo.tblDocument.OrderIssuedOnBehalfOfCC
		FROM dbo.tblDocument INNER JOIN
		dbo.tblLineItems as line ON dbo.tblDocument.Id = line.DocumentID
		WHERE (dbo.tblDocument.DocumentTypeId = 1)
		AND  convert(nvarchar(26),dbo.tblDocument.DocumentDateIssued,23) between @sDate and @eDate
		AND ((dbo.tblDocument.OrderOrderTypeId = 1)OR(dbo.tblDocument.OrderOrderTypeId = 3 AND dbo.tblDocument.DocumentStatusId = 99))))  
                        
GO

-- ===================================================================================================================================
-- ===================================================================================================================================


IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[map_spDev_ZS_GetRoutes]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[map_spDev_ZS_GetRoutes]
GO

CREATE procedure [dbo].[map_spDev_ZS_GetRoutes](
	@sDate as datetime,
	@eDate as datetime,
	@uDistributor varchar(200),
	@uSalesman varchar(200)
)
as 
SELECT  DISTINCT
		   routes.RouteID AS RouteId, 
           routes.Name AS RouteName
FROM    dbo.tblCostCentre outlets
 JOIN   dbo.tblRoutes routes ON outlets.RouteId = routes.RouteID 
 JOIN   dbo.tblSalemanRoute sr on routes.RouteID = sr.RouteId
 JOIN   dbo.tblCostCentre sm on sr.SalemanId = sm.Id
 JOIN   dbo.tblCostCentre AS dist ON outlets.ParentCostCentreId = dist.Id 
 JOIN   dbo.tblDocument s ON outlets.Id = s.OrderIssuedOnBehalfOfCC
WHERE (outlets.CostCentreType = 5)
	AND dist.Id = (case when @uDistributor = 'ALL' then dist.Id else @uDistributor end )
	AND sm.Id = (case when @uSalesman = 'ALL' then sm.Id else @uSalesman end )
	AND s.Longitude is not null 
	AND s.Latitude is not null 
	AND (s.Longitude  <> 0 or s.Latitude <> 0)
	AND(outlets.Id NOT IN(
		SELECT dbo.tblDocument.OrderIssuedOnBehalfOfCC
		FROM dbo.tblDocument INNER JOIN
		dbo.tblLineItems as line ON dbo.tblDocument.Id = line.DocumentID
		WHERE (dbo.tblDocument.DocumentTypeId = 1)
		AND  convert(nvarchar(26),dbo.tblDocument.DocumentDateIssued,23) between @sDate and @eDate
		AND ((dbo.tblDocument.OrderOrderTypeId = 1)OR(dbo.tblDocument.OrderOrderTypeId = 3 AND dbo.tblDocument.DocumentStatusId = 99))))                    
GO

-- ===================================================================================================================================
-- ===================================================================================================================================


IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[map_spDev_ZS_GetOutlets]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[map_spDev_ZS_GetOutlets]
GO

CREATE procedure [dbo].[map_spDev_ZS_GetOutlets](
	@sDate as datetime,
	@eDate as datetime,
	@uDistributor varchar(200),
	@uSalesman varchar(200),
	@uRoute varchar(200)
)
as 

SELECT  distinct 
		dbo.tblCostCentre.Id AS OutletId, 
        dbo.tblCostCentre.Name AS OutletName
FROM
	dbo.tblCostCentre INNER JOIN
	dbo.tblRoutes ON dbo.tblCostCentre.RouteId = dbo.tblRoutes.RouteID INNER JOIN
	dbo.tblCostCentre AS tblCostCentre_1 ON dbo.tblCostCentre.ParentCostCentreId = tblCostCentre_1.Id INNER JOIN
	dbo.tblDocument s ON dbo.tblCostCentre.Id = s.OrderIssuedOnBehalfOfCC
WHERE     (dbo.tblCostCentre.CostCentreType = 5)
AND tblCostCentre_1.Id = (case when @uDistributor = 'ALL' then tblCostCentre_1.Id else @uDistributor end )
AND dbo.tblRoutes.RouteID = (case when @uRoute = 'ALL' then dbo.tblRoutes.RouteID else @uRoute end )
	AND s.Longitude is not null 
	AND s.Latitude is not null 
	AND (s.Longitude  <> 0 or s.Latitude <> 0)
AND(dbo.tblCostCentre.Id NOT IN(
	SELECT dbo.tblDocument.OrderIssuedOnBehalfOfCC
	FROM dbo.tblDocument INNER JOIN
	dbo.tblLineItems as line ON dbo.tblDocument.Id = line.DocumentID
	WHERE (dbo.tblDocument.DocumentTypeId = 1)
	AND ((dbo.tblDocument.OrderOrderTypeId = 1)OR(dbo.tblDocument.OrderOrderTypeId = 3 AND dbo.tblDocument.DocumentStatusId = 99)))
)   
                        
GO