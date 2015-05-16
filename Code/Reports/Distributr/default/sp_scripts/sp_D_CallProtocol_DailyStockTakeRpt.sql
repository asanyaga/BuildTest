DROP PROCEDURE [dbo].[sp_D_CallProtocol_DailyStockTakeRpt]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_D_CallProtocol_DailyStockTakeRpt]
(
@startDate AS DATE,
@endDate AS DATE,
@distributorId AS NVARCHAR(50),
@salesmanId AS NVARCHAR(50),
@routeId AS NVARCHAR(50),
@outletId AS NVARCHAR(50)


)
AS 
if  RTRIM(LTRIM(@distributorId))='ALL'  begin set @distributorId='%' end
if  RTRIM(LTRIM(@salesmanId))='ALL'  begin set @salesmanId='%' end
if  RTRIM(LTRIM(@routeId))='ALL'  begin set @routeId='%' end
if  RTRIM(LTRIM(@outletId))='ALL'  begin set @outletId='%' end

;WITH CallProtocol_CTE(DistributorId,DistributorName,
                       SalesmanId,SalesmanName,
                       RouteId,RouteName,
                       OutletId,OutletName,
                       ChannelId,ChannelName,
                       sBrandId,sBrandName,
                       sProductId,sProductName,
                       stBrandId,stBrandName,
                       stProductId,stProductName,
                       Available,Sold,
                       VisitDate)
AS
(
SELECT dbo.v_D_CallProtocol_Sales.DistributorId,dbo.v_D_CallProtocol_Sales.DistributorName,
       dbo.v_D_CallProtocol_Sales.SalesmanId,dbo.v_D_CallProtocol_Sales.SalesmanName,
       dbo.tblRoutes.RouteID AS RouteId,dbo.tblRoutes.Name AS RouteName,
       dbo.tblCostCentre.Id AS OutletId,dbo.tblCostCentre.Name AS OutletName, 
       cp.OutletTypeId AS ChannelId,cp.OutletTypeName AS ChannelName,
       --dbo.tblDocument.DocumentTypeId, 
       --dbo.tblDocument.DocumentReference, 
       --dbo.tblDocument.Id, 
       dbo.v_D_CallProtocol_Sales.sBrandId,
       dbo.v_D_CallProtocol_Sales.sBrandName, 
       dbo.v_D_CallProtocol_Sales.sProductId,     
       dbo.v_D_CallProtocol_Sales.sProductName,
        
       dbo.v_D_CallProtocol_StockTake.stProductBrandId,
       dbo.v_D_CallProtocol_StockTake.stProductBrandName,
       dbo.v_D_CallProtocol_StockTake.stProductId, 
       dbo.v_D_CallProtocol_StockTake.stProduct,
       
       dbo.v_D_CallProtocol_StockTake.stQty,
       dbo.v_D_CallProtocol_Sales.sQty,  
       --dbo.v_D_CallProtocol_StockTake.stDocRef, 
       --dbo.v_D_CallProtocol_Sales.SaleRef, 
       dbo.tblDocument.DocumentDateIssued AS VisitDate     
FROM  dbo.tblDocument  
JOIN  dbo.v_D_CallProtocol_Sales ON dbo.tblDocument.Id = dbo.v_D_CallProtocol_Sales.sVisitId 
JOIN  dbo.v_D_CallProtocol_StockTake ON dbo.tblDocument.Id = dbo.v_D_CallProtocol_StockTake.stVisitId 
JOIN  dbo.tblCostCentre ON dbo.tblDocument.OrderIssuedOnBehalfOfCC = dbo.tblCostCentre.Id 
JOIN  dbo.tblRoutes ON dbo.tblCostCentre.RouteId = dbo.tblRoutes.RouteID
JOIN  dbo.v_D_ChannelPacks cp ON dbo.tblCostCentre.Outlet_Type_Id = cp.OutletTypeId
WHERE    (dbo.tblDocument.DocumentTypeId = 20) AND (dbo.tblCostCentre.CostCentreType = 5)
    -- AND CONVERT(NVARCHAR(50),dbo.tblCostCentre.Id) LIKE ISNULL(@outletId,'%')
    -- AND CONVERT(NVARCHAR(50),dbo.tblRoutes.RouteID) LIKE ISNULL(@routeId,'%')
     AND CONVERT(NVARCHAR(26),dbo.tblDocument.DocumentDateIssued,23) BETWEEN @startDate AND @endDate

    -- AND (dbo.v_D_CallProtocol_StockTake.stProductId in (SELECT ProductId FROM  v_D_ChannelPacks ) )

),
 cp_stockTake_cte as (
--CASE 1 : There's a STOCK TAKE of Product X and a SALE of Product X
SELECT DistributorId,DistributorName,
       SalesmanId,SalesmanName,
       RouteId,RouteName,
       OutletId,OutletName,
	   ChannelName,
	   sBrandName Brand,
	   sProductName SKU,
	   Available,
	   Sold,
	   VisitDate 
FROM CallProtocol_CTE
WHERE sProductId = stProductId

UNION ALL
--CASE 2: There's a SALE of Product X but There's NO Stock Taken of Product X
SELECT DistributorId,DistributorName,
       SalesmanId,SalesmanName,
	   RouteId,RouteName,
       OutletId,OutletName,
	   ChannelName,
	   stBrandName Brand,
	   stProductName SKU,
	   Available,
	   0 Sold,
	   VisitDate 
FROM CallProtocol_CTE
WHERE sProductId <> stProductId


UNION ALL
--CASE 3: There's a STOCK TAKE of Product X but There's NO SALE of Product X
SELECT DISTINCT DistributorId,DistributorName,
                SalesmanId,SalesmanName,
	            RouteId,RouteName,
                OutletId,OutletName,
				ChannelName,
				sBrandName Brand,
				sProductName SKU,
				NULL Available,
				Sold,
				VisitDate 
FROM CallProtocol_CTE
WHERE sProductId <> stProductId
)

select distinct DistributorName,
                SalesmanName,
				RouteName,
				OutletName,
				ChannelName,
				Brand,
				SKU,
				SUM(Available) Stock,
				SUM(Sold) Sales,
				VisitDate
from cp_stockTake_cte
where convert(nvarchar(50),DistributorId) like isnull(@distributorId,'%')
  and convert(nvarchar(50),SalesmanId) like isnull(@salesmanId,'%')
  and convert(nvarchar(50),RouteId) like isnull(@routeId,'%')
  and convert(nvarchar(50),OutletId) like isnull(@outletId,'%')
  and convert(nvarchar(26),VisitDate,23) between @startDate and @endDate

group by DistributorName,
                SalesmanName,
				RouteName,
				OutletName,
				ChannelName,
				Brand,
				SKU,VisitDate


-- EXEC [dbo].[sp_D_CallProtocol_DailyStockTakeRpt] @startDate ='2013-01-01', @endDate ='2014-10-10',@distributorId ='ALL',@salesmanId ='ALL',@routeId='ALL',@outletId ='ALL'

--  SELECT * FROM [dbo].[v_D_CallProtocol_StockTake] 

GO