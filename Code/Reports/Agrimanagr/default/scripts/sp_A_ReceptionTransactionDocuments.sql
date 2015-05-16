IF NOT EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'sp_A_ReceptionTransactionDocuments')
   exec('CREATE PROCEDURE [sp_A_ReceptionTransactionDocuments] AS BEGIN SET NOCOUNT ON; END')
GO
Alter PROCEDURE sp_A_ReceptionTransactionDocuments
	 @routeId varchar(50)=NULL ,
	 @hubId varchar(50)=NULL    
AS
BEGIN
	PRINT 'HubId = '+isnull(@hubId,'NULL');
	PRINT 'RouteId = '+isnull(@routeId,'NULL');
    if(@routeId='' or @routeId='ALL') set @routeId=null;
    if(@hubId='' or @hubId='ALL') set @hubId=null;
	
	SELECT TOP(1)'ALL' AS HubId, 
             'ALL' AS DocRef, 
             'ALL' AS HubName
FROM       dbo.tblSourcingDocument as doc INNER JOIN
           dbo.tblCostCentre as cc ON doc.DocumentRecipientCostCentreId = cc.Id
           where (doc.DocumentTypeId=17)

UNION ALL


SELECT     CONVERT(NVARCHAR(50),cc.Id) AS HubId, 
           doc.DocumentReference AS DocRef, 
           cc.Name AS HubName
FROM       dbo.tblSourcingDocument as doc INNER JOIN
           dbo.tblCostCentre as cc ON doc.DocumentRecipientCostCentreId = cc.Id
           where (doc.DocumentTypeId=17)
	and(@routeId is null or doc.routeId=@routeId)
	and(@hubId is null or cc.Id=@hubId)
	
	
END;

--EXEC sp_A_ReceptionTransactionDocuments @routeid='ALL', @hubId='ALL'
--EXEC sp_A_ReceptionTransactionDocuments @routeid='ALL', @hubId='E6834108-4BA7-4A1A-98CA-9DD4DF8D68E3'