DROP PROCEDURE [dbo].[sp_A_DB_CommodityCenterWeightAnalysisPerHub]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[sp_A_DB_CommodityCenterWeightAnalysisPerHub]
(

@startDate as datetime,
@endDate as datetime,
@regionId as nvarchar(50)
)
as 
if  @regionId='ALL'  begin set @regionId='%' end

  
  SELECT DISTINCT 
          reg.Name as RegionName,
          CASE WHEN doc.DocumentTypeId = 13  THEN item.Weight ELSE 0 END AS PurchaseWeight,
          CASE WHEN doc.DocumentTypeId = 16  THEN item.Weight ELSE 0 END AS DeliveredWeight,
          CASE WHEN doc.DocumentTypeId = 15 or doc.DocumentTypeId = 17  THEN item.Weight ELSE 0 END AS ReceivedAndStoredWeight


   FROM tblcostcentre cc 
   LEFT JOIN tblSourcingDocument  doc ON (cc.Id = doc.DocumentISsuerCostCentreId OR  cc.Id = DocumentRecipientCostCentreId)
   LEFT JOIN tblSourcingLineItem  item ON doc.Id = item.DocumentId 
   LEFT JOIN tblroutes rt ON cc.RouteId = rt.RouteID 
   INNER JOIN tblRegion reg ON cc.Distributor_RegionId = reg.id

   WHERE cc.costcentretype= 8 
   AND( doc.DocumentTypeId = 13 OR doc.DocumentTypeId = 15 OR doc.DocumentTypeId = 16 OR doc.DocumentTypeId = 17)
   AND convert(nvarchar(26),doc.DocumentDate,23) between @startDate and @endDate  
   
   
 -- Exec [dbo].[sp_A_DB_CommodityCenterWeightAnalysisPerHub] @startDate='2013-10-16',@endDate='2013-10-16',@regionId='ALL'
 
 
 GO