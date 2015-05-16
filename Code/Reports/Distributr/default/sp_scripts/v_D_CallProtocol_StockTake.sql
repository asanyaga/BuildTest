DROP VIEW [dbo].[v_D_CallProtocol_StockTake]
GO
CREATE VIEW [dbo].[v_D_CallProtocol_StockTake]
AS
SELECT  st.id stDocId,
        st.DocumentTypeId AS stDocType, 
        st.DocumentReference AS stDocRef, 
        st.OrderOrderTypeId AS stOrdOrdType, 
        st.DocumentDateIssued AS stDate, 
        stItems.Quantity AS stQty, 
        st.VisitId AS stVisitId, 
        prod.Description AS stProduct, 
        prod.id AS stProductId, 
        dbo.tblProductBrand.id AS stProductBrandId, 
        dbo.tblProductBrand.name AS stProductBrandName
FROM    dbo.tblDocument AS st 
 JOIN   dbo.tblLineItems stItems ON st.Id = stItems.DocumentID 
 JOIN   dbo.tblProduct prod ON stItems.ProductID = prod.id 
 JOIN   dbo.tblProductBrand ON prod.BrandId = dbo.tblProductBrand.id
WHERE  (st.DocumentTypeId = 9) 
   AND (st.OrderOrderTypeId = 6)