DROP PROCEDURE [dbo].[sp_D_DB_InventoryReceipt_PerRegion]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[sp_D_DB_InventoryReceipt_PerRegion]
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

SELECT     docs.DocumentTypeId, 
           docs.DocumentReference, 
           LineItems.Quantity * LineItems.Value as InvValue,             
           Hq.Id AS HqId, 
           Hq.Name AS HqName, 
           Hq.CostCentreType AS HqCCtype, 
           docs.IRNOrderReferences, 
           Distributor.Id AS DistributorId, 
           Distributor.Name AS DistributorName, 
           Distributor.CostCentreType AS DistributorCCtype, 
           Region.id AS RegionId, 
           Region.Name AS RegionName,
           Country.id AS CountryId, 
           Country.Name AS CountryName
FROM         dbo.tblDocument as docs INNER JOIN
             dbo.tblLineItems as LineItems ON docs.Id = LineItems.DocumentID INNER JOIN
             dbo.tblCostCentre as Hq ON docs.IRNGoodsReceivedFromCC = Hq.Id INNER JOIN
             dbo.tblCostCentre AS Distributor ON docs.DocumentRecipientCostCentre = Distributor.Id INNER JOIN
             dbo.tblRegion as Region ON Distributor.Distributor_RegionId = Region.id INNER JOIN
             dbo.tblCountry as Country ON Region.Country = Country.id
WHERE     (docs.DocumentTypeId = 3)
      AND (CONVERT(VARCHAR(26), docs.DocumentDateIssued,23) BETWEEN @startDate AND @endDate) 
      AND (CONVERT(VARCHAR(50),Country.id) LIKE ISNULL (@countryId,'%'))
      AND (CONVERT(VARCHAR(50),Region.id) LIKE ISNULL (@regionId,'%'))
      AND (CONVERT(VARCHAR(50),Hq.id) LIKE ISNULL (@HqId,'%'))
      AND (CONVERT(VARCHAR(50),Distributor.id) LIKE ISNULL (@distributorId,'%'))
      
      
 --  Exec  [dbo].[sp_D_DB_InventoryReceipt_PerRegion]   @regionId='ALL',@countryId='ALL',@HqId='ALL',@distributorId='ALL',@routesId='ALL',@salesmanId='ALL',@outletId='ALL',@startDate ='1-Jan-2013',@endDate ='31-Dec-2013'

 GO     

      





