--
DROP PROCEDURE [dbo].[sp_D_Stock_StockByProductTypePerBrand]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[sp_D_Stock_StockByProductTypePerBrand]
(
@brandId AS NVARCHAR(50),
@productId AS NVARCHAR(50),
@packagingId AS NVARCHAR(50),
@distributorId AS NVARCHAR(50),
@salesmanId AS NVARCHAR(50),
@Brand AS NVARCHAR(50),
@subBrandId as nvarchar(50),
@Product AS NVARCHAR(50),
@productTypeId as nvarchar(50)
)
AS 
if  ltrim(rtrim(@distributorId))='ALL'  begin set @distributorId='%' end
if  ltrim(rtrim(@salesmanId))='ALL'  begin set @salesmanId='%' end
if  ltrim(rtrim(@brandId))='ALL'  begin set @brandId='%' end
if  ltrim(rtrim(@productId))='ALL'  begin set @productId='%' end
if  ltrim(rtrim(@packagingId))='ALL'  begin set @packagingId='%' end
if  ltrim(rtrim(@Brand))='ALL'  begin set @Brand='%' end
if  ltrim(rtrim(@subBrandId))='ALL'  begin set @subBrandId='%' end
if  ltrim(rtrim(@Product))='ALL'  begin set @Product='%' end
if  ltrim(rtrim(@productTypeId))='ALL'  begin set @productTypeId='%' end

SELECT     CONVERT(NVARCHAR(50), dbo.tblCostCentre.Id) AS DistributorId, dbo.tblCostCentre.Name AS DistributorName, dbo.tblProduct.id AS ProductId, 
                      dbo.tblProduct.Description AS ProductName, dbo.tblProductBrand.id AS BrandId, dbo.tblProductBrand.name AS BrandName, dbo.tblInventory.Balance AS InvBalance, 
                      dbo.tblInventory.Value AS InvValue, dbo.tblInventory.IM_DateCreated, dbo.tblInventory.UnavailableBalance, dbo.tblProduct.ExFactoryPrice, 
                      dbo.tblProductType.id AS productTypeId, dbo.tblProductType.name AS productTypeName, dbo.tblProductFlavour.name AS SubBrandName, 
                      dbo.tblProductPackaging.Id AS PackageId, dbo.tblProductPackaging.Name AS PackageName
FROM         dbo.tblProductBrand INNER JOIN
                      dbo.tblProduct INNER JOIN
                      dbo.tblCostCentre INNER JOIN
                      dbo.tblInventory ON dbo.tblCostCentre.Id = dbo.tblInventory.WareHouseId ON dbo.tblProduct.id = dbo.tblInventory.ProductId ON 
                      dbo.tblProductBrand.id = dbo.tblProduct.BrandId INNER JOIN
                      dbo.tblProductType ON dbo.tblProduct.ProductTypeId = dbo.tblProductType.id INNER JOIN
                      dbo.tblProductFlavour ON dbo.tblProduct.FlavourId = dbo.tblProductFlavour.id INNER JOIN
                      dbo.tblProductPackaging ON dbo.tblProduct.PackagingId = dbo.tblProductPackaging.Id
WHERE     (dbo.tblInventory.Balance >= 0) AND (dbo.tblCostCentre.CostCentreType = 2)
           AND dbo.tblProduct.IM_Status = 1 --Only Active Product      
           AND (dbo.tblProductBrand.name LIKE ISNULL(@Brand, N'%')) 
           AND(CONVERT(NVARCHAR(50), tblProductBrand.id) LIKE ISNULL(@brandId,N'%'))
           AND(CONVERT(NVARCHAR(50),tblCostCentre.Id) LIKE ISNULL(@distributorId, N'%')) 
           AND(CONVERT(NVARCHAR(50),dbo.tblCostCentre.Id) LIKE ISNULL(@salesmanId, N'%')) 
           AND(CONVERT(NVARCHAR(50),dbo.tblProduct.id) LIKE ISNULL(@productId, N'%')) 
           AND(CONVERT(NVARCHAR(50),dbo.tblProductFlavour.id ) LIKE ISNULL(@subBrandId, N'%'))
           AND(CONVERT(NVARCHAR(50),dbo.tblProductType.id) LIKE ISNULL(@productTypeId, N'%')) 
           AND(dbo.tblProduct.Description LIKE ISNULL(@Product, N'%'))


union all

SELECT     CONVERT(NVARCHAR(50), tblCostCentre_1.Id) AS DistributorId, tblCostCentre_1.Name AS DistributorName, dbo.tblProduct.id AS ProductId, 
                      dbo.tblProduct.Description AS ProductName, dbo.tblProductBrand.id AS BrandId, dbo.tblProductBrand.name AS BrandName, dbo.tblInventory.Balance AS InvBalance, 
                      dbo.tblInventory.Value AS InvValue, dbo.tblInventory.IM_DateCreated, dbo.tblInventory.UnavailableBalance, dbo.tblProduct.ExFactoryPrice, 
                      dbo.tblProductType.id AS productTypeId, dbo.tblProductType.name AS productTypeName, dbo.tblProductFlavour.name AS SubBrandName,
                       dbo.tblProductPackaging.Id AS PackageId, dbo.tblProductPackaging.Name AS PackageName
FROM         dbo.tblProductBrand INNER JOIN
                      dbo.tblProduct INNER JOIN
                      dbo.tblCostCentre INNER JOIN
                      dbo.tblInventory ON dbo.tblCostCentre.Id = dbo.tblInventory.WareHouseId ON dbo.tblProduct.id = dbo.tblInventory.ProductId ON 
                      dbo.tblProductBrand.id = dbo.tblProduct.BrandId INNER JOIN
                      dbo.tblCostCentre AS tblCostCentre_1 ON dbo.tblCostCentre.ParentCostCentreId = tblCostCentre_1.Id INNER JOIN
                      dbo.tblProductType ON dbo.tblProduct.ProductTypeId = dbo.tblProductType.id INNER JOIN
                      dbo.tblProductFlavour ON dbo.tblProduct.FlavourId = dbo.tblProductFlavour.id INNER JOIN
                      dbo.tblProductPackaging ON dbo.tblProduct.PackagingId = dbo.tblProductPackaging.Id
           
WHERE     (dbo.tblInventory.Balance >= 0) AND (dbo.tblCostCentre.CostCentreType = 4)
           AND dbo.tblProduct.IM_Status = 1 --Only Active Product
           AND (dbo.tblProductBrand.name LIKE ISNULL(@Brand, N'%')) 
           AND(CONVERT(NVARCHAR(50), tblProductBrand.id) LIKE ISNULL(@brandId,N'%'))
           AND(CONVERT(NVARCHAR(50),tblCostCentre_1.Id) LIKE ISNULL(@distributorId, N'%')) 
           AND(CONVERT(NVARCHAR(50),dbo.tblCostCentre.Id) LIKE ISNULL(@salesmanId, N'%')) 
           AND(CONVERT(NVARCHAR(50),dbo.tblProduct.id) LIKE ISNULL(@productId, N'%')) 
           AND(CONVERT(NVARCHAR(50),dbo.tblProductFlavour.id ) LIKE ISNULL(@subBrandId, N'%'))
           AND(CONVERT(NVARCHAR(50),dbo.tblProductType.id) LIKE ISNULL(@productTypeId, N'%')) 
           AND(dbo.tblProduct.Description LIKE ISNULL(@Product, N'%'))