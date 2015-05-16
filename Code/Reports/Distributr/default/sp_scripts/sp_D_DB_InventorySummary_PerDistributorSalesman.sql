DROP PROCEDURE [dbo].[sp_D_DB_InventorySummary_PerDistributorSalesman]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[sp_D_DB_InventorySummary_PerDistributorSalesman]
(
@startDate as datetime,
@endDate as datetime, 
@countryId as nvarchar(100),
@regionId as nvarchar(100),
@HqId as nvarchar(100),
@distributorId as nvarchar(100),
@routesId as nvarchar(100),
@salesmanId as nvarchar(100),
@outletId as nvarchar(100)
)
as 
if  @regionId='ALL'   begin set @regionId='%' end
if  @countryId='ALL'  begin set @countryId='%' end
if  @HqId='ALL'  begin set @HqId='%' end
if  @distributorId='ALL'  begin set @distributorId='%' end
if  @routesId='ALL'  begin set @routesId='%' end
if  @salesmanId='ALL'  begin set @salesmanId='%' end
if  @outletId='ALL'  begin set @outletId='%' end

SELECT     Salesman.Id AS SalesmanId, 
           Salesman.Name AS SalesmanName, 
           Salesman.CostCentreType AS SalesmanCCtype, 
           Inventory.Balance AS InvBalance, 
           Inventory.Value AS InvValue, 
           Inventory.IM_DateCreated, 
           Inventory.UnavailableBalance, 
           Region.id AS RegionId, 
           Region.Name AS RegionName, 
           Country.id AS CountryId, 
           Country.Name AS CountryName, 
           Hq.Id AS HqId, 
           Hq.Name AS HqName, 
           dbo.tblCostCentre.Id as DistributorId, 
           dbo.tblCostCentre.Name as DistributorName, 
           dbo.tblCostCentre.CostCentreType as DistributorCCtype
FROM         dbo.tblCostCentre INNER JOIN
                      dbo.tblCostCentre AS Salesman INNER JOIN
                      dbo.tblInventory AS Inventory ON Salesman.Id = Inventory.WareHouseId ON dbo.tblCostCentre.Id = Salesman.ParentCostCentreId INNER JOIN
                      dbo.tblCostCentre AS Hq ON dbo.tblCostCentre.ParentCostCentreId = Hq.Id INNER JOIN
                      dbo.tblCountry AS Country INNER JOIN
                      dbo.tblRegion AS Region ON Country.id = Region.Country ON dbo.tblCostCentre.Distributor_RegionId = Region.id
WHERE     (Salesman.CostCentreType = 4) AND (Inventory.Balance >= 0)
      AND (CONVERT(VARCHAR(50),Country.id) LIKE ISNULL (@countryId,'%'))
      AND (CONVERT(VARCHAR(50),Region.id) LIKE ISNULL (@regionId,'%'))
      AND (CONVERT(VARCHAR(50),Hq.id) LIKE ISNULL (@HqId,'%'))
      AND (CONVERT(VARCHAR(50),Salesman.id) LIKE ISNULL (@salesmanId,'%'))
      AND (CONVERT(VARCHAR(50),tblCostCentre.Id) LIKE ISNULL (@distributorId,'%'))

      
      
 --  Exec  [dbo].[sp_D_DB_InventorySummary_PerDistributorSalesman]   @regionId='ALL',@countryId='ALL',@HqId='ALL',@distributorId='ALL',@routesId='ALL',@salesmanId='ALL',@outletId='ALL',@startDate ='1-Jan-2013',@endDate ='31-Dec-2013'

 GO     

      





