DROP PROCEDURE [dbo].[SP_A_CommodityDeliveryException] 
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[SP_A_CommodityDeliveryException]
(
@startDate as datetime,
@endDate as datetime,
@CentreId AS NVARCHAR(50),
@DriverId AS NVARCHAR(50),
@CommodityId AS NVARCHAR(50),
@variance as Int
)
as 
if  rtrim(ltrim(@CentreId))='ALL'  begin set @CentreId='%' end
if  rtrim(ltrim(@DriverId))='ALL'  begin set @DriverId='%' end
if  rtrim(ltrim(@CommodityId))='ALL'  begin set @CommodityId='%' end
if @variance = null begin set @variance='%' end


SELECT	dbo.tblSourcingDocument.DocumentReference,
		dbo.tblCentre.Id AS CentreId,
		dbo.tblCentre.Name AS CentreName, 
		dbo.tblSourcingDocument.DriverName,
		dbo.tblUsers.Id AS DriverId,
		CONVERT(NVARCHAR(26),dbo.tblSourcingDocument.DocumentDate,23) as DeliveryDate,
		CONVERT(NVARCHAR(26),dbo.tblSourcingDocument.DateSent,23) as SentDate,
		CAST(DATEDIFF(DAY,dbo.tblSourcingDocument.DocumentDate,dbo.tblSourcingDocument.DateSent) AS NVARCHAR(50))+' Days' AS Variance,
		dbo.tblSourcingLineItem.Weight AS NetWeight, 
		dbo.tblCommodity.Id AS CommodityId, 
		dbo.tblCommodity.Name AS Commodity, 
		dbo.tblCommodity.Code AS CommodityCode

FROM	tblcostcentre 
		LEFT JOIN tblSourcingDocument ON tblcostcentre.Id = tblSourcingDocument.DocumentIssuerCostCentreId or  tblcostcentre.Id =DocumentRecipientCostCentreId
		LEFT JOIN tblSourcingLineItem ON tblSourcingDocument.Id = tblSourcingLineItem.DocumentId
		LEFT JOIN dbo.tblCommodity ON tblSourcingLineItem.CommodityId = dbo.tblCommodity.Id  
		LEFT JOIN tblUsers on tblUsers.UserName = tblSourcingDocument.drivername
		LEFT JOIN dbo.tblCentre ON dbo.tblSourcingDocument.CentreId = dbo.tblCentre.Id

WHERE	(dbo.tblSourcingDocument.DocumentTypeId = 16) AND (dbo.tblCostCentre.CostCentreType = 8)
		AND(CONVERT(VARCHAR(26), dbo.tblSourcingDocument.DocumentDate,23)  BETWEEN @startDate AND @endDate) 
		AND(CONVERT(NVARCHAR(50),dbo.tblCentre.Id) LIKE ISNULL(@CentreId, N'%'))
		AND(CONVERT(NVARCHAR(50), dbo.tblUsers.Id) LIKE ISNULL(@DriverId, N'%'))
		AND(CONVERT(NVARCHAR(50), dbo.tblCommodity.Id) LIKE ISNULL(@CommodityId, N'%'))
		AND ((@variance is not null and DATEDIFF(DAY,dbo.tblSourcingDocument.DocumentDate,dbo.tblSourcingDocument.DateSent) = @variance) OR
		(@variance is null and DATEDIFF(DAY,dbo.tblSourcingDocument.DocumentDate,dbo.tblSourcingDocument.DateSent)>= 0))

ORDER BY dbo.tblSourcingDocument.DateIssued DESC

--  EXEC [dbo].[SP_A_CommodityDeliveryException] @startDate='2015-01-01', @endDate='2015-06-26',@variance='',@CentreId='ALL',@DriverId='ALL',@CommodityId='ALL'
