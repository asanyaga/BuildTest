IF NOT EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'sp_A_DepositorTransaction')
   exec('CREATE PROCEDURE [sp_A_DepositorTransaction] AS BEGIN SET NOCOUNT ON; END')
GO
Alter PROCEDURE sp_A_DepositorTransaction
(
@StartDate AS DATE,
@EndDate AS DATE,
@CountryId AS NVARCHAR(50),
@LocationId AS NVARCHAR(50),
@HubId AS NVARCHAR(50),
@StoreId AS NVARCHAR(50),
@ClerkId AS NVARCHAR(50),
@DepositorId AS NVARCHAR(50),
@CommodityId AS NVARCHAR(50),
@GradeId AS NVARCHAR(50)
)
AS

BEGIN    

if  @CountryId='ALL'  begin set @CountryId='%' end
if  @LocationId='ALL'  begin set @LocationId='%' end
if  @HubId='ALL'  begin set @HubId='%' end
if  @StoreId='ALL'  begin set @StoreId='%' end
if  @ClerkId='ALL'  begin set @ClerkId='%' end
if  @DepositorId='ALL'  begin set @DepositorId='%' end
if  @CommodityId='ALL'  begin set @CommodityId='%' end
if  @GradeId='ALL'  begin set @GradeId='%' end

SELECT DISTINCT doc.Id,CONVERT(NVARCHAR(26),doc.DocumentDate,23) AS DepositDate,
	doc.DocumentReference,
	clerk.Name AS Clerk,
	hub.Name AS Warehouse,
	tblRegion.Name AS Location, 
	tblCommodityOwner.FirstName AS Depositor,
	tblCommodity.Name AS Commodity,
	tblCommodityGrade.Name AS Grade,
	item.Weight AS NetWeight,
	CONVERT(NVARCHAR(26),DATEADD(MM,6,DocumentDate),23) AS ExpiryDate

FROM tblSourcingDocument doc INNER JOIN
	tblSourcingLineItem item ON doc.Id = item.DocumentId INNER JOIN
	tblCostCentre hub ON doc.DocumentIssuerCostCentreId = hub.Id OR doc.DocumentRecipientCostCentreId = hub.Id INNER JOIN
	tblCostCentre AS clerk ON hub.Id = clerk.ParentCostCentreId INNER JOIN
	tblCostCentre AS store ON hub.Id = store.ParentCostCentreId INNER JOIN
	tblCommodity ON item.CommodityId = tblCommodity.Id INNER JOIN
	tblCommodityGrade ON item.GradeId = tblCommodityGrade.Id INNER JOIN
	tblRegion ON hub.Distributor_RegionId = tblRegion.id INNER JOIN
	tblCountry ON tblRegion.Country = tblCountry.id INNER JOIN
	tblCommodityOwner ON doc.CommodityProducerId = tblCommodityOwner.CostCentreId

WHERE (doc.DocumentTypeId = 27) 
	AND (hub.CostCentreType = 8) 
	AND (clerk.CostCentreType = 10) 
	AND (store.CostCentreType = 11)
	AND (CONVERT(VARCHAR(26),doc.DocumentDate,23)  BETWEEN @startDate AND @endDate)             
    AND(CONVERT(NVARCHAR(50),tblCountry.id) LIKE ISNULL(@CountryId, N'%'))  
    AND(CONVERT(NVARCHAR(50),tblRegion.id) LIKE ISNULL(@LocationId, N'%'))
	AND(CONVERT(NVARCHAR(50),hub.Id) LIKE ISNULL(@HubId, N'%'))
	AND(CONVERT(NVARCHAR(50),store.Id) LIKE ISNULL(@StoreId, N'%')) 
	AND(CONVERT(NVARCHAR(50),clerk.Id) LIKE ISNULL(@ClerkId, N'%'))
    AND(CONVERT(NVARCHAR(50),dbo.tblCommodityOwner.Id) LIKE ISNULL(@DepositorId, N'%'))
	AND(CONVERT(NVARCHAR(50),tblCommodity.Id) LIKE ISNULL(@CommodityId, N'%'))
	AND(CONVERT(NVARCHAR(50),tblCommodityGrade.Id) LIKE ISNULL(@GradeId, N'%'))
	
END;

-- EXEC sp_A_DepositorTransaction @StartDate='2015-06-01',@EndDate='2015-07-23',@HubId='ALL',@ClerkId='ALL',@CountryId='ALL',@LocationId='ALL',@DepositorId='ALL',@StoreId='ALL',@CommodityId='ALL',@GradeId='ALL'
