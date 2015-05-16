IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_D_CrateInventory_ByDistributor]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[sp_D_CrateInventory_ByDistributor]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_D_CrateInventory_ByDistributor]
(
@distributorId AS NVARCHAR(50),
@productId AS NVARCHAR(50)
)
AS 
if  LTRIM(rtrim(@distributorId)) = 'ALL'  begin set @distributorId='%' end
if  LTRIM(rtrim(@productId)) = 'ALL'  begin set @productId='%' end

SELECT     CONVERT(NVARCHAR(50), dbo.tblCostCentre.Id) AS DistributorId, 
           dbo.tblCostCentre.Name AS DistributorName, 
           prod.id AS ProductId, 
           prod.Description AS ProductName, 
           brand.id AS BrandId, 
           brand.name AS BrandName, 
           inv.Balance AS InvBalance, 
           inv.Value AS InvValue, 
           inv.UnavailableBalance, 
           prod.ExFactoryPrice,
   		   CAST(ROUND((inv.Balance/retunablePack.Capacity),1) AS INT) NoOfCrates,
		   CAST(ROUND((inv.Balance%retunablePack.Capacity),1) AS INT) ExtraBottles,
		   prod.Returnable ReturnableId,
		   retunable.Description ReturnableName,
		   retunablePack.Description ReturnablePack,
		   retunablePack.Capacity ReturnablePackCapacity

FROM       dbo.tblProductBrand brand
JOIN       dbo.tblProduct prod
JOIN       dbo.tblProductPackaging prodPack ON prod.PackagingId = prodPack.Id
JOIN       dbo.tblProduct retunable ON prod.Returnable = retunable.id
JOIN       dbo.tblProduct retunablePack ON retunable.Returnable = retunablePack.id
JOIN       dbo.tblCostCentre 
JOIN       dbo.tblInventory inv ON dbo.tblCostCentre.Id = inv.WareHouseId 
		                        ON prod.id = inv.ProductId 
							    ON brand.id = prod.BrandId
WHERE     (inv.Balance >= 0) 
           AND (dbo.tblCostCentre.CostCentreType = 2)
           AND prod.IM_Status = 1 --Only Active Product
           AND(CONVERT(NVARCHAR(50),tblCostCentre.Id) LIKE ISNULL(@distributorId, N'%')) 
           AND(CONVERT(NVARCHAR(50),prod.id) LIKE ISNULL(@productId, N'%')) 
union
SELECT     CONVERT(NVARCHAR(50), tblCostCentre_1.Id) AS DistributorId,  
           tblCostCentre_1.Name AS DistributorName, 
           prod.id AS ProductId, 
           prod.Description AS ProductName, 
           brand.id AS BrandId,            
           brand.name AS BrandName, 
           inv.Balance AS InvBalance, 
           inv.Value AS InvValue, 
           inv.UnavailableBalance, 
           prod.ExFactoryPrice,
   		   CAST(ROUND((inv.Balance/retunablePack.Capacity),1) AS INT) NoOfCrates,
		   CAST(ROUND((inv.Balance%retunablePack.Capacity),1) AS INT) ExtraBottles,
		   prod.Returnable ReturnableId,
		   retunable.Description ReturnableName,
		   retunablePack.Description ReturnablePack,
		   retunablePack.Capacity ReturnablePackCapacity


FROM       dbo.tblProductBrand brand  
JOIN       dbo.tblProduct prod 
JOIN       dbo.tblProductPackaging prodPack ON prod.PackagingId = prodPack.Id
JOIN       dbo.tblProduct retunable ON prod.Returnable = retunable.id
JOIN       dbo.tblProduct retunablePack ON retunable.Returnable = retunablePack.id
 
JOIN       dbo.tblCostCentre  
JOIN       dbo.tblInventory inv ON dbo.tblCostCentre.Id = inv.WareHouseId 
		                    ON prod.id = inv.ProductId 
							ON brand.id = prod.BrandId 
JOIN       dbo.tblCostCentre AS tblCostCentre_1 ON dbo.tblCostCentre.ParentCostCentreId = tblCostCentre_1.Id
           
WHERE     (inv.Balance >= 0) 
           AND (dbo.tblCostCentre.CostCentreType = 4)
           AND prod.IM_Status = 1 --Only Active Product
           AND(CONVERT(NVARCHAR(50),tblCostCentre_1.Id) LIKE ISNULL(@distributorId, N'%')) 
           AND(CONVERT(NVARCHAR(50),prod.id) LIKE ISNULL(@productId, N'%')) 


-- exec [dbo].[sp_D_CrateInventory_ByDistributor] @distributorId ='ALL',@productId ='ALL'


