
DROP PROCEDURE [dbo].[sp_D_SaleValueDiscount_PerOutlet]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[sp_D_SaleValueDiscount_PerOutlet]
(
@startDate as datetime,
@endDate as datetime,
@distributorId AS NVARCHAR(50),
@salesmanId AS NVARCHAR(50),
@outletId AS NVARCHAR(50)

)
as 
if  rtrim(ltrim(@distributorId))='ALL'  begin set @distributorId='%' end
if  rtrim(ltrim(@salesmanId))='ALL'  begin set @salesmanId='%' end
if  rtrim(ltrim(@outletId))='ALL'  begin set @outletId='%' end

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
           AND((CONVERT(NVARCHAR(50),DocumentIssuerCostCentreId) LIKE ISNULL(@distributorId,'%') and CONVERT(NVARCHAR(50),DocumentRecipientCostCentre) LIKE ISNULL(@salesmanId,'%')) 
            or (CONVERT(NVARCHAR(50),DocumentIssuerCostCentreId) LIKE ISNULL(@salesmanId,'%') and CONVERT(NVARCHAR(50),DocumentRecipientCostCentre) LIKE ISNULL(@distributorId,'%'))) 
           AND (CONVERT(NVARCHAR(26),DocumentDateIssued ,23)) between @startDate and @endDate
           
 --  Exec [dbo].[sp_D_SaleValueDiscount_PerOutlet] @startDate ='29-Oct-2013',@endDate ='29-Oct-2013',@distributorId = 'ALL',@salesmanId = 'ALL' ,@outletId = 'ALL'
 
 
 GO