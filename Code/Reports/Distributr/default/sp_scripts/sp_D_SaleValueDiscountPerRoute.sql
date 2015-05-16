DROP PROCEDURE [dbo].[sp_D_SaleValueDiscountPerRoute]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[sp_D_SaleValueDiscountPerRoute]
(
@startDate as datetime,
@endDate as datetime,
@distributorID AS NVARCHAR(50),
@salesManID AS NVARCHAR(50),
@outletId AS NVARCHAR(50),
@RouteID as nvarchar(50),
@docId as Nvarchar(50)
)
as 

if  RTRIM(LTRIM(@distributorID))='ALL'  begin set @distributorID='%' end
if  RTRIM(LTRIM(@salesManID))='ALL'  begin set @salesManID='%' end
if  RTRIM(LTRIM(@outletId))='ALL'  begin set @outletId='%' end
if  RTRIM(LTRIM(@RouteID))='ALL'  begin set @RouteID='%' end
if  RTRIM(LTRIM(@docId))='ALL' begin set @docId='%' end

SELECT     sale.Id as DocID, 
           sale.DocumentTypeId as DocTypeId, 
           sale.OrderOrderTypeId as OOtypeId,            
           sale.DocumentStatusId as DocStatId, 
           sale.SaleDiscount AS SaleValueDiscounts, 
           sale.DocumentDateIssued as DocDateIss,
           sale.DocumentReference as DocRef,
           sale.DocumentRecipientCostCentre,
           sale.DocumentIssuerCostCentreId,
           outlet.RouteId
          
FROM       dbo.tblDocument sale
inner join dbo.tblCostCentre outlet on sale.OrderIssuedOnBehalfOfCC =  outlet.Id

WHERE     (DocumentTypeId = 1) 
           AND ((OrderOrderTypeId = 1) OR(OrderOrderTypeId = 3 AND DocumentStatusId = 99))
           and convert(nvarchar(50),outlet.RouteId) like ISNULL(@RouteID,'%')
           and(convert(nvarchar(50),OrderIssuedOnBehalfOfCC) like ISNULL(@outletId,'%'))
           and(convert(nvarchar(50),sale.Id) like ISNULL(@docId,'%'))
           AND((CONVERT(NVARCHAR(50),sale.DocumentIssuerCostCentreId) LIKE ISNULL(@distributorID,'%') and CONVERT(NVARCHAR(50),sale.DocumentRecipientCostCentre) LIKE ISNULL(@salesManID,'%')) 
            or (CONVERT(NVARCHAR(50),sale.DocumentIssuerCostCentreId) LIKE ISNULL(@salesManID,'%') and CONVERT(NVARCHAR(50),sale.DocumentRecipientCostCentre) LIKE ISNULL(@distributorID,'%'))) 
           AND (CONVERT(NVARCHAR(26),sale.DocumentDateIssued ,23)) between @startDate and @endDate
           
           
 -- EXEC   [dbo].[sp_D_SaleValueDiscountPerRoute]   @startDate='2013-11-20',@endDate='2013-11-20',@RouteID='0439A275-E9FE-44C1-BB73-703B33F953CF',@outletId='ALL',@docId='ALL',@distributorID='ALL',@salesManID='ALL'
 
 
 GO