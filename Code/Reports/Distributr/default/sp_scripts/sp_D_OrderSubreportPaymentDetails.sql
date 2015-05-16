DROP PROCEDURE [dbo].[sp_D_OrderSubreportPaymentDetails]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create PROCEDURE [dbo].[sp_D_OrderSubreportPaymentDetails] 
(@documentId AS NVARCHAR(50))

as

if  @documentId='ALL'  begin set @documentId='%' end

SELECT dbo.tblCostCentre.Name,
       dbo.tblCostCentre.CostCentreType,
       dbo.tblCostCentre.ParentCostCentreId,
       dbo.tblDocument.DocumentTypeId, 
       dbo.tblDocument.OrderOrderTypeId,
       tblDocument_1.DocumentTypeId AS Expr1,
       tblDocument_2.DocumentTypeId AS Expr3,
       dbo.tblLineItems.Value, 
       dbo.tblLineItems.ProductDiscount, 
       Receipt_PaymentTypeId =
       CASE dbo.tblLineItems.Receipt_PaymentTypeId
       
         WHEN '1' THEN 'Cash'
         WHEN '2' THEN 'Cheque'
         WHEN '3' THEN 'Mmoney'
        
      END
       , dbo.tblCostCentre.Id,
        dbo.tblLineItems.Quantity,
        dbo.tblLineItems.Receipt_PaymentReference as PaymentReference,
        dbo.tblDocument.DocumentReference
        
FROM  dbo.tblCostCentre INNER JOIN
               dbo.tblDocument ON dbo.tblCostCentre.Id = dbo.tblDocument.DocumentRecipientCostCentre INNER JOIN
               dbo.tblDocument AS tblDocument_1 ON dbo.tblDocument.Id = tblDocument_1.InvoiceOrderId INNER JOIN
               dbo.tblDocument AS tblDocument_2 ON tblDocument_1.Id = tblDocument_2.InvoiceOrderId INNER JOIN
               dbo.tblLineItems ON tblDocument_2.Id = dbo.tblLineItems.DocumentID
               
WHERE (dbo.tblDocument.DocumentTypeId = 1) 
 AND (dbo.tblDocument.OrderOrderTypeId = 1) 
 AND (tblDocument_1.DocumentTypeId = 5) 
 AND (tblDocument_2.DocumentTypeId = 8) 
 AND (CONVERT(NVARCHAR(50), dbo.tblDocument.Id)LIKE ISNULL(@documentId, N'%'))
--