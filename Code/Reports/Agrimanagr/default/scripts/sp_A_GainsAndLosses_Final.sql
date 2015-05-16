DROP PROCEDURE [dbo].[sp_A_GainsAndLosses_Final]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [sp_A_GainsAndLosses_Final]
(
@startDate DATETIME,
@endDate DATETIME,
@hubId AS NVARCHAR(50),
@routeId AS NVARCHAR(50),
@clerkId AS NVARCHAR(50)
)
AS
--if LTRIM(RTRIM(@startDate))='ALL' begin set @startDate='%' end
--if LTRIM(RTRIM(@endDate))='ALL' begin set @endDate='%' end

if LTRIM(RTRIM(@hubId))='ALL' begin set @hubId='%' end
if LTRIM(RTRIM(@routeId))='ALL' begin set @routeId='%' end
if LTRIM(RTRIM(@clerkId))='ALL' begin set @clerkId='%' end



DECLARE @FactoryIds nvarchar(50) ,
        @FactoryName nvarchar(50) ,
        @RouteIds nvarchar(50) ,
        @RouteName nvarchar(50) ,
        @ClerkIds nvarchar(50) ,
        @ClerkName nvarchar(50) ,
        @ClerkCode nvarchar(50) ,
        @ReceiptWeight decimal(18,2) ,
        @LorryWeight decimal(18,2) ,
        @FactoryWeight decimal(18,2 ),
        @PurchaseDate date ,
        @DeliveryNoteId nvarchar(50);

        
DECLARE @GainsLosses TABLE (
         FactoryIds nvarchar(50) ,
         FactoryName nvarchar(50) ,
         RouteIds nvarchar(50) ,
         RouteName nvarchar(50) ,
         ClerkIds nvarchar(50) ,
         ClerkName nvarchar(50) ,
         ClerkCode nvarchar(50) ,
         ReceiptWeight decimal(18,2) ,
         LorryWeight decimal(18,2) ,
         FactoryWeight decimal(18,2) ,
         PurchaseDate date ,
         DeliveryNoteId nvarchar(50) );
        
DECLARE  @PurchasDelivery Table(
			FactoryIds nvarchar(50),
			FactoryName nvarchar(50),
			RouteIds nvarchar(50),
			RouteName nvarchar(50),
			ClerkIds nvarchar(50),
			ClerkName nvarchar(50),
			ClerkCode nvarchar(50),
			ReceiptWeight decimal(18,2),
			LorryWeight decimal(18,2),
			PurchaseDate date,
			DeliveryNoteId nvarchar(50));
                          

-- BEGIN PN DN

INSERT   @PurchasDelivery
SELECT DISTINCT hub.Id FactoryIds,
                hub.Name FactoryName,
                case when purchase.RouteId is null OR [route].RouteID is null    then CAST(CAST(0 AS BINARY) AS UNIQUEIDENTIFIER) else [route].RouteID end  as RouteIds,
                case when purchase.RouteId is null OR [route].RouteID is null   then 'No route' else [route].name end  as RouteName, 
                clerk.Id as ClerkIds,
                clerk.UserName as ClerkName, 
                hub.Cost_Centre_Code as ClerkCode,
                case when purchase.DocumentTypeId = 13 then sum(purchaseItem.Weight) else 0 end as ReceiptWeight,--Purchase
                case when purchase.DocumentTypeId = 16 then sum(purchaseItem.Weight) else 0 end as LorryWeight,--Delivery
                --convert(nvarchar(26),purchase.DocumentDate,23) as PurchaseDate  ,
                purchase.DocumentDate,
                purchase.Id                   
                   
FROM                     dbo.tblSourcingDocument purchase
              INNER JOIN dbo.tblSourcingLineItem purchaseItem ON purchase.Id = purchaseItem.DocumentId 
              INNER JOIN dbo.tblCostCentre hub ON purchase.DocumentRecipientCostCentreId = hub.Id 
              INNER JOIN dbo.tblUsers clerk ON purchase.DocumentIssuerUserId = clerk.Id
              LEFT OUTER JOIN  dbo.tblRoutes [route] ON purchase.RouteId = [route].RouteID or purchase.RouteId is null

WHERE --convert(nvarchar(26),purchase.DocumentDate,23) between @startDate and  @endDate
 (DocumentTypeId = 13 or DocumentTypeId = 16)
GROUP BY [route].RouteID,[route].name,hub.Name,hub.Id,clerk.UserName, purchase.Id  , clerk.Id,hub.Cost_Centre_Code,purchase.DocumentDate,purchase.DocumentTypeId,purchase.RouteId
 
 DECLARE gainslosses_cursor CURSOR FOR 

SELECT     FactoryIds ,
           FactoryName , 
           RouteIds ,
           RouteName ,
           ClerkIds ,
           ClerkName ,
           ClerkCode ,
           sum(ReceiptWeight) ReceiptWeight,
           sum(LorryWeight) LorryWeight,
           PurchaseDate,
           DeliveryNoteId
           
             
FROM       @PurchasDelivery
GROUP BY   RouteIds,RouteName,FactoryIds ,FactoryName,ClerkIds,ClerkName,ClerkCode,PurchaseDate,DeliveryNoteId 
           
           --END PN and DN
           
OPEN gainslosses_cursor
FETCH NEXT FROM gainslosses_cursor
INTO    @FactoryIds ,
        @FactoryName ,
        @RouteIds ,
        @RouteName ,
        @ClerkIds ,
        @ClerkName ,
        @ClerkCode ,
        @ReceiptWeight ,
        @LorryWeight ,
        @PurchaseDate ,
        @DeliveryNoteId

     
WHILE @@FETCH_STATUS = 0  
BEGIN
 
SELECT @FactoryWeight = sum(storeItem.Weight )                
FROM       dbo.tblSourcingDocument store
INNER JOIN dbo.tblSourcingLineItem storeItem ON store.Id = storeItem.DocumentId
INNER JOIN dbo.tblSourcingLineItem recptItem ON recptItem.Id = storeItem.ParentId
INNER JOIN dbo.tblSourcingDocument recpt ON recptItem.DocumentId = recpt.Id
WHERE --convert(nvarchar(26),store.DocumentDate,23) between @startDate and  @endDate
   (store.DocumentTypeId = 15) 
and(recpt.DocumentParentId = @DeliveryNoteId)

INSERT INTO @GainsLosses VALUES(    @FactoryIds ,
									@FactoryName ,
									@RouteIds ,
									@RouteName ,
									@ClerkIds ,
									@ClerkName ,
									@ClerkCode ,
									@ReceiptWeight ,
									@LorryWeight ,
									@FactoryWeight ,
									@PurchaseDate ,
									@DeliveryNoteId
                                   
                                   );
FETCH NEXT FROM gainslosses_cursor INTO @FactoryIds ,
										@FactoryName ,
										@RouteIds ,
										@RouteName ,
										@ClerkIds ,
										@ClerkName ,
										@ClerkCode ,
										@ReceiptWeight ,
										@LorryWeight ,
										@PurchaseDate ,
										@DeliveryNoteId
                                     
END;        
CLOSE  gainslosses_cursor ;      
DEALLOCATE gainslosses_cursor;

SELECT  FactoryIds ,
		FactoryName ,
		RouteIds ,
		RouteName ,
		ClerkIds ,
		ClerkName ,
		ClerkCode ,
		SUM(ReceiptWeight)ReceiptWeight,
		SUM(LorryWeight) LorryWeight,
		SUM(FactoryWeight) FactoryWeight ,
	    SUM(LorryWeight)- SUM(ReceiptWeight) as VarianceBA,
	  -- ABS((SUM(LorryWeight)- SUM(ReceiptWeight))/SUM(ReceiptWeight))as VarianceBAA,
	    SUM(FactoryWeight)-SUM(ReceiptWeight)as VarianceCA,
	 --  ABS((AVG(FactoryWeight)-SUM(ReceiptWeight))/SUM(ReceiptWeight))as VarianceCAA,

		PurchaseDate 
FROM   @GainsLosses
WHERE CONVERT(NVARCHAR(50),FactoryIds)LIKE ISNULL(@hubId,'%')
 AND CONVERT(NVARCHAR(50),RouteIds)LIKE ISNULL(@routeId,'%')
 AND CONVERT(NVARCHAR(50),ClerkIds)LIKE ISNULL(@clerkId,'%')
 AND CONVERT(NVARCHAR(26),PurchaseDate,23) between @startDate and @endDate
GROUP BY FactoryIds,FactoryName ,RouteIds ,RouteName ,ClerkIds ,ClerkName ,ClerkCode ,PurchaseDate


-- exec [dbo].[sp_A_GainsAndLosses_Final] @startDate ='03/01/2014',@endDate  ='02/27/2014',@hubId ='ALL',@routeId  ='ALL',@clerkId ='ALL'

-- exec [dbo].[sp_A_GainsAndLosses_Final] @startDate ='2015-03-01',@endDate  ='2015-03-24',@hubId ='ALL',@routeId  ='ALL',@clerkId ='E9A53968-5934-4D0A-AE7A-D4877FD98A8F'


GO