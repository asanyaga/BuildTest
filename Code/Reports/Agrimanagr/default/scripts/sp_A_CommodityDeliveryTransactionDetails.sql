IF NOT EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'sp_A_CommodityDeliveryByTransactionDetails')
   exec('CREATE PROCEDURE [sp_A_CommodityDeliveryByTransactionDetails] AS BEGIN SET NOCOUNT ON; END')
GO
Alter PROCEDURE sp_A_CommodityDeliveryByTransactionDetails
    @documentId varchar(50) = NULL
AS
BEGIN    
	select convert(varchar(50),doc.id) as documentId, 
	doc.documentReference,	
	doc.documentDate,
	cc.name as hub,
	(select username from tblusers where username=doc.drivername ) as DriverName,
	(select name from tblRoutes where routeid =doc.routeid) as RouteName,
	(select name from tblCentre where id =doc.centreid) as CentreName,
	(select name from tblCommodity where id =item.CommodityId) as Commodity,
	(select name from tblCommodityGrade where id =item.GradeId) as Grade,
	item.weight as Weight
	from tblcostcentre cc 
	inner JOIN tblSourcingDocument  doc ON cc.Id = doc.DocumentIssuerCostCentreId or  cc.Id =DocumentRecipientCostCentreId
	inner JOIN tblSourcingLineItem  item ON doc.Id = item.DocumentId 
	where cc.costcentretype= 8 and doc.DocumentTypeId = 16
	and  doc.id=@documentId		END;
--EXEC sp_A_CommodityDeliveryByTransactionDetails @startDate = '10-Jan-2013', @endDate = '10-Dec-2013',@routeid='    '
