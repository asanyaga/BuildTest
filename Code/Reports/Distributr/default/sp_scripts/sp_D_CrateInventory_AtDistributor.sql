IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_D_CrateInventory_AtDistributor]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[sp_D_CrateInventory_AtDistributor]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_D_CrateInventory_AtDistributor]
(
@distributorId AS NVARCHAR(50),
@productId AS NVARCHAR(50)
)
AS 
if  LTRIM(rtrim(@distributorId)) = 'ALL'  begin set @distributorId='%' end
if  LTRIM(rtrim(@productId)) = 'ALL'  begin set @productId='%' end

SELECT     cc.Id AS DistributorId,
           cc.Name AS DistributorName, 
		   cc.CostCentreType AS Distcctype, 
		   cc.ParentCostCentreId, 
		   inv.Balance, 
		   prod.ExFactoryPrice, 
		   prod.Description AS ProductName, 
           prod.id AS ProductId,
		   prodPack.description PackagingName,
		   CAST(ROUND((inv.Balance/retunablePack.Capacity),1) AS INT) NoOfCrates,
		   CAST(ROUND((inv.Balance%retunablePack.Capacity),1) AS INT) ExtraBottles,
		   prod.Returnable ReturnableId,
		   retunable.Description ReturnableName,
		   retunablePack.Description ReturnablePack,
		   retunablePack.Capacity ReturnablePackCapacity,

		   CASE when cc.CostCentreType = 2 then 'Available Stock' END as StockType

FROM      dbo.tblInventory AS inv 
 JOIN     dbo.tblCostCentre AS cc ON inv.WareHouseId = cc.Id 
 JOIN     dbo.tblProduct AS prod ON inv.ProductId = prod.id 
 JOIN     dbo.tblProductBrand AS brand ON prod.BrandId = brand.id
 JOIN     dbo.tblProductPackaging prodPack ON prod.PackagingId = prodPack.Id
 JOIN     dbo.tblProduct retunable ON prod.Returnable = retunable.id
 JOIN     dbo.tblProduct retunablePack ON retunable.Returnable = retunablePack.id
		  
WHERE     (inv.Balance >= 0)
      AND (cc.CostCentreType = 2)
	  AND (CONVERT(NVARCHAR(50),cc.Id) LIKE ISNULL(@distributorId, N'%')) 
	  AND (CONVERT(NVARCHAR(50),prod.id) LIKE ISNULL(@productId, N'%')) 

group by cc.Id ,
           cc.Name , 
		   cc.CostCentreType , 
		   cc.ParentCostCentreId, 
		   inv.Balance, 
		   prod.ExFactoryPrice, 
		   prod.Description , 
           prod.id,prod.Returnable,
		   retunable.Description,
		   retunablePack.Description,
		   retunablePack.Capacity,
		   prodPack.description
having inv.Balance  > 0

UNION ALL

SELECT     cc.Id AS DistributorId,
           cc.Name AS DistributorName, 
		   cc.CostCentreType AS Distcctype, 
		   cc.ParentCostCentreId, 
		   inv.Balance, 
		   prod.ExFactoryPrice, 
		   prod.Description AS ProductName, 
           prod.id AS ProductId,
		   prodPack.description PackagingName,
		   CAST(ROUND((inv.Balance/retunablePack.Capacity),1) AS INT) NoOfCrates,
		   CAST(ROUND((inv.Balance%retunablePack.Capacity),1) AS INT) ExtraBottles,
		   prod.Returnable ReturnableId,
		   retunable.Description ReturnableName,
		   retunablePack.Description ReturnablePack,
		   retunablePack.Capacity ReturnablePackCapacity,

		   CASE when cc.CostCentreType = 6 then 'Pending Dispatch Stock' END as StockType

FROM      dbo.tblInventory AS inv 
 JOIN     dbo.tblCostCentre AS cc ON inv.WareHouseId = cc.Id 
 JOIN     dbo.tblProduct AS prod ON inv.ProductId = prod.id 
 JOIN     dbo.tblProductBrand AS brand ON prod.BrandId = brand.id
 JOIN     dbo.tblProductPackaging prodPack ON prod.PackagingId = prodPack.Id
 JOIN     dbo.tblProduct retunable ON prod.Returnable = retunable.id
 JOIN     dbo.tblProduct retunablePack ON retunable.Returnable = retunablePack.id
		  
WHERE     (inv.Balance >= 0)
      AND (cc.CostCentreType = 6)

	  AND (CONVERT(NVARCHAR(50),cc.Id) LIKE ISNULL(@distributorId, N'%')) 
	  AND (CONVERT(NVARCHAR(50),prod.id) LIKE ISNULL(@productId, N'%')) 

group by cc.Id ,
           cc.Name , 
		   cc.CostCentreType , 
		   cc.ParentCostCentreId, 
		   inv.Balance, 
		   prod.ExFactoryPrice, 
		   prod.Description , 
           prod.id,prod.Returnable,
		   retunable.Description,
		   retunablePack.Description,
		   retunablePack.Capacity,
		   prodPack.description
having inv.Balance  > 0

go

-- exec [dbo].[sp_D_CrateInventory_AtDistributor] @distributorId='ALL',@productId='ALL'