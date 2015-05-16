
DROP PROCEDURE [dbo].[sp_A_GainsAndLossesPerFieldClerk_Revised]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[sp_A_GainsAndLossesPerFieldClerk_Revised]
(

@startDate as datetime,
@endDate as datetime,
@routeId as nvarchar(50),
@hubId as nvarchar(50),
@clerkId as nvarchar(50)
)
as 
if  @routeId='ALL'  begin set @routeId='%' end
if  @hubId='ALL'  begin set @hubId='%' end
if  @clerkId='ALL'  begin set @clerkId='%' end


SELECT distinct case when purchase.DocumentTypeId = 13 then sum(purchaseItem.Weight) else 0 end as ReceiptWeight,
                case when purchase.DocumentTypeId = 16 then sum(purchaseItem.Weight) else 0 end as LorryWeight,
                case when purchase.DocumentTypeId = 15 then sum(purchaseItem.Weight) else 0 end as FactoryWeight,                
                case when purchase.RouteId is null OR [route].RouteID is null    then CAST(CAST(0 AS BINARY) AS UNIQUEIDENTIFIER) else [route].RouteID end  as RouteId,
                case when purchase.RouteId is null OR [route].RouteID is null   then 'No route' else [route].name end  as RouteName,
                             hub.Name,
                             hub.Id, 
                             clerk.UserName as ClerkName, 
                             clerk.Id as ClerkId,
                             hub.Cost_Centre_Code as ClerkCode,
                             convert(nvarchar(26),purchase.DocumentDate,23) as PurchaseDate                     
                   
FROM                     dbo.tblSourcingDocument purchase
              INNER JOIN dbo.tblSourcingLineItem purchaseItem ON purchase.Id = purchaseItem.DocumentId 
              INNER JOIN dbo.tblCostCentre hub ON purchase.DocumentRecipientCostCentreId = hub.Id 
              INNER JOIN dbo.tblUsers clerk ON purchase.DocumentIssuerUserId = clerk.Id
              LEFT OUTER JOIN  dbo.tblRoutes [route] ON purchase.RouteId = [route].RouteID or purchase.RouteId is null

WHERE    
convert(nvarchar(26),purchase.DocumentDate,23) between @startDate and @endDate
and (DocumentTypeId = 13 or DocumentTypeId = 15 or DocumentTypeId = 16)
and convert(nvarchar(50),clerk.Id) like isnull(@clerkId,'%')

group by [route].RouteID,[route].name,hub.Name,hub.Id,clerk.UserName , clerk.Id,hub.Cost_Centre_Code,purchase.DocumentDate,purchase.DocumentTypeId,purchase.RouteId
     
--  Exec [dbo].[sp_A_GainsAndLossesPerFieldClerk_Revised] @startDate = '2013-10-16',@endDate = '2013-10-16' ,@routeId ='ALL',@hubId ='ALL',@clerkId='ALL'


GO