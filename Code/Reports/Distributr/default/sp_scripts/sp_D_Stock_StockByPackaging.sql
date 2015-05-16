/****** Object:  StoredProcedure [dbo].[sp_D_Stock_StockByPackaging]    Script Date: 07/24/2013 08:51:32 ******/
DROP PROCEDURE [dbo].[sp_D_Stock_StockByPackaging]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_D_Stock_StockByPackaging]
(
@distributorId AS NVARCHAR(50),
@salesmanId AS NVARCHAR(50),
@brandId AS NVARCHAR(50),
@productId AS NVARCHAR(50),
@packagingId AS NVARCHAR(50),
@flavourId AS NVARCHAR(50)

)
AS 
if  ltrim(rtrim(@distributorId))='ALL'  begin set @distributorId='%' end
if  ltrim(rtrim(@salesmanId))='ALL'  begin set @salesmanId='%' end
if  ltrim(rtrim(@brandId))='ALL'  begin set @brandId='%' end
if  ltrim(rtrim(@productId))='ALL'  begin set @productId='%' end
if  ltrim(rtrim(@packagingId))='ALL'  begin set @packagingId='%' end
if  ltrim(rtrim(@flavourId))='ALL'  begin set @flavourId='%' end

SELECT     CONVERT(NVARCHAR(50), dbo.tblCostCentre.Id) AS DistributorId, 
           dbo.tblCostCentre.Name AS DistributorName, 
           
           dbo.tblProduct.id AS ProductId, 
           dbo.tblProduct.Description AS ProductName, 
           
           dbo.tblProductBrand.id AS BrandId, 
           dbo.tblProductBrand.name AS BrandName, 
           
           flav.id FlavourId,
           flav.name FlavourName,
           
           case when dbo.tblProductPackaging.Id is null then cast(cast(0 as binary) as uniqueidentifier) else dbo.tblProductPackaging.Id end as PackagingId,
           case when dbo.tblProductPackaging.Name is null then '' else dbo.tblProductPackaging.Name end as  PackagingName,  
           
           dbo.tblCostCentre.CostCentreType ,
           dbo.tblInventory.Balance AS InvBalance, 
           dbo.tblInventory.Value AS InvValue, 
           dbo.tblInventory.IM_DateCreated, 
           dbo.tblInventory.UnavailableBalance, 
           dbo.tblProduct.ExFactoryPrice
           
FROM        dbo.tblProductBrand
 INNER JOIN dbo.tblProduct 
 LEFT OUTER JOIN dbo.tblProductPackaging ON dbo.tblProduct.PackagingId = dbo.tblProductPackaging.Id
 INNER JOIN dbo.tblProductFlavour flav ON flav.id = dbo.tblProduct.FlavourId 
 inner join dbo.tblCostCentre 
 INNER JOIN dbo.tblInventory ON dbo.tblCostCentre.Id = dbo.tblInventory.WareHouseId ON dbo.tblProduct.id = dbo.tblInventory.ProductId ON dbo.tblProductBrand.id = dbo.tblProduct.BrandId
 
WHERE     (dbo.tblInventory.Balance >= 0) 
      AND (dbo.tblCostCentre.CostCentreType = 2)
      AND dbo.tblProduct.IM_Status = 1 --Only Active Product
      AND(CONVERT(NVARCHAR(50), tblProductBrand.id) LIKE ISNULL(@brandId,N'%'))
      AND(CONVERT(NVARCHAR(50),tblCostCentre.Id) LIKE ISNULL(@distributorId, N'%')) 
      AND(CONVERT(NVARCHAR(50),dbo.tblCostCentre.Id) LIKE ISNULL(@salesmanId, N'%')) 
      AND(CONVERT(NVARCHAR(50),dbo.tblProduct.id) LIKE ISNULL(@productId, N'%')) 
      AND(CONVERT(NVARCHAR(50),dbo.tblProductPackaging.Id) LIKE ISNULL(@packagingId, N'%')) 


union all
SELECT     CONVERT(NVARCHAR(50), tblCostCentre_1.Id) AS DistributorId,  
           tblCostCentre_1.Name AS DistributorName, 
           
           dbo.tblProduct.id AS ProductId, 
           dbo.tblProduct.Description AS ProductName, 
           
           dbo.tblProductBrand.id AS BrandId,            
           dbo.tblProductBrand.name AS BrandName, 
           
           flav.id FlavourId,
           flav.name FlavourName,  
                    
           case when dbo.tblProductPackaging.Id is null then cast(cast(0 as binary) as uniqueidentifier) else dbo.tblProductPackaging.Id end as PackagingId,
           case when dbo.tblProductPackaging.Name is null then '' else dbo.tblProductPackaging.Name end as  PackagingName,  
           
           dbo.tblCostCentre.CostCentreType ,    
           dbo.tblInventory.Balance AS InvBalance, 
           dbo.tblInventory.Value AS InvValue, 
           dbo.tblInventory.IM_DateCreated, 
           dbo.tblInventory.UnavailableBalance, 
           dbo.tblProduct.ExFactoryPrice

FROM       dbo.tblProductBrand 
INNER JOIN dbo.tblProduct 
LEFT OUTER JOIN dbo.tblProductPackaging ON dbo.tblProduct.PackagingId = dbo.tblProductPackaging.Id
INNER JOIN dbo.tblProductFlavour flav ON flav.id = dbo.tblProduct.FlavourId 
inner join dbo.tblCostCentre 
INNER JOIN dbo.tblInventory ON dbo.tblCostCentre.Id = dbo.tblInventory.WareHouseId ON dbo.tblProduct.id = dbo.tblInventory.ProductId ON dbo.tblProductBrand.id = dbo.tblProduct.BrandId 
INNER JOIN dbo.tblCostCentre AS tblCostCentre_1 ON dbo.tblCostCentre.ParentCostCentreId = tblCostCentre_1.Id

WHERE     (dbo.tblInventory.Balance >= 0) 
      AND (dbo.tblCostCentre.CostCentreType = 4)
      AND dbo.tblProduct.IM_Status = 1 --Only Active Product
      AND(CONVERT(NVARCHAR(50), tblProductBrand.id) LIKE ISNULL(@brandId,N'%'))
      AND(CONVERT(NVARCHAR(50),tblCostCentre_1.Id) LIKE ISNULL(@distributorId, N'%')) 
      AND(CONVERT(NVARCHAR(50),dbo.tblCostCentre.Id) LIKE ISNULL(@salesmanId, N'%')) 
      AND(CONVERT(NVARCHAR(50),dbo.tblProduct.id) LIKE ISNULL(@productId, N'%')) 
      AND(CONVERT(NVARCHAR(50),dbo.tblProductPackaging.Id) LIKE ISNULL(@packagingId, N'%')) 
   
           


       
 --  EXEC [dbo].[sp_D_Stock_StockByPackaging] @brandId='ALL',@distributorId='ALL',@salesmanId='ALL',@productId='ALL',@packagingId='ALL',@flavourId='ALL'   
       
           
           
GO
