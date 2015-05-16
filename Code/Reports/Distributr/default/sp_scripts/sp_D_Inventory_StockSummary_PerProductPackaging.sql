/****** Object:  StoredProcedure [dbo].[sp_D_Inventory_StockSummary_PerProductPackaging]    Script Date: 07/24/2013 08:51:32 ******/
DROP PROCEDURE [dbo].[sp_D_Inventory_StockSummary_PerProductPackaging]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_D_Inventory_StockSummary_PerProductPackaging]
(
@brandId AS NVARCHAR(50),
@productId AS NVARCHAR(50),
@packagingId AS NVARCHAR(50),
@distributorId AS NVARCHAR(50),
@salesmanId AS NVARCHAR(50),
@productTypeId as nvarchar(50),
@subBrandId as nvarchar(50)
)
AS 
if  ltrim(rtrim(@distributorId))='ALL'  begin set @distributorId='%' end
if  ltrim(rtrim(@salesmanId))='ALL'  begin set @salesmanId='%' end
if  ltrim(rtrim(@brandId))='ALL'  begin set @brandId='%' end
if  ltrim(rtrim(@productId))='ALL'  begin set @productId='%' end
if  ltrim(rtrim(@packagingId))='ALL'  begin set @packagingId='%' end
if  ltrim(rtrim(@subBrandId))='ALL'  begin set @subBrandId='%' end
if  ltrim(rtrim(@subBrandId))='ALL'  begin set @subBrandId='%' end
if  ltrim(rtrim(@productTypeId))='ALL'  begin set @productTypeId='%' end

SELECT     dbo.tblCostCentre.Id AS DistributorId, 
           dbo.tblCostCentre.Name AS DistributorName,
           dbo.tblProduct.id AS ProductId, 
           dbo.tblProduct.Description AS ProductName, 
           dbo.tblProductBrand.id AS BrandId, 
           dbo.tblProductBrand.name AS BrandName, 
           dbo.tblCostCentre.CostCentreType AS DistributorCCtype, 
           dbo.tblInventory.Balance AS InvBalance, 
           dbo.tblInventory.Value AS InvValue, 
           dbo.tblInventory.IM_DateCreated, 
           dbo.tblInventory.UnavailableBalance, 
           dbo.tblProduct.ExFactoryPrice, 
               tblCostCentre_1.Id AS SalesmanId, 
               tblCostCentre_1.Name AS SalesmanName, 
               tblCostCentre_1.CostCentreType AS SalesmanCCtype,
            dbo.tblProductType.id as productTypeId,
            dbo.tblProductType.name as productTypeName,
            dbo.tblProductFlavour.id as SubBrandId,
            dbo.tblProductFlavour.name as SubBrandName,
            dbo.tblProductPackaging.Id as PackageId,
            dbo.tblProductPackaging.Name as PackageName,
            dbo.tblProductPackagingType.id as PackagingTypeId,
            dbo.tblProductPackagingType.name as PackagingTypeName

FROM         dbo.tblCostCentre INNER JOIN
             dbo.tblCostCentre AS tblCostCentre_1 ON 
             dbo.tblCostCentre.Id = tblCostCentre_1.ParentCostCentreId INNER JOIN
             dbo.tblProductBrand INNER JOIN
             dbo.tblProduct INNER JOIN
             dbo.tblInventory ON dbo.tblProduct.id = dbo.tblInventory.ProductId ON 
             dbo.tblProductBrand.id = dbo.tblProduct.BrandId ON 
                 tblCostCentre_1.Id = dbo.tblInventory.WareHouseId
                 inner join dbo.tblProductType on dbo.tblProduct.ProductTypeId = dbo.tblProductType.id
                 inner join dbo.tblProductFlavour ON dbo.tblProductBrand.id = dbo.tblProductFlavour.BrandId
                 inner join dbo.tblProductPackaging ON dbo.tblProduct.PackagingId = dbo.tblProductPackaging.Id
                 inner join dbo.tblProductPackagingType ON dbo.tblProduct.PackagingTypeId = dbo.tblProductPackagingType.id

WHERE     (dbo.tblCostCentre.CostCentreType = 2) AND 
          (dbo.tblInventory.Balance >= 0) AND 
              (tblCostCentre_1.CostCentreType = 4)
           AND dbo.tblProduct.IM_Status = 1 --Only Active Product
           AND(CONVERT(NVARCHAR(50), tblProductBrand.id) LIKE ISNULL(@brandId,N'%'))
           AND(CONVERT(NVARCHAR(50),tblCostCentre.Id) LIKE ISNULL(@distributorId, N'%')) 
           AND(CONVERT(NVARCHAR(50),tblCostCentre_1.Id) LIKE ISNULL(@salesmanId, N'%')) 
           AND(CONVERT(NVARCHAR(50),dbo.tblProduct.id) LIKE ISNULL(@productId, N'%')) 
           AND(CONVERT(NVARCHAR(50),dbo.tblProductType.id) LIKE ISNULL(@productTypeId, N'%')) 
           AND(CONVERT(NVARCHAR(50),dbo.tblProductFlavour.id ) LIKE ISNULL(@subBrandId, N'%'))
           AND(CONVERT(NVARCHAR(50),dbo.tblProductPackaging.Id ) LIKE ISNULL(@packagingId, N'%')) 

GO
