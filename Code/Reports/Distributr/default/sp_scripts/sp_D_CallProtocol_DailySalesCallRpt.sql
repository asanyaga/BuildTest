DROP PROCEDURE [dbo].[sp_D_CallProtocol_DailySalesCallRpt]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_D_CallProtocol_DailySalesCallRpt]
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

;WITH CallProtocol_CTE(RouteId,RouteName,
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
SELECT dbo.tblRoutes.RouteID AS RouteId,dbo.tblRoutes.Name AS RouteName,
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
       convert(nvarchar(26),dbo.tblDocument.DocumentDateIssued,23) AS VisitDate     
FROM  dbo.tblDocument  
JOIN  dbo.v_D_CallProtocol_Sales ON dbo.tblDocument.Id = dbo.v_D_CallProtocol_Sales.sVisitId 
JOIN  dbo.v_D_CallProtocol_StockTake ON dbo.tblDocument.Id = dbo.v_D_CallProtocol_StockTake.stVisitId 
JOIN  dbo.tblCostCentre ON dbo.tblDocument.OrderIssuedOnBehalfOfCC = dbo.tblCostCentre.Id 
JOIN  dbo.tblRoutes ON dbo.tblCostCentre.RouteId = dbo.tblRoutes.RouteID
JOIN  dbo.v_D_ChannelPacks cp ON dbo.tblCostCentre.Outlet_Type_Id = cp.OutletTypeId
WHERE    (dbo.tblDocument.DocumentTypeId = 20)
     AND (dbo.tblCostCentre.CostCentreType = 5)
     AND CONVERT(NVARCHAR(50),dbo.tblCostCentre.Id) LIKE ISNULL(@outletId,'%')
     AND CONVERT(NVARCHAR(50),dbo.tblRoutes.RouteID) LIKE ISNULL(@routeId,'%')
     AND CONVERT(NVARCHAR(26),dbo.tblDocument.DocumentDateIssued,23) BETWEEN @startDate AND @endDate

    -- AND (dbo.v_D_CallProtocol_StockTake.stProductId in (SELECT ProductId FROM  v_D_ChannelPacks ) )

)
--CASE 1 : There's a STOCK TAKE of Product X and a SALE of Product X
SELECT RouteName,OutletName,ChannelName,sBrandName MandatedBrand,sProductName MandatedSKU,Available,Sold,VisitDate 
FROM CallProtocol_CTE
WHERE sProductId = stProductId

UNION ALL
--CASE 2: There's a SALE of Product X but There's NO Stock Taken of Product X
SELECT RouteName,OutletName,ChannelName,stBrandName MandatedBrand,stProductName MandatedSKU,Available,0 Sold,VisitDate 
FROM CallProtocol_CTE
WHERE sProductId <> stProductId


UNION ALL
--CASE 3: There's a STOCK TAKE of Product X but There's NO SALE of Product X
SELECT DISTINCT RouteName,OutletName,ChannelName,sBrandName MandatedBrand,sProductName MandatedSKU,NULL Available,Sold,VisitDate 
FROM CallProtocol_CTE
WHERE sProductId <> stProductId



-- EXEC [dbo].[sp_D_CallProtocol_DailySalesCallRpt] @startDate ='2014-10-08', @endDate ='2014-10-10',@distributorId ='ALL',@salesmanId ='01469fba-9693-450f-b2dc-38d98a220554',@routeId='ALL',@outletId ='ALL'

--  SELECT * FROM [dbo].[v_D_CallProtocol_StockTake] 

GO