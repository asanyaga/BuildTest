DROP PROCEDURE [dbo].[sp_A_DB_CommodityCenterPurchaseWeightAnalysis]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[sp_A_DB_CommodityCenterPurchaseWeightAnalysis]
(

@startDate as datetime,
@endDate as datetime,
@regionId as nvarchar(50)
)
as 
if  @regionId='ALL'  begin set @regionId='%' end

  
  SELECT DISTINCT 
          reg.Name as RegionName,
          item.Weight AS PurchaseWeight


   FROM tblcostcentre cc 
   LEFT JOIN tblSourcingDocument  doc ON (cc.Id = doc.DocumentISsuerCostCentreId OR  cc.Id = DocumentRecipientCostCentreId)
   LEFT JOIN tblSourcingLineItem  item ON doc.Id = item.DocumentId 
   LEFT JOIN tblroutes rt ON cc.RouteId = rt.RouteID 
   INNER JOIN tblRegion reg ON cc.Distributor_RegionId = reg.id

   WHERE cc.costcentretype= 8 
   AND doc.DocumentTypeId = 13
   AND convert(nvarchar(26),doc.DocumentDate,23) between @startDate and @endDate 
   
   
 -- Exec [dbo].[sp_A_DB_CommodityCenterPurchaseWeightAnalysis] @startDate='2013-10-16',@endDate='2013-10-16',@regionId='ALL'
 
 
 GO