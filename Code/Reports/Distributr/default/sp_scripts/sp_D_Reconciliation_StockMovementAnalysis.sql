/****** Object:  StoredProcedure [dbo].[sp_D_Reconciliation_StockMovementAnalysis]    Script Date: 07/24/2013 08:51:32 ******/
DROP PROCEDURE [dbo].[sp_D_Reconciliation_StockMovementAnalysis]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_D_Reconciliation_StockMovementAnalysis]
(
@startDate AS DATE,
@endDate AS DATE,
@productId AS NVARCHAR(50),
@distributorId AS NVARCHAR(50)
)
AS 
if LTRIM(RTRIM(@distributorId))='ALL' begin set @distributorId='%' end
if LTRIM(RTRIM(@productId))='ALL' begin set @productId='%' end

--OPENING BALANCE

SELECT     
            country.id AS CountryId, 
            country.Name AS CountryName,
            region.id AS RegionId, 
            region.Name AS RegionName,
            distributor.Id as DistributorId,
            distributor.Name as DistributorName, 
            brand.id BrandId,
            brand.name BrandName, 
            prod.id as ProductId, 
            prod.Description as ProductName,
            --inv.Balance, 
            --inv.Value,             
            invTrans.DateInserted, 
            --invTrans.NoItems,
           (inv.Balance - invTrans.NoItems) as OpeningBalance,
            0 as StockIssue, -- Purchases
            0 as SoldStock
            
             

FROM         dbo.tblInventory inv
 INNER JOIN  dbo.tblInventoryTransaction invTrans ON inv.id = invTrans.InventoryId 
 INNER JOIN  dbo.tblProduct prod ON inv.ProductId = prod.id 
 INNER JOIN  dbo.tblCostCentre distributor ON inv.WareHouseId = distributor.Id 
 INNER JOIN  dbo.tblRegion region ON distributor.Distributor_RegionId = region.id 
 INNER JOIN  dbo.tblCountry country ON region.Country = country.id
 INNER JOIN  dbo.tblProductBrand brand ON prod.BrandId = brand.id

WHERE     (distributor.CostCentreType = 2)
      AND (CONVERT(VARCHAR(26),invTrans.DateInserted,23) BETWEEN @startDate AND @endDate)

union all 

--PURCHASES
SELECT     country.id AS CountryId, 
           country.Name AS CountryName,
           region.id AS RegionId, 
           region.Name AS RegionName, 
           distributor.Id AS DistributorId, 
           distributor.Name AS DistributorName, 
           dbo.tblProductBrand.id AS BrandId, 
           dbo.tblProductBrand.name AS BrandName, 
           prod.id AS ProductId, 
           prod.Description AS ProductName, 
           purch.DocumentDateIssued AS TransDate,
           0 as OpeningBalance ,
           purchItems.Quantity AS StockIssues,
           0 AS SoldStock 
FROM        
             dbo.tblDocument purch
INNER JOIN   dbo.tblLineItems purchItems ON purch.Id = purchItems.DocumentID 
INNER JOIN   dbo.tblProduct prod ON purchItems.ProductID = prod.id 
INNER JOIN   dbo.tblCostCentre distributor ON purch.DocumentRecipientCostCentre = distributor.Id 
INNER JOIN   dbo.tblRegion region ON distributor.Distributor_RegionId = region.id 
INNER JOIN   dbo.tblCountry country ON region.Country = country.id 
INNER JOIN   dbo.tblProductBrand ON prod.BrandId = dbo.tblProductBrand.id
WHERE     (purch.DocumentTypeId = 1) 
      AND (purch.OrderOrderTypeId = 2) 
      AND (purch.DocumentStatusId = 99) 
      AND (purch.Id = purch.OrderParentId) 
      AND (distributor.CostCentreType = 2)
      AND (CONVERT(VARCHAR(26),purch.DocumentDateIssued,23) BETWEEN @startDate AND @endDate)
      
    union all
    
    --SALES
    
    SELECT country.id AS CountryId, 
           country.Name AS CountryName, 
           region.id AS RegionId, 
           region.Name AS RegionName, 
           distributor.Id AS DistributorId, 
           distributor.Name AS DistributorName, 
           brand.id AS BrandId, 
           brand.name AS BrandName, 
           prod.id AS ProductId, 
           prod.Description AS ProductName, 
           sales.DocumentDateIssued AS TransDate,
            0 as OpeningBalance,
            0 as StockIssues,
           salesItems.Quantity as SoldStock

FROM       dbo.tblDocument sales
INNER JOIN dbo.tblLineItems salesItems ON sales.Id = salesItems.DocumentID 
INNER JOIN dbo.tblProduct prod ON salesItems.ProductID = prod.id 
INNER JOIN dbo.tblCostCentre distributor ON sales.DocumentRecipientCostCentre = distributor.Id OR sales.DocumentIssuerCostCentreId = distributor.Id 
INNER JOIN dbo.tblRegion region ON distributor.Distributor_RegionId = region.id 
INNER JOIN dbo.tblCountry country ON region.Country = country.id 
INNER JOIN dbo.tblProductBrand brand ON prod.BrandId = brand.id

WHERE     (distributor.CostCentreType = 2) 
      AND (CONVERT(VARCHAR(26),sales.DocumentDateIssued,23) BETWEEN @startDate AND @endDate)
      AND (sales.Id = sales.OrderParentId) 
      AND (sales.DocumentTypeId = 1) 
      AND (distributor.CostCentreType = 2) 
      AND (sales.OrderOrderTypeId = 1) OR (distributor.CostCentreType = 2) 
      AND (sales.Id = sales.OrderParentId) 
      AND (sales.DocumentTypeId = 1) 
      AND (distributor.CostCentreType = 2) 
      AND (sales.OrderOrderTypeId = 3) 
      AND (sales.DocumentStatusId = 99)
      
             
              ---END--
   --  Exec [dbo].[sp_D_Reconciliation_StockMovementAnalysis] @startDate ='30-sep-2013', @endDate = '25-Dec-2013',@productId='',@distributorId=''

GO
