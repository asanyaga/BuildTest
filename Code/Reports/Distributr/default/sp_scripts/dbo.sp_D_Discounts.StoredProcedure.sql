/****** Object:  StoredProcedure [dbo].[sp_D_Discounts]    Script Date: 07/24/2013 08:51:32 ******/
DROP PROCEDURE [dbo].[sp_D_Discounts]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_D_Discounts]
(
@productId AS NVARCHAR(50),
@distributorId AS NVARCHAR(50),
@discountType AS INT,
@startDate AS DATE,
@endDate AS DATE
)
AS 
if  rtrim(ltrim(@distributorId))='ALL'  begin set @distributorId='%' end
if  rtrim(ltrim(@productId))='ALL'  begin set @productId='%' end


SELECT     dbo.tblDocument.Id AS DocumentId, 
           dbo.tblDocument.DocumentTypeId, 
           dbo.tblDocument.DocumentReference, 
           dbo.tblDocument.OrderOrderTypeId, 
           dbo.tblDocument.DocumentStatusId, 
           dbo.tblLineItems.Description, 
           dbo.tblLineItems.Quantity, 
           dbo.tblLineItems.Value, 
           dbo.tblLineItems.Vat, 
           dbo.tblLineItems.DiscountLineItemTypeId, 
           dbo.tblCostCentre.Id AS DistributorId, 
           dbo.tblCostCentre.Name AS DistributorName, 
           dbo.tblCostCentre.CostCentreType AS DistributorCCtype, 
           dbo.tblProduct.id AS ProductId, 
           dbo.tblProduct.Description AS ProductName

FROM       dbo.tblDocument INNER JOIN
           dbo.tblLineItems ON dbo.tblDocument.Id = dbo.tblLineItems.DocumentID INNER JOIN
           dbo.tblProduct ON dbo.tblLineItems.ProductID = dbo.tblProduct.id INNER JOIN
           dbo.tblCostCentre ON ((dbo.tblDocument.DocumentIssuerCostCentreId = dbo.tblCostCentre.Id) OR (dbo.tblDocument.DocumentIssuerCostCentreId = dbo.tblCostCentre.Id))

WHERE     (dbo.tblDocument.Id = dbo.tblDocument.OrderParentId) AND (dbo.tblCostCentre.CostCentreType = 2)AND
         ((dbo.tblDocument.DocumentTypeId = 1) AND 
          (dbo.tblDocument.OrderOrderTypeId = 1)OR
         ((dbo.tblDocument.OrderOrderTypeId = 3) AND 
          (dbo.tblDocument.DocumentStatusId = 99)))AND
          (dbo.tblLineItems.DiscountLineItemTypeId = @discountType)AND
          --          (dbo.tblLineItems.DiscountLineItemTypeId = @discountType or dbo.tblLineItems.DiscountLineItemTypeId = 1)AND

          (CONVERT(NVARCHAR(50),dbo.tblCostCentre.Id) LIKE ISNULL(@distributorId, N'%'))AND
          (CONVERT(NVARCHAR(50),dbo.tblProduct.id) LIKE ISNULL(@productId, N'%')) AND
          (convert(nvarchar(50),DocumentDateIssued) BETWEEN @startDate AND @endDate)
          
 -- Exec    [dbo].[sp_D_Discounts] @distributorId='ALL',@productId='ALL' ,@discountType = 5,@startDate='22-May-2014',@endDate='22-May-2014'
    
GO
