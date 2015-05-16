IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_D_CrateInventory_BySalesman]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[sp_D_CrateInventory_BySalesman]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_D_CrateInventory_BySalesman]
(
@distributorId AS NVARCHAR(50),
@salesmanId AS NVARCHAR(50),
@productId AS NVARCHAR(50)
)
AS 
if  LTRIM(rtrim(@distributorId)) = 'ALL'  begin set @distributorId='%' end
if  LTRIM(rtrim(@salesmanId)) = 'ALL'  begin set @salesmanId='%' end
if  LTRIM(rtrim(@productId)) = 'ALL'  begin set @productId='%' end

SELECT     dist.Id AS DistributorId, 
           dist.Name AS DistributorName,
           sm.Id AS SalesmanId, 
           sm.Name AS SalesmanName,
           prod.id AS ProductId, 
           prod.Description AS ProductName, 
           brand.id AS BrandId, 
           brand.name AS BrandName, 
           sum(inv.Balance) AS InvBalance, 
           inv.Value AS InvValue, 
           sum(inv.UnavailableBalance) UnavailableBalance, 
           prod.ExFactoryPrice ,
  		   prodPack.description PackagingName,
		   CAST(ROUND((inv.Balance/retunablePack.Capacity),1) AS INT) NoOfCrates,
		   CAST(ROUND((inv.Balance%retunablePack.Capacity),1) AS INT) ExtraBottles,
		   prod.Returnable ReturnableId,
		   retunable.Description ReturnableName,
		   retunablePack.Description ReturnablePack,
		   retunablePack.Capacity ReturnablePackCapacity
FROM       dbo.tblCostCentre  dist
JOIN       dbo.tblCostCentre AS sm ON dist.Id = sm.ParentCostCentreId 
JOIN       dbo.tblProductBrand brand
JOIN       dbo.tblProduct prod
JOIN       dbo.tblProductPackaging prodPack ON prod.PackagingId = prodPack.Id
JOIN       dbo.tblProduct retunable ON prod.Returnable = retunable.id
JOIN       dbo.tblProduct retunablePack ON retunable.Returnable = retunablePack.id
JOIN       dbo.tblInventory inv ON prod.id = inv.ProductId
                                ON brand.id = prod.BrandId 
							    ON sm.Id = inv.WareHouseId

WHERE     (dist.CostCentreType = 2) 
AND       (inv.Balance >= 0) 
AND       (sm.CostCentreType = 4)
AND       CONVERT(nvarchar(50),dist.Id) like isnull(@distributorId,'%')
AND       CONVERT(nvarchar(50),sm.Id) like isnull(@salesmanId,'%')
AND       CONVERT(nvarchar(50),prod.id) like isnull(@productId,'%')
GROUP BY dist.Id,dist.Name,
         sm.Id,sm.Name,
		 prod.id,prod.Description,
		 brand.id,brand.name,
		 inv.Value,prod.ExFactoryPrice, prod.Returnable,retunable.Description,retunablePack.Description,retunablePack.Capacity,
		 prod.ExFactoryPrice ,prodPack.description,inv.Value,inv.Balance

-- Exec [dbo].[sp_D_CrateInventory_BySalesman] @distributorId ='ALL',@productId='ALL'