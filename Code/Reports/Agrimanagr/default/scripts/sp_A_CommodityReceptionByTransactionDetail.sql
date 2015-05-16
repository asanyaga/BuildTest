IF NOT EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'sp_A_CommodityReceptionByTransactionDetail')
   exec('CREATE PROCEDURE [sp_A_CommodityReceptionByTransactionDetail] AS BEGIN SET NOCOUNT ON; END')
GO
Alter PROCEDURE sp_A_CommodityReceptionByTransactionDetail
   
    @documentId varchar(50)
AS
BEGIN    
    		
	select convert(varchar(50),doc.id) as documentId, 
	doc.documentReference,	
	doc.documentDate,
	cc.name as hub,
	(select name from tblRoutes where routeid =doc.routeid) as RouteName,
	(select name from tblCentre where id =doc.centreid) as CentreName,
	(select name from tblCommodity where id =item.CommodityId) as Commodity,
	(select name from tblCommodityGrade where id =item.GradeId) as Grade,
	item.Weight as TotalWeight
	from tblcostcentre cc 
	left JOIN tblSourcingDocument  doc ON cc.id= doc.DocumentRecipientCostCentreId or cc.id =doc.[DocumentIssuerCostCentreId]
	left JOIN tblSourcingLineItem  item ON doc.Id = item.DocumentId 
	where cc.costcentretype= 8 and doc.DocumentTypeId = 17 
	and  doc.id=@documentId		
	
END;
--EXEC sp_A_CommodityReceptionByTransactionDetail @documentId='4B2793FC-8961-42A0-9AB3-13AD371EE64F'
--EXEC sp_A_CommodityReceptionByTransactionDetail @startDate = '10-Jan-2013', @endDate = '10-Dec-2013',@routeid='    '
--EXEC sp_A_CommodityReceptionByTransactionDetail @startDate = '     ', @endDate = '',@routeid='    '
--EXEC sp_A_CommodityReceptionByTransactionDetail @startDate = '10-Jan-2013', @endDate = '10-Dec-2013' ,@routeid='9E67C9B7-0660-406D-B17D-9E227C58F424'
--EXEC sp_A_CommodityReceptionByTransactionDetail @documentId='352FB5F2-3A94-4764-A101-2848813EBD01'