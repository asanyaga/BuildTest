/****** Object:  StoredProcedure [dbo].[sp_D_Inventory_StockSummary_PerDistributor]    Script Date: 07/24/2013 08:51:32 ******/
DROP PROCEDURE [dbo].[sp_D_Inventory_StockSummary_PerDistributor]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_D_Inventory_StockSummary_PerDistributor]
(
@brandId AS NVARCHAR(50),
@productId AS NVARCHAR(50),
@packagingId AS NVARCHAR(50),
@distributorId AS NVARCHAR(50),
@salesmanId AS NVARCHAR(50),
@Brand AS NVARCHAR(50),
@Product AS NVARCHAR(50)
)
AS 
if  ltrim(rtrim(@distributorId))='ALL'  begin set @distributorId='%' end
if  ltrim(rtrim(@salesmanId))='ALL'  begin set @salesmanId='%' end
if  ltrim(rtrim(@brandId))='ALL'  begin set @brandId='%' end
if  ltrim(rtrim(@productId))='ALL'  begin set @productId='%' end
if  ltrim(rtrim(@packagingId))='ALL'  begin set @packagingId='%' end
if  ltrim(rtrim(@Brand))='ALL'  begin set @Brand='%' end
if  ltrim(rtrim(@Product))='ALL'  begin set @Product='%' end

SELECT     dbo.tblCostCentre.Id AS DistributorId, 
           dbo.tblCostCentre.Name AS DistributorName, 
           dbo.tblProduct.id AS ProductId, 
           dbo.tblProduct.Description AS ProductName, 
           dbo.tblProductBrand.id AS BrandId, 
           dbo.tblProductBrand.name AS BrandName, 
           dbo.tblCostCentre.CostCentreType, 
           dbo.tblInventory.Balance AS InvBalance, 
           dbo.tblInventory.Value AS InvValue, 
           dbo.tblInventory.IM_DateCreated, 
           dbo.tblInventory.UnavailableBalance, 
           dbo.tblProduct.ExFactoryPrice

FROM       dbo.tblProductBrand INNER JOIN
           dbo.tblProduct INNER JOIN
           dbo.tblCostCentre INNER JOIN
           dbo.tblInventory ON 
           dbo.tblCostCentre.Id = dbo.tblInventory.WareHouseId ON
           dbo.tblProduct.id = dbo.tblInventory.ProductId ON 
           dbo.tblProductBrand.id = dbo.tblProduct.BrandId

WHERE     (dbo.tblCostCentre.CostCentreType = 2) AND 
          (dbo.tblInventory.Balance >= 0)
           AND (dbo.tblProductBrand.name LIKE ISNULL(@Brand, N'%')) 
           AND(CONVERT(NVARCHAR(50), tblProductBrand.id) LIKE ISNULL(@brandId,N'%'))
           AND(CONVERT(NVARCHAR(50),tblCostCentre.Id) LIKE ISNULL(@distributorId, N'%')) 
           AND(CONVERT(NVARCHAR(50),dbo.tblCostCentre.Id) LIKE ISNULL(@salesmanId, N'%')) 
           AND(CONVERT(NVARCHAR(50),dbo.tblProduct.id) LIKE ISNULL(@productId, N'%')) 
           AND(dbo.tblProduct.Description LIKE ISNULL(@Product, N'%'))
           AND dbo.tblProduct.IM_Status = 1 --Only Active Product
           
-- Exec   [dbo].[sp_D_Inventory_StockSummary_PerDistributor]  @distributorId='ALL',@salesmanId='ALL',@brandId='ALL',@productId='ALL',@packagingId='ALL',@Brand ='ALL',@Product ='ALL'         

           
GO
