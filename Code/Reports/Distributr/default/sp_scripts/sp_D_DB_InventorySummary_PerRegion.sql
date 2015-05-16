DROP PROCEDURE [dbo].[sp_D_DB_InventorySummary_PerRegion]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[sp_D_DB_InventorySummary_PerRegion]
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

SELECT      Distributor.Id AS DistributorId, 
            Distributor.Name AS DistributorName, 
            Distributor.CostCentreType AS DistributorCCtype,
            Inventory.Balance AS InvBalance, 
            Inventory.Value AS InvValue, 
            Inventory.IM_DateCreated, 
            Inventory.UnavailableBalance, 
            Region.id AS RegionId, 
            Region.Name AS RegionName, 
            Country.id AS CountryId, 
            Country.Name AS CountryName, 
                Hq.Id AS HqId, 
                Hq.Name AS HqName
FROM         dbo.tblCostCentre  as Distributor
          INNER JOIN  dbo.tblInventory as Inventory ON Distributor.Id = Inventory.WareHouseId 
          INNER JOIN  dbo.tblRegion as Region ON  Distributor.Distributor_RegionId = Region.id 
          INNER JOIN  dbo.tblCountry as Country ON Region.Country = Country.id 
          INNER JOIN  dbo.tblCostCentre AS Hq ON Distributor.ParentCostCentreId = Hq.Id
WHERE     (Distributor.CostCentreType = 2) 
      AND (Inventory.Balance >= 0)
      AND (CONVERT(VARCHAR(50),Country.id) LIKE ISNULL (@countryId,'%'))
      AND (CONVERT(VARCHAR(50),Region.id) LIKE ISNULL (@regionId,'%'))
      AND (CONVERT(VARCHAR(50),Hq.id) LIKE ISNULL (@HqId,'%'))
      AND (CONVERT(VARCHAR(50),Distributor.id) LIKE ISNULL (@distributorId,'%'))
      
      
 --  Exec  [dbo].[sp_D_DB_InventorySummary_PerRegion]   @regionId='ALL',@countryId='ALL',@HqId='ALL',@distributorId='ALL',@routesId='ALL',@salesmanId='ALL',@outletId='ALL',@startDate ='1-Jan-2013',@endDate ='31-Dec-2013'

 GO     

      





