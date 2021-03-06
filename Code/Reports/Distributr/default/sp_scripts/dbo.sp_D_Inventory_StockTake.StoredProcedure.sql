/****** Object:  StoredProcedure [dbo].[sp_D_Inventory_StockTake]    Script Date: 07/24/2013 08:51:32 ******/
DROP PROCEDURE [dbo].[sp_D_Inventory_StockTake]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_D_Inventory_StockTake]
(
@startDate AS DATE,
@endDate AS DATE,
@brandId AS NVARCHAR(50),
@productId AS NVARCHAR(50),
@packagingId AS NVARCHAR(50),
@flavourId AS NVARCHAR(50),
@distributorId AS NVARCHAR(50)
)
AS 
if  @brandId='ALL'  begin set @brandId='%' end
if  @productId='ALL'  begin set @productId='%' end
if  @packagingId='ALL'  begin set @packagingId='%' end
if  @flavourId='ALL'  begin set @flavourId='%' end
if  @distributorId='ALL'  begin set @distributorId='%' end

SELECT     dbo.tblDocument.Id AS DocumentId, 
           dbo.tblDocument.DocumentReference, 
           dbo.tblDocument.DocumentTypeId, 
           dbo.tblDocument.OrderOrderTypeId, 
           dbo.tblCostCentre.Id AS DistributorId, 
           dbo.tblCostCentre.Name AS DistributorName, 
           dbo.tblCostCentre.CostCentreType AS DistributorCCtype, 
           dbo.tblProduct.id AS ProductId, 
           dbo.tblProduct.Description AS ProductName, 
           dbo.tblDocument.DocumentDateIssued, 
           dbo.tblLineItems.Description AS Reasons, 
           dbo.tblLineItems.IAN_Actual AS ActualQty, 
           dbo.tblLineItems.Quantity AS ExpectedQty

FROM       dbo.tblDocument INNER JOIN
           dbo.tblCostCentre ON dbo.tblDocument.DocumentIssuerCostCentreId = dbo.tblCostCentre.Id INNER JOIN
           dbo.tblLineItems ON dbo.tblDocument.Id = dbo.tblLineItems.DocumentID INNER JOIN
           dbo.tblProduct ON dbo.tblLineItems.ProductID = dbo.tblProduct.id

WHERE     (dbo.tblDocument.DocumentTypeId = 9) AND 
          (dbo.tblDocument.OrderOrderTypeId = 4)AND
          (convert(nvarchar(50),dbo.tblDocument.DocumentDateIssued) BETWEEN @startDate AND @endDate)AND
          (CONVERT(NVARCHAR(50),dbo.tblProduct.id) LIKE ISNULL(@productId,N'%'))AND
          (CONVERT(NVARCHAR(50),dbo.tblCostCentre.Id) LIKE ISNULL(@distributorId,N'%'))
GO
