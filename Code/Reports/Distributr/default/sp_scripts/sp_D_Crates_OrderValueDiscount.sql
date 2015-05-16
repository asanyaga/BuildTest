IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_D_Crates_OrderValueDiscount]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[sp_D_Crates_OrderValueDiscount]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[sp_D_Crates_OrderValueDiscount]
(
@startDate as datetime,
@endDate as datetime,
@countryId AS NVARCHAR(50),
@regionId AS NVARCHAR(50),
@distributorId AS NVARCHAR(50),
@salesmanId AS NVARCHAR(50),
@routeId AS NVARCHAR(50),
@outletId AS NVARCHAR(50),
@docId AS NVARCHAR(50)
)
as 
if  RTRIM(LTRIM(@countryId))='ALL'  begin set @countryId='%' end
if  RTRIM(LTRIM(@regionId))='ALL'  begin set @regionId='%' end
if  RTRIM(LTRIM(@distributorId))='ALL'  begin set @distributorId='%' end
if  RTRIM(LTRIM(@salesmanId))='ALL'  begin set @salesmanId='%' end
if  RTRIM(LTRIM(@routeId))='ALL'  begin set @routeId='%' end
if  RTRIM(LTRIM(@outletId))='ALL'  begin set @outletId='%' end
if  RTRIM(LTRIM(@docId))='ALL'  begin set @docId='%' end

SELECT  cntry.id CountryId,cntry.Name CountryName,
        reg.id RegionId,reg.Name RegionName, 
		dist.Id DistributorId,dist.Name DistributorName,
		sm.Id SalesmanId,sm.Name SalesmanName,
		sale.Id as saleId, 
		sale.SaleDiscount AS OrderValueDiscounts, 
		sale.DocumentDateIssued as SaleDate,
		sale.DocumentReference as SaleRef

FROM     dbo.tblDocument sale
	JOIN dbo.tblcostcentre dist ON (sale.DocumentIssuerCostCentreId = dist.id or sale.DocumentRecipientCostCentre = dist.Id)
	JOIN dbo.tblcostcentre sm ON (sale.DocumentIssuerCostCentreId = sm.id or sale.DocumentRecipientCostCentre = sm.Id)
	JOIN dbo.tblRegion reg ON dist.Distributor_RegionId = reg.id 
	JOIN dbo.tblCountry cntry ON reg.Country = cntry.id
WHERE    sale.DocumentTypeId = 1
     AND sale.OrderOrderTypeId = 3
	 AND dist.CostCentreType = 2
	 AND sm.CostCentreType = 4
	 AND CONVERT(NVARCHAR(50),cntry.id) LIKE ISNULL(@countryId,'%')
	 AND CONVERT(NVARCHAR(50),reg.id) LIKE ISNULL(@regionId,'%')
	 AND CONVERT(NVARCHAR(50),dist.id) LIKE ISNULL(@distributorId,'%')
	 AND CONVERT(NVARCHAR(50),sm.id) LIKE ISNULL(@salesmanId,'%')
     AND (CONVERT(NVARCHAR(26),DocumentDateIssued ,23)) between @startDate and @endDate


 -- EXEC [dbo].[sp_D_Crates_OrderValueDiscount] @startDate='2014-12-01',@endDate='2014-12-22',@countryId=' ALL',@regionId=' ALL',@distributorId=' ALL',@salesmanId=' ALL',@routeId=' ALL',@outletId=' ALL',@docId=' ALL'
 


           --AND((CONVERT(NVARCHAR(50),DocumentIssuerCostCentreId) LIKE ISNULL(@distributorID,'%') and CONVERT(NVARCHAR(50),DocumentRecipientCostCentre) LIKE ISNULL(@salesmanId,'%')) 
           -- or (CONVERT(NVARCHAR(50),DocumentIssuerCostCentreId) LIKE ISNULL(@salesmanId,'%') and CONVERT(NVARCHAR(50),DocumentRecipientCostCentre) LIKE ISNULL(@distributorID,'%'))) 
      