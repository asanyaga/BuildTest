DROP PROCEDURE [dbo].[SP_A_CommodityPurchaseException] 
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[SP_A_CommodityPurchaseException]
(
@startDate as datetime,
@endDate as datetime,
@HubId AS NVARCHAR(50),
@FarmerId AS NVARCHAR(50),
@CommodityId AS NVARCHAR(50),
@variance as Int
)
as 
if  rtrim(ltrim(@HubId))='ALL'  begin set @HubId='%' end
if  rtrim(ltrim(@FarmerId))='ALL'  begin set @FarmerId='%' end
if  rtrim(ltrim(@CommodityId))='ALL'  begin set @CommodityId='%' end
if @variance = null begin set @variance='%' end

SELECT	dbo.tblSourcingDocument.DocumentReference,
		dbo.tblCostCentre.Id AS HubId,
		dbo.tblCostCentre.Name AS Hub, 
		dbo.tblCommodityOwner.Id AS FarmerId, 
		dbo.tblCommodityOwner.Surname, 
		dbo.tblCommodityOwner.FirstName, 
		dbo.tblCommodityOwner.Code,
		convert(nvarchar(26),dbo.tblSourcingDocument.DocumentDate,23) as PurchaseDate,
		convert(nvarchar(26),dbo.tblSourcingDocument.DateSent,23) as SentDate,
		cast(DATEDIFF(DAY,dbo.tblSourcingDocument.DocumentDate,dbo.tblSourcingDocument.DateSent) as nvarchar(50))+' Days' as Variance,
		CASE
			WHEN DocumentStatusId = 0 THEN 'New'        
			WHEN DocumentStatusId = 1 THEN 'Confirmed'
			WHEN DocumentStatusId = 2 THEN 'Submitted'
			WHEN DocumentStatusId = 3 THEN 'Cancelled'
			WHEN DocumentStatusId = 4 THEN 'Approved'
			WHEN DocumentStatusId = 5 THEN 'Rejected'
			WHEN DocumentStatusId = 99 THEN 'Closed'
		END AS Status,
		dbo.tblSourcingLineItem.Weight AS NetWeight, 
		dbo.tblCommodity.Id AS CommodityId, 
		dbo.tblCommodity.Name AS Commodity, 
		dbo.tblCommodity.Code AS CommodityCode, 
		dbo.tblCommodityGrade.Id AS CommodityGradeId, 
		dbo.tblCommodityGrade.Name AS CommodityGrade
                      
FROM	dbo.tblSourcingLineItem INNER JOIN
		dbo.tblSourcingDocument ON dbo.tblSourcingLineItem.DocumentId = dbo.tblSourcingDocument.Id INNER JOIN
		dbo.tblRoutes ON dbo.tblSourcingDocument.RouteId = dbo.tblRoutes.RouteID INNER JOIN
		dbo.tblCentre ON dbo.tblSourcingDocument.CentreId = dbo.tblCentre.Id INNER JOIN
		dbo.tblCommodityOwner ON dbo.tblSourcingDocument.CommodityOwnerId = dbo.tblCommodityOwner.Id INNER JOIN
		dbo.tblCostCentre ON dbo.tblSourcingDocument.DocumentRecipientCostCentreId = dbo.tblCostCentre.Id or dbo.tblSourcingDocument.DocumentRecipientCostCentreId = dbo.tblCostCentre.Id  INNER JOIN
		dbo.tblCommodity ON dbo.tblSourcingLineItem.CommodityId = dbo.tblCommodity.Id INNER JOIN
		dbo.tblCommodityGrade ON dbo.tblSourcingLineItem.GradeId = dbo.tblCommodityGrade.Id

WHERE	(dbo.tblSourcingDocument.DocumentTypeId = 13) AND (dbo.tblCostCentre.CostCentreType = 8)
		AND(CONVERT(VARCHAR(26), dbo.tblSourcingDocument.DocumentDate,23)  BETWEEN @startDate AND @endDate) 
		AND(CONVERT(NVARCHAR(50),dbo.tblCostCentre.Id) LIKE ISNULL(@hubId, N'%'))
		AND(CONVERT(NVARCHAR(50), dbo.tblCommodityOwner.Id) LIKE ISNULL(@farmerId, N'%'))
		AND(CONVERT(NVARCHAR(50), dbo.tblCommodity.Id) LIKE ISNULL(@CommodityId, N'%'))
		--AND (CONVERT(NVARCHAR(50),DATEDIFF(DAY,dbo.tblSourcingDocument.DocumentDate,dbo.tblSourcingDocument.DateSent))LIKE ISNULL(@variance, N'%'))
		AND ((@variance is not null and DATEDIFF(DAY,dbo.tblSourcingDocument.DocumentDate,dbo.tblSourcingDocument.DateSent) = @variance) OR
		(@variance is null and DATEDIFF(DAY,dbo.tblSourcingDocument.DocumentDate,dbo.tblSourcingDocument.DateSent)>= 0))

ORDER BY dbo.tblSourcingDocument.DateIssued DESC

--  EXEC [dbo].[SP_A_CommodityPurchaseException] @startDate='2015-01-01', @endDate='2015-06-26',@variance='',@HubId='ALL',@FarmerId='ALL',@CommodityId='ALL'


