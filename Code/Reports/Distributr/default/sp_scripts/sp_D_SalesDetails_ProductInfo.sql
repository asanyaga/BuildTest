/****** Object:  StoredProcedure [dbo].[sp_D_SalesDetails_ProductInfo]    Script Date: 07/24/2013 08:51:32 ******/
DROP PROCEDURE [dbo].[sp_D_SalesDetails_ProductInfo]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_D_SalesDetails_ProductInfo]
(@docId AS NVARCHAR(50))
AS
if  @docId='ALL'  begin set @docId='%' end

SELECT    
            dbo.tblDocument.DocumentReference, 
            dbo.tblDocument.Id   , 
            dbo.tblProduct.id AS ProductId, 
            dbo.tblProduct.Description AS ProductName, 
            dbo.tblDocument.DocumentTypeId,            
            dbo.tblDocument.OrderOrderTypeId, 
            dbo.tblLineItems.OrderLineItemType, 
            dbo.tblProduct.Returnable, 
            dbo.tblProduct.ReturnableType, 
            dbo.tblProduct.DomainTypeId, 
            CASE WHEN DomainTypeId = 1 THEN 'Sale' 
                 WHEN DomainTypeId = 2 THEN 'Consolidated' 
                 WHEN DomainTypeId = 3 THEN 'Returnable' 
                 WHEN DomainTypeId = 7 THEN 'Discount' END AS ProductType, 
            dbo.tblLineItems.Quantity, 
            dbo.tblLineItems.Value,
            dbo.tblLineItems.Vat, 
            dbo.tblDocument.SaleDiscount, 
            dbo.tblLineItems.ProductDiscount,
            dbo.tblProduct.ProductCode AS ProductCode 
FROM        dbo.tblDocument INNER JOIN
            dbo.tblLineItems ON dbo.tblDocument.Id = dbo.tblLineItems.DocumentID INNER JOIN
            dbo.tblProduct ON dbo.tblLineItems.ProductID = dbo.tblProduct.id 
           
WHERE      (dbo.tblDocument.DocumentTypeId = 1) 
     --AND   (dbo.tblDocument.OrderOrderTypeId = 3)
     AND   (CONVERT(NVARCHAR(50), dbo.tblDocument.Id)LIKE ISNULL(@docId, N'%'))
   --  and  dbo.tblDocument.DocumentReference like     'S_monica_CKO6597_20140730_144550_00014'
 --  Exec  [dbo].[sp_D_SalesDetails_ProductInfo]     @docId='ALL'  
 --   exec [dbo].[sp_D_SalesSubReport]  @docId='8e4b3494-4509-4d72-898f-63e1342260ff'
           

GO
