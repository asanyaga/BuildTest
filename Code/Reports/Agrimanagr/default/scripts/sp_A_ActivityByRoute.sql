IF NOT EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'sp_A_ActivityByRoute')
   exec('CREATE PROCEDURE [sp_A_ActivityByRoute] AS BEGIN SET NOCOUNT ON; END')
GO
Alter PROCEDURE sp_A_ActivityByRoute
(
@StartDate AS DATE,
@EndDate AS DATE,
@HubId AS NVARCHAR(50),
@RouteId AS NVARCHAR(50),
@CentreId AS NVARCHAR(50),
@FarmerId AS NVARCHAR(50),
@FarmId AS NVARCHAR(50),
@ActivityId AS NVARCHAR(50),
@ClerkId AS NVARCHAR(50)
)

AS

if  @HubId='ALL'  begin set @HubId='%' end
if  @RouteId='ALL'  begin set @RouteId='%' end
if  @CentreId='ALL'  begin set @CentreId='%' end
if  @FarmerId='ALL'  begin set @FarmerId='%' end
if  @FarmId='ALL'  begin set @FarmId='%' end
if  @ActivityId='ALL'  begin set @ActivityId='%' end
if  @ClerkId='ALL'  begin set @ClerkId='%' end

;WITH Activity_CTE AS(
SELECT	DISTINCT dbo.tblActivityDocument.Id AS ActivityId,
		dbo.tblActivityDocument.ActivityReference,
		dbo.tblActivityDocument.ActivityDate,
		dbo.tblActivityType.Name AS ActivityName,
		1 AS NoOfActivities, 
		dbo.tblSupplier.Name AS SupplierName,
		dbo.tblProductType.name AS ProductName,
		dbo.tblProductBrand.name AS ProductBrand,
		dbo.tblProductFlavour.name AS ProductSubBrand,
		tblProductType_1.name AS ProductType, 
		dbo.tblProductPackaging.Name AS ProductPackaging,
		dbo.tblProductPackagingType.name AS PackagingType,
		dbo.tblVATClass.Name AS VATClass,
		dbo.tblActivityInputLineItem.Quantity,
		dbo.tblProduct.ExFactoryPrice,
		hub.Name AS Factory,
		dbo.tblRoutes.Name AS RouteName,
		dbo.tblCostCentre.Name AS ServiceSupplier,
		(dbo.tblCommodityOwner.FirstName + ' ' + dbo.tblCommodityOwner.Surname) as FarmerName,
		dbo.tblCommodityProducer.Name AS Farm

FROM	dbo.tblActivityDocument 
		INNER JOIN dbo.tblActivityType ON dbo.tblActivityDocument.ActivityTypeId = dbo.tblActivityType.Id 
		INNER JOIN dbo.tblActivityInputLineItem ON dbo.tblActivityDocument.Id = dbo.tblActivityInputLineItem.ActivityId
		INNER JOIN dbo.tblProduct ON dbo.tblActivityInputLineItem.ProductId = dbo.tblProduct.id
		INNER JOIN dbo.tblProductType ON dbo.tblProduct.ProductTypeId = dbo.tblProductType.id
		INNER JOIN dbo.tblProductBrand ON dbo.tblProduct.BrandId = dbo.tblProductBrand.id
		INNER JOIN dbo.tblProductFlavour ON dbo.tblProduct.FlavourId = dbo.tblProductFlavour.id 
		INNER JOIN dbo.tblSupplier ON dbo.tblProductBrand.SupplierId = dbo.tblSupplier.id
		INNER JOIN dbo.tblProductType AS tblProductType_1 ON dbo.tblProduct.ProductTypeId = tblProductType_1.id
		INNER JOIN dbo.tblProductPackaging ON dbo.tblProduct.PackagingId = dbo.tblProductPackaging.Id
		INNER JOIN dbo.tblProductPackagingType ON dbo.tblProduct.PackagingTypeId = dbo.tblProductPackagingType.id
		INNER JOIN dbo.tblVATClass ON dbo.tblProduct.VatClassId = dbo.tblVATClass.id
		INNER JOIN dbo.tblCostCentre ON dbo.tblCostCentre.Id = dbo.tblActivityDocument.CommoditySupplierId
		INNER JOIN dbo.tblCostCentre AS hub ON dbo.tblCostCentre.ParentCostCentreId = hub.Id
		INNER JOIN dbo.tblRoutes ON dbo.tblActivityDocument.RouteId = dbo.tblRoutes.RouteID
		INNER JOIN dbo.tblCommodityOwner ON dbo.tblCostCentre.Id = dbo.tblCommodityOwner.CostCentreId
		INNER JOIN dbo.tblCommodityProducer ON dbo.tblActivityDocument.CommodityProducerId = dbo.tblCommodityProducer.Id
		INNER JOIN dbo.tblUsers ON dbo.tblActivityDocument.ClerkId = dbo.tblUsers.CostCenterId

WHERE	(CONVERT(VARCHAR(26),tblActivityDocument.ActivityDate,23)  BETWEEN @startDate AND @endDate)   
        AND(CONVERT(NVARCHAR(50),hub.Id) LIKE ISNULL(@HubId, N'%'))             
        AND(CONVERT(NVARCHAR(50),dbo.tblActivityDocument.RouteID) LIKE ISNULL(@RouteId, N'%'))  
        AND(CONVERT(NVARCHAR(50),dbo.tblActivityDocument.CentreId) LIKE ISNULL(@CentreId, N'%'))
        AND(CONVERT(NVARCHAR(50),dbo.tblCommodityOwner.Id) LIKE ISNULL(@FarmerId, N'%'))
		AND(CONVERT(NVARCHAR(50),dbo.tblCommodityProducer.Id) LIKE ISNULL(@FarmId, N'%'))
		AND(CONVERT(NVARCHAR(50),dbo.tblActivityType.Id) LIKE ISNULL(@ActivityId, N'%'))
		AND(CONVERT(NVARCHAR(50),dbo.tblUsers.Id) LIKE ISNULL(@ClerkId, N'%'))

GROUP BY dbo.tblActivityDocument.Id,
		dbo.tblActivityDocument.ActivityReference,
		dbo.tblActivityDocument.ActivityDate,
		dbo.tblActivityType.Name, 
		dbo.tblSupplier.Name,
		dbo.tblProductType.name,
		dbo.tblProductBrand.name,
		dbo.tblProductFlavour.name,
		tblProductType_1.name, 
		dbo.tblProductPackaging.Name,
		dbo.tblProductPackagingType.name,
		dbo.tblVATClass.Name,
		dbo.tblActivityInputLineItem.Quantity,
		dbo.tblProduct.ExFactoryPrice,
		hub.Name,
		dbo.tblRoutes.Name,
		dbo.tblCostCentre.Name,
		dbo.tblCommodityOwner.FirstName,dbo.tblCommodityOwner.Surname,
		dbo.tblCommodityProducer.Name,
		dbo.tblUsers.UserName
)

SELECT RouteName,ActivityName,SUM(NoOfActivities) AS NoOfActivities
FROM Activity_CTE

GROUP BY RouteName,ActivityName

-- EXEC sp_A_ActivityByRoute @StartDate='2014-01-01',@EndDate='2015-07-15',@HubId='ALL',@RouteId='ALL',@CentreId='ALL',@FarmerId='ALL',@FarmId='ALL',@ActivityId='ALL',@ClerkId='ALL'
					 