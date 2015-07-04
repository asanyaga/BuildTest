IF NOT EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'sp_A_AFarm')
   exec('CREATE PROCEDURE [sp_A_AFarm] AS BEGIN SET NOCOUNT ON; END')
GO
Alter PROCEDURE sp_A_AFarm   
@FarmerId AS NVARCHAR(50)
AS

if  @FarmerId='ALL'  begin set @FarmerId='%' end

SELECT  'ALL' AS Id, 'ALL' AS Name
UNION
SELECT DISTINCT CONVERT(NVARCHAR(50),tblCommodityProducer.Id),tblCommodityProducer.Name
FROM tblActivityDocument
	INNER JOIN dbo.tblCostCentre ON dbo.tblCostCentre.ParentCostCentreId = dbo.tblActivityDocument.hubId
	INNER JOIN dbo.tblCommodityOwner ON dbo.tblCostCentre.Id = dbo.tblCommodityOwner.CostCentreId
	INNER JOIN dbo.tblCommodityProducer ON dbo.tblActivityDocument.CommodityProducerId = dbo.tblCommodityProducer.Id
WHERE (CONVERT(NVARCHAR(50),dbo.tblCommodityOwner.Id) LIKE ISNULL(@FarmerId, N'%'))
ORDER BY Name

-- EXEC sp_A_AFarm @FarmerId='ALL'
