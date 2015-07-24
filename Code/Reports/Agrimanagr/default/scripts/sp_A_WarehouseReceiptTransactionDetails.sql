IF NOT EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'sp_A_WarehouseReceiptTransactionDetails')
   exec('CREATE PROCEDURE [sp_A_WarehouseReceiptTransactionDetails] AS BEGIN SET NOCOUNT ON; END')
GO
Alter PROCEDURE sp_A_WarehouseReceiptTransactionDetails
(
@DocumentId AS NVARCHAR(50)
)
AS

BEGIN    

SELECT DISTINCT CONVERT(NVARCHAR(26),doc.DocumentDate,23) AS DepositDate,
	doc.DocumentReference,
	clerk.Name AS Clerk,
	hub.Name AS Warehouse,
	store.Name AS Store,
	tblRegion.Name AS Location, 
	Depositor.FirstName + ' '+ Depositor.Surname AS Depositor,
	tblCommodity.Name AS Commodity,
	tblCommodityGrade.Name AS Grade,
	item.Weight AS NetWeight,
	Depositor.PostalAddress AS depositorAddress,
	Depositor.PhoneNo AS DepositorPhoneNo,
	Depositor.Email AS DepsitorEmail,
	Depositor.PhysicalAddress AS DepositorLocation,
	CONVERT(NVARCHAR(26),DATEADD(MM,6,DocumentDate),23) AS ExpiryDate,
	--doc.DocumentStatusId AS ReceiptStatus,
	(CASE WHEN DocumentStatusId=1 THEN 'New' 
	WHEN DocumentStatusId=2 THEN 'Confirmed'
	WHEN DocumentStatusId=3 THEN 'Released'
	WHEN DocumentStatusId=4 THEN 'Reverted'
	WHEN DocumentStatusId=5 THEN 'Closed' END) AS ReceiptStatus,
	tblCountry.Name AS Country,
	tblUsers.Mobile


FROM tblSourcingDocument doc INNER JOIN
	tblSourcingLineItem item ON doc.Id = item.DocumentId INNER JOIN
	tblCostCentre hub ON doc.DocumentIssuerCostCentreId = hub.Id OR doc.DocumentRecipientCostCentreId = hub.Id INNER JOIN
	tblCostCentre AS clerk ON hub.Id = clerk.ParentCostCentreId INNER JOIN
	tblCostCentre AS store ON hub.Id = store.ParentCostCentreId INNER JOIN
	tblCommodity ON item.CommodityId = tblCommodity.Id INNER JOIN
	tblCommodityGrade ON item.GradeId = tblCommodityGrade.Id INNER JOIN
	tblRegion ON hub.Distributor_RegionId = tblRegion.id INNER JOIN
	tblCountry ON tblRegion.Country = tblCountry.id INNER JOIN
	tblCommodityOwner Depositor ON doc.CommodityProducerId = Depositor.CostCentreId INNER JOIN
	tblUsers ON clerk.Id=tblUsers.CostCenterId

WHERE (doc.DocumentTypeId = 27) 
	AND (hub.CostCentreType = 8) 
	AND (clerk.CostCentreType = 10) 
	AND (store.CostCentreType = 11)
	AND(CONVERT(NVARCHAR(50),doc.Id) LIKE ISNULL(@DocumentId, N'%'))

	
END;

-- EXEC sp_A_WarehouseReceiptTransactionDetails @DocumentId = '394866F3-5CF2-47B5-9533-DC4A582E34F7'
