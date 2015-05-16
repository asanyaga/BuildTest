
DROP PROCEDURE [dbo].[sp_D_Stock_StockByProductType]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[sp_D_Stock_StockByProductType]

(
@brandId AS NVARCHAR(50),
@productId AS NVARCHAR(50),
@packagingId AS NVARCHAR(50),
@distributorId AS NVARCHAR(50),
@salesmanId AS NVARCHAR(50),
@productTypeId as nvarchar(50),
@subBrandId as nvarchar(50),
@packagingTypeId AS NVARCHAR(50)

)
AS 
if  ltrim(rtrim(@distributorId))='ALL'  begin set @distributorId='%' end
if  ltrim(rtrim(@salesmanId))='ALL'  begin set @salesmanId='%' end
if  ltrim(rtrim(@brandId))='ALL'  begin set @brandId='%' end
if  ltrim(rtrim(@productId))='ALL'  begin set @productId='%' end
if  ltrim(rtrim(@packagingId))='ALL'  begin set @packagingId='%' end
if  ltrim(rtrim(@subBrandId))='ALL'  begin set @subBrandId='%' end
if  ltrim(rtrim(@productTypeId))='ALL'  begin set @productTypeId='%' end
if  ltrim(rtrim(@packagingTypeId))='ALL'  begin set @packagingTypeId='%' end

SELECT     CASE WHEN cc.CostCentreType = 2 THEN cc.Id ELSE CAST(CAST(0 AS BINARY) AS UNIQUEIDENTIFIER) END AS DistributorId, 
           CASE WHEN cc.CostCentreType = 2 THEN cc.Name ELSE '' END AS DistributorName, 
           CASE WHEN cc.CostCentreType = 4 THEN cc.Id ELSE CAST(CAST(0 AS BINARY) AS UNIQUEIDENTIFIER) END AS SalesmanId, 
           CASE WHEN cc.CostCentreType = 4 THEN cc.Name ELSE '' END AS SalesmanName, 
           inv.Value AS InvValue, 
           inv.Balance AS InvBalance, 
           prod.id AS ProductId, 
           prod.Description AS ProductName,
           prodType.id AS ProductTypeId, 
           prodType.name AS ProductTypeName, 
           prodBrand.id AS BrandId, 
           prodBrand.name AS BrandName, 
           prodFlav.id AS SubBrandId, 
           prodFlav.name AS SubBrandName, 
           prodPack.Id AS PackageId, 
           prodPack.Name AS PackageName,
           prodPackType.id AS PackagingTypeId, 
           prodPackType.name AS PackagingTypeName,
           prod.ExFactoryPrice
FROM       dbo.tblInventory inv
INNER JOIN dbo.tblCostCentre cc ON inv.WareHouseId = cc.Id 
INNER JOIN dbo.tblProduct prod ON inv.ProductId = prod.id 
LEFT OUTER JOIN dbo.tblProductType prodType ON prod.ProductTypeId = prodType.id 
INNER JOIN dbo.tblProductBrand prodBrand ON prod.BrandId = prodBrand.id 
INNER JOIN dbo.tblProductPackaging prodPack ON prod.PackagingId = prodPack.Id 
INNER JOIN dbo.tblProductPackagingType prodPackType ON prod.PackagingTypeId = prodPackType.id 
LEFT OUTER JOIN dbo.tblProductFlavour prodFlav ON prod.FlavourId = prodFlav.id
WHERE  (inv.Balance > 0)  
       AND(CONVERT(NVARCHAR(50), prodBrand.id) LIKE ISNULL(@brandId,N'%'))
       AND(CONVERT(NVARCHAR(50),cc.Id) LIKE ISNULL(@distributorId, N'%')) 
       AND(CONVERT(NVARCHAR(50),cc.Id) LIKE ISNULL(@salesmanId, N'%')) 
       AND(CONVERT(NVARCHAR(50),prod.id) LIKE ISNULL(@productId, N'%')) 
       AND(CONVERT(NVARCHAR(50),prodType.id) LIKE ISNULL(@productTypeId, N'%')) 
       AND(CONVERT(NVARCHAR(50),prodFlav.id ) LIKE ISNULL(@subBrandId, N'%'))
       AND(CONVERT(NVARCHAR(50),prodPack.Id ) LIKE ISNULL(@packagingId, N'%')) 
       AND(CONVERT(NVARCHAR(50),prodPackType.id ) LIKE ISNULL(@packagingTypeId, N'%'))  
       AND(cc.CostCentreType = 2 OR cc.CostCentreType = 4) 
       AND prod.IM_Status = 1 --Only Active Product           

           
--   Exec [dbo].[sp_D_Stock_StockByProductType] @brandId='ALL',@distributorId='ALL',@salesmanId='ALL',@productId='ALL',@packagingId='ALL',@subBrandId='ALL',@productTypeId='ALL',@packagingTypeId='ALL'
           
           
           
           
 GO
           
