/****** Object:  StoredProcedure [dbo].[map_spGetSalesPoints]    Script Date: 11/12/2014 10:39:45 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[map_spGetSalesPoints]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[map_spGetSalesPoints]
GO

CREATE procedure [dbo].[map_spGetSalesPoints](
	@sDate as datetime,
	@eDate as datetime,
	@uDistributor varchar(200),
	@uSalesman varchar(200),
	@uRoute varchar(200),
	@uOutlet varchar(200)
)
as 

SELECT ResultID , DistributorId ,Distributor ,OutletId ,Outlet ,SaleAmount ,DocumentReference ,DocumentDateIssued ,
Latitude ,Longitude ,RouteID ,Route ,Salesman ,SalesmanID ,SalesmanTypeID ,RouteIDroutesTable ,
ProductDiscount ,Region ,Country ,SaleDiscount ,TotalProductDiscount
FROM vLocationResults
WHERE (Latitude <> '' and Longitude <> '')
AND DistributorId = (case when @uDistributor = 'ALL' then DistributorId else @uDistributor end )
AND SalesmanID = (case when @uSalesman = 'ALL' then SalesmanID else @uSalesman end )
AND RouteID = (case when @uRoute = 'ALL' then RouteID else @uRoute end )
AND OutletId = (case when @uOutlet = 'ALL' then OutletId else @uOutlet end )
AND DocumentDateIssued between @sDate and @eDate
-- order by DocumentDateIssued asc
GO


/****** Object:  StoredProcedure [dbo].[map_spGetSalesMen]    Script Date: 11/12/2014 10:39:45 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[map_spGetSalesMen]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[map_spGetSalesMen]
GO


CREATE procedure [dbo].[map_spGetSalesMen](
	@sDate as datetime,
	@eDate as datetime,
	@uDistributor varchar(200)
)
as 


SELECT distinct [Salesman] , [SalesmanID] FROM [vLocationResults]
where (Latitude <> '' and Longitude <> '')
AND DistributorId = (case when @uDistributor = 'ALL' then DistributorId else @uDistributor end )
AND DocumentDateIssued between @sDate and @eDate
GO


/****** Object:  StoredProcedure [dbo].[map_spGetRoutes]   Script Date: 11/12/2014 10:39:45 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[map_spGetRoutes]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[map_spGetRoutes]
GO


CREATE procedure [dbo].[map_spGetRoutes](
	@sDate as datetime,
	@eDate as datetime,
	@uDistributor varchar(200),
	@uSalesman varchar(200)
)
as 

SELECT distinct [RouteID] ,[Route] FROM [vLocationResults]
where (Latitude <> '' and Longitude <> '')
AND DistributorId = (case when @uDistributor = 'ALL' then DistributorId else @uDistributor end )
AND SalesmanID = (case when @uSalesman = 'ALL' then SalesmanID else @uSalesman end )
AND DocumentDateIssued between @sDate and @eDate
GO


/****** Object:  StoredProcedure [dbo].[map_spGetPoints_DevZeroSales]    Script Date: 11/12/2014 10:39:45 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[map_spGetPoints_DevZeroSales]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[map_spGetPoints_DevZeroSales]
GO


CREATE procedure [dbo].[map_spGetPoints_DevZeroSales](
	@sDate as datetime,
	@eDate as datetime,
	@uDistributor varchar(200),
	@uSalesman varchar(200),
	@uRoute varchar(200),
	@uOutlet varchar(200)
)
as 

SELECT top 5 ResultID , DistributorId ,Distributor ,OutletId ,Outlet ,SaleAmount ,DocumentReference ,DocumentDateIssued ,
Latitude ,Longitude ,RouteID ,Route ,Salesman ,SalesmanID ,SalesmanTypeID ,RouteIDroutesTable ,
ProductDiscount ,Region ,Country ,SaleDiscount ,TotalProductDiscount , 1 as isDeviation , '' as ReasonNotSold
FROM vLocationResults
WHERE (Latitude <> '' and Longitude <> '')
--AND DistributorId = (case when @uDistributor = 'ALL' then DistributorId else @uDistributor end )
--AND SalesmanID = (case when @uSalesman = 'ALL' then SalesmanID else @uSalesman end )
--AND RouteID = (case when @uRoute = 'ALL' then RouteID else @uRoute end )
--AND OutletId = (case when @uOutlet = 'ALL' then OutletId else @uOutlet end )
--AND DocumentDateIssued between @sDate and @eDate
--order by DocumentDateIssued asc
GO


/****** Object:  StoredProcedure [dbo].[map_spGetPoints_DevTargetValue]    Script Date: 11/12/2014 10:39:45 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[map_spGetPoints_DevTargetValue]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[map_spGetPoints_DevTargetValue]
GO


CREATE procedure [dbo].[map_spGetPoints_DevTargetValue](
	@sDate as datetime,
	@eDate as datetime,
	@uDistributor varchar(200),
	@uSalesman varchar(200),
	@uRoute varchar(200),
	@uOutlet varchar(200)
)
as 

SELECT top 5 ResultID , DistributorId ,Distributor ,OutletId ,Outlet ,SaleAmount ,DocumentReference ,DocumentDateIssued ,
Latitude ,Longitude ,RouteID ,Route ,Salesman ,SalesmanID ,SalesmanTypeID ,RouteIDroutesTable ,
ProductDiscount ,Region ,Country ,SaleDiscount ,TotalProductDiscount , 1 as isDeviation , '' as ReasonNotSold
FROM vLocationResults
WHERE (Latitude <> '' and Longitude <> '')
--AND DistributorId = (case when @uDistributor = 'ALL' then DistributorId else @uDistributor end )
--AND SalesmanID = (case when @uSalesman = 'ALL' then SalesmanID else @uSalesman end )
--AND RouteID = (case when @uRoute = 'ALL' then RouteID else @uRoute end )
--AND OutletId = (case when @uOutlet = 'ALL' then OutletId else @uOutlet end )
--AND DocumentDateIssued between @sDate and @eDate
--order by DocumentDateIssued asc
GO


/****** Object:  StoredProcedure [dbo].[map_spGetPoints_DevReasonNotSold]    Script Date: 11/12/2014 10:39:45 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[map_spGetPoints_DevReasonNotSold]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[map_spGetPoints_DevReasonNotSold]
GO


CREATE procedure [dbo].[map_spGetPoints_DevReasonNotSold](
	@sDate as datetime,
	@eDate as datetime,
	@uDistributor varchar(200),
	@uSalesman varchar(200),
	@uRoute varchar(200),
	@uOutlet varchar(200)
)
as 

SELECT top 5 ResultID , DistributorId ,Distributor ,OutletId ,Outlet ,SaleAmount ,DocumentReference ,DocumentDateIssued ,
Latitude ,Longitude ,RouteID ,Route ,Salesman ,SalesmanID ,SalesmanTypeID ,RouteIDroutesTable ,
ProductDiscount ,Region ,Country ,SaleDiscount ,TotalProductDiscount , 1 as isDeviation , '' as ReasonNotSold
FROM vLocationResults
WHERE (Latitude <> '' and Longitude <> '')
--AND DistributorId = (case when @uDistributor = 'ALL' then DistributorId else @uDistributor end )
--AND SalesmanID = (case when @uSalesman = 'ALL' then SalesmanID else @uSalesman end )
--AND RouteID = (case when @uRoute = 'ALL' then RouteID else @uRoute end )
--AND OutletId = (case when @uOutlet = 'ALL' then OutletId else @uOutlet end )
--AND DocumentDateIssued between @sDate and @eDate
--order by DocumentDateIssued asc
GO


/****** Object:  StoredProcedure [dbo].[map_spGetPoints_DevPlannedRoute]    Script Date: 11/12/2014 10:39:45 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[map_spGetPoints_DevPlannedRoute]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[map_spGetPoints_DevPlannedRoute]
GO


CREATE procedure [dbo].[map_spGetPoints_DevPlannedRoute](
	@sDate as datetime,
	@eDate as datetime,
	@uDistributor varchar(200),
	@uSalesman varchar(200),
	@uRoute varchar(200),
	@uOutlet varchar(200)
)
as 

SELECT top 5 ResultID , DistributorId ,Distributor ,OutletId ,Outlet ,SaleAmount ,DocumentReference ,DocumentDateIssued ,
Latitude ,Longitude ,RouteID ,Route ,Salesman ,SalesmanID ,SalesmanTypeID ,RouteIDroutesTable ,
ProductDiscount ,Region ,Country ,SaleDiscount ,TotalProductDiscount , 1 as isDeviation , '' as ReasonNotSold
FROM vLocationResults
WHERE (Latitude <> '' and Longitude <> '')
--AND DistributorId = (case when @uDistributor = 'ALL' then DistributorId else @uDistributor end )
--AND SalesmanID = (case when @uSalesman = 'ALL' then SalesmanID else @uSalesman end )
--AND RouteID = (case when @uRoute = 'ALL' then RouteID else @uRoute end )
--AND OutletId = (case when @uOutlet = 'ALL' then OutletId else @uOutlet end )
--AND DocumentDateIssued between @sDate and @eDate
--order by DocumentDateIssued asc
GO


/****** Object:  StoredProcedure [dbo].[map_spGetOutletTransactions]    Script Date: 11/12/2014 10:39:45 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[map_spGetOutletTransactions]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[map_spGetOutletTransactions]
GO


CREATE procedure [dbo].[map_spGetOutletTransactions](
	@sDate as datetime,
	@eDate as datetime,
	@sResultID varchar(200)
)
as
 
-- ResultID = Outlet ID 

SELECT ROW_NUMBER() OVER (ORDER BY DocumentDateIssued ASC) as row , ResultID , SaleAmount , DocumentReference , DocumentDateIssued , 
Outlet , Salesman ,
SaleDiscount , ProductDiscount from dbo.vLocationResults
where (Latitude <> '' and Longitude <> '')
AND ResultID = @sResultID
AND DocumentDateIssued between @sDate and @eDate
GO


/****** Object:  StoredProcedure [dbo].[map_spGetOutlets]    Script Date: 11/12/2014 10:39:45 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[map_spGetOutlets]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[map_spGetOutlets]
GO

CREATE procedure [dbo].[map_spGetOutlets](
	@sDate as datetime,
	@eDate as datetime,
	@uDistributor varchar(200),
	@uSalesman varchar(200),
	@uRoute varchar(200)
)
as 

SELECT distinct [OutletId] ,[Outlet] FROM [vLocationResults]
WHERE (Latitude <> '' and Longitude <> '')
AND DistributorId = (case when @uDistributor = 'ALL' then DistributorId else @uDistributor end )
AND SalesmanID = (case when @uSalesman = 'ALL' then SalesmanID else @uSalesman end )
AND RouteID = (case when @uRoute = 'ALL' then RouteID else @uRoute end )
AND DocumentDateIssued between @sDate and @eDate
GO


/****** Object:  StoredProcedure [dbo].[map_spGetOutlets]    Script Date: 11/12/2014 10:39:45 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[map_spGetDistributors]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[map_spGetDistributors]
GO


CREATE procedure [dbo].[map_spGetDistributors](
	@sDate as datetime,
	@eDate as datetime
)
as 


SELECT distinct [DistributorId] ,[Distributor] FROM [vLocationResults]
where (Latitude <> '' and Longitude <> '')
AND DocumentDateIssued between @sDate and @eDate
GO


IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[v_D_OutletVisits]'))
DROP VIEW [dbo].[v_D_OutletVisits]
GO


CREATE VIEW [dbo].[v_D_OutletVisits]
AS
SELECT ov.EffectiveDate, 
	   ov.OutletId ,
	   dbo.tblCostCentre.Name OutletName,
	   dbo.ufd_GetDayOfWeek(ov.VistDay) VisitDay
	   --DATENAME(DW,ov.VistDay) VisitDay
FROM   dbo.tblOutletVisitDay ov
 JOIN  dbo.tblCostCentre ON ov.OutletId = dbo.tblCostCentre.Id


GO

