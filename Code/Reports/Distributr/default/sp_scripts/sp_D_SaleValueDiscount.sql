
DROP PROCEDURE [dbo].[sp_D_SaleValueDiscount]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[sp_D_SaleValueDiscount]
(
@startDate as datetime,
@endDate as datetime,
@distributorID AS NVARCHAR(50),
@salesmanId AS NVARCHAR(50)
)
as 
if  RTRIM(LTRIM(@distributorID))='ALL'  begin set @distributorID='%' end
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
           AND ((OrderOrderTypeId = 1) OR(OrderOrderTypeId = 3 AND DocumentStatusId = 99))
           AND((CONVERT(NVARCHAR(50),DocumentIssuerCostCentreId) LIKE ISNULL(@distributorID,'%') and CONVERT(NVARCHAR(50),DocumentRecipientCostCentre) LIKE ISNULL(@salesmanId,'%')) 
            or (CONVERT(NVARCHAR(50),DocumentIssuerCostCentreId) LIKE ISNULL(@salesmanId,'%') and CONVERT(NVARCHAR(50),DocumentRecipientCostCentre) LIKE ISNULL(@distributorID,'%'))) 
           AND (CONVERT(NVARCHAR(26),DocumentDateIssued ,23)) between @startDate and @endDate
           
 --  Exec [dbo].[sp_D_SaleValueDiscount] @startDate ='1-May-2014',@endDate ='8-Jan-2015',@distributorID = 'ALL',@salesmanId = 'ALL'
 
 
 GO