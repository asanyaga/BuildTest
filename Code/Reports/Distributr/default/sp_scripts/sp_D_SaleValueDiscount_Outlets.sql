
DROP PROCEDURE [dbo].[sp_D_SaleValueDiscount_Outlets]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[sp_D_SaleValueDiscount_Outlets]
(
@startDate as datetime,
@endDate as datetime,
@distributorID AS NVARCHAR(50),
@salesManID AS NVARCHAR(50),
@outletId AS NVARCHAR(50)

)
as 
if  RTRIM(LTRIM(@distributorID))='ALL'  begin set @distributorID='%' end
if  RTRIM(LTRIM(@salesManID))='ALL'  begin set @salesManID='%' end
if  RTRIM(LTRIM(@outletId))='ALL'  begin set @outletId='%' end

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
           and CONVERT(nvarchar(50),OrderIssuedOnBehalfOfCC) like ISNULL(@outletId,'%')
           AND((CONVERT(NVARCHAR(50),DocumentIssuerCostCentreId) LIKE ISNULL(@distributorID,'%') and CONVERT(NVARCHAR(50),DocumentRecipientCostCentre) LIKE ISNULL(@salesManID,'%')) 
            or (CONVERT(NVARCHAR(50),DocumentIssuerCostCentreId) LIKE ISNULL(@salesManID,'%') and CONVERT(NVARCHAR(50),DocumentRecipientCostCentre) LIKE ISNULL(@distributorID,'%'))) 
           AND (CONVERT(NVARCHAR(26),DocumentDateIssued ,23)) between @startDate and @endDate
           
 --  Exec [dbo].[sp_D_SaleValueDiscount_Outlets] @startDate ='1-May-2014',@endDate ='15-May-2014',@distributorID = 'ALL',@salesManID = 'ALL' ,@outletId = 'ALL'
 
 
 GO