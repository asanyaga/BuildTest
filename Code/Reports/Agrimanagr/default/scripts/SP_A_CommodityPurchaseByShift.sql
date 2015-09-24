IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SP_A_CommodityPurchaseByShift]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[SP_A_CommodityPurchaseByShift]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SP_A_CommodityPurchaseByShift]

(
@startDate AS DATE,
@endDate AS DATE,
@hubId AS NVARCHAR(50),
@routeId AS NVARCHAR(50),
@centreId AS NVARCHAR(50),
@farmerId AS NVARCHAR(50)

)

AS

if  @hubId='ALL'  begin set @hubId='%' end
if  @routeId='ALL'  begin set @routeId='%' end
if  @centreId='ALL'  begin set @centreId='%' end
if  @farmerId='ALL'  begin set @farmerId='%' end


;With Shift_CTE AS (
SELECT	TOP 100 PERCENT		  dbo.tblCostCentre.Id AS HubId,
                      dbo.tblRoutes.RouteID AS RouteId,
                      dbo.tblRoutes.Name AS [Route],
					  dbo.tblCentre.Name AS BuyingCentre,
                      dbo.tblCommodityOwner.Id AS FarmerId,
                      dbo.tblCommodityOwner.Surname,
                      dbo.tblCommodityOwner.FirstName,
                      dbo.tblCommodityOwner.Code AS FarmerCode,
                      convert(varchar,dbo.tblSourcingLineItem.[Weight]) [Weight],
					  REPLACE(CONVERT(CHAR(10),tblSourcingDocument.DocumentDate, 103), '/', '') as [Date],
					  REPLACE(RIGHT(CONVERT(VARCHAR,tblSourcingDocument.DocumentDate,100),7),':', '') as [Time]

                      
FROM         dbo.tblSourcingLineItem INNER JOIN
                      dbo.tblSourcingDocument ON dbo.tblSourcingLineItem.DocumentId = dbo.tblSourcingDocument.Id INNER JOIN
                      dbo.tblRoutes ON dbo.tblSourcingDocument.RouteId = dbo.tblRoutes.RouteID INNER JOIN
                      dbo.tblCentre ON dbo.tblSourcingDocument.CentreId = dbo.tblCentre.Id INNER JOIN
                      dbo.tblCommodityOwner ON dbo.tblSourcingDocument.CommodityOwnerId = dbo.tblCommodityOwner.Id INNER JOIN
                      dbo.tblCostCentre ON dbo.tblSourcingDocument.DocumentRecipientCostCentreId = dbo.tblCostCentre.Id or dbo.tblSourcingDocument.DocumentIssuerCostCentreId = dbo.tblCostCentre.Id
WHERE     (dbo.tblSourcingDocument.DocumentTypeId = 13) AND (dbo.tblCostCentre.CostCentreType = 8)
           AND(CONVERT(VARCHAR(26), dbo.tblSourcingDocument.DocumentDate,23)  BETWEEN @startDate AND @endDate)   
           AND(CONVERT(NVARCHAR(50),dbo.tblCostCentre.Id) LIKE ISNULL(@hubId, N'%'))             
           AND(CONVERT(NVARCHAR(50),dbo.tblRoutes.RouteID) LIKE ISNULL(@routeId, N'%'))  
           AND(CONVERT(NVARCHAR(50),dbo.tblCentre.Id) LIKE ISNULL(@centreId, N'%'))
           AND(CONVERT(NVARCHAR(50), dbo.tblCommodityOwner.Id) LIKE ISNULL(@farmerId, N'%'))
		   
ORDER BY dbo.tblSourcingDocument.DocumentDate DESC
)
SELECT (FarmerCode+'   '+[Date]+''+[Route]+' '+BuyingCentre+'      '+[Weight]+'   '+[Time])AS ReportData FROM Shift_CTE

--  exec [SP_A_CommodityPurchaseByShift] @startDate='2015-06-01', @endDate='2015-06-26', @hubId='ALL', @routeId='ALL', @centreId='ALL', @farmerId='ALL'

