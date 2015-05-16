
DROP PROCEDURE [dbo].[sp_D_OrderValueDiscount]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[sp_D_OrderValueDiscount]
(
@startDate as datetime,
@endDate as datetime,
@distributorId AS NVARCHAR(50),
@salesmanId AS NVARCHAR(50)
)
as 
if  RTRIM(LTRIM(@distributorId))='ALL'  begin set @distributorId='%' end
if  RTRIM(LTRIM(@salesmanId))='ALL'  begin set @salesmanId='%' end

SELECT     Id as DocID, 
           DocumentTypeId as DocTypeId, 
           OrderOrderTypeId as OOtypeId,            
           DocumentStatusId as DocStatId, 
           SaleDiscount AS SaleValueDiscounts, 
           DocumentDateIssued as DocDateIss,
           DocumentReference as DocRef,
           DocumentRecipientCostCentre,
           DocumentIssuerCostCentreId
FROM       dbo.tblDocument
WHERE     (DocumentTypeId = 1) 
       AND(OrderOrderTypeId = 3)
          -- AND ((OrderOrderTypeId = 1) OR(OrderOrderTypeId = 3 AND DocumentStatusId = 99))
           AND((CONVERT(NVARCHAR(50),DocumentIssuerCostCentreId) LIKE ISNULL(@distributorId,'%') and CONVERT(NVARCHAR(50),DocumentRecipientCostCentre) LIKE ISNULL(@salesmanId,'%')) 
            or (CONVERT(NVARCHAR(50),DocumentIssuerCostCentreId) LIKE ISNULL(@salesmanId,'%') and CONVERT(NVARCHAR(50),DocumentRecipientCostCentre) LIKE ISNULL(@distributorId,'%'))) 
           AND (CONVERT(NVARCHAR(26),DocumentDateIssued ,23)) between @startDate and @endDate
           
--  Exec [dbo].[sp_D_OrderValueDiscount] @startDate ='1-May-2014',@endDate ='19-May-2014',@distributorId = 'ALL',@salesmanId = 'ALL'

GO