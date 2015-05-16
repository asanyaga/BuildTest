/****** Object:  StoredProcedure [dbo].[sp_D_SalesSummary_PerDistributor_PerProductType]    Script Date: 07/24/2013 08:51:32 ******/
DROP PROCEDURE [dbo].[sp_D_SalesSummary_PerDistributor_PerProductType]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[sp_D_SalesSummary_PerDistributor_PerProductType]
(
@startDate as datetime,
@endDate as datetime,
@distributorID AS NVARCHAR(50),
@salesmanId AS NVARCHAR(50),
@productTypeId AS NVARCHAR(50)
)
as 


if  rtrim(ltrim(@distributorID))='ALL'  begin set @distributorID='%' end
if  rtrim(ltrim(@salesmanId))='ALL'  begin set @salesmanId='%' end
if  rtrim(ltrim(@productTypeId))='ALL'  begin set @productTypeId='%' end
if  rtrim(ltrim(@productTypeId))='EMPTY'  begin set @productTypeId='EMPTY' end


                      SELECT DISTINCT dbo.tblLineItems.id, 
                             dbo.tblDocument.DocumentParentId, 
                             dbo.tblLineItems.ProductID, 
                             dbo.tblLineItems.Description, 
                             dbo.tblDocument.DocumentTypeId, 
                             dbo.tblLineItems.Quantity, 
                             dbo.tblLineItems.Value, 
                             dbo.tblLineItems.Vat, 
                             dbo.tblLineItems.IAN_Actual, 
                             dbo.tblDocument.DocumentReference, 
                             dbo.tblDocument.DocumentIssuerCostCentreId, 
                             dbo.tblDocument.DocumentIssuerUserId, 
                             dbo.tblDocument.DocumentDateIssued, 
                             dbo.tblDocument.DocumentRecipientCostCentre, 
                             dbo.tblDocument.OrderDateRequired, 
                             dbo.tblDocument.DocumentIssuerCostCentreApplicationId, 
                             dbo.tblDocument.OrderIssuedOnBehalfOfCC, 
                             dbo.tblDocument.InvoiceOrderId, 
                             dbo.tblDocument.OrderOrderTypeId, 
                             dbo.tblDocument.SendDateTime, 
                             dbo.tblDocument.PaymentDocId, 
                             dbo.tblCostCentre.Name AS Salesman, 
                             dbo.tblCostCentre.CostCentreType AS SalesmanTypeID, 
                             dbo.tblCostCentre.ParentCostCentreId, 
                             dbo.tblCostCentre.RouteId, 
                             dbo.tblLineItems.DocumentID,
                             dbo.tblDocument.OrderParentId,  
                             dbo.tblDocument.DocumentStatusId, 
                             dbo.tblLineItems.ApprovedQuantity, 
                             dbo.tblLineItems.LostSaleQuantity, 
                             dbo.tblLineItems.BackOrderQuantity, 
                             dbo.tblLineItems.DispatchedQuantity, 
                             tblCostCentre_1.Distributor_RegionId, 
                             tblCostCentre_1.Id AS DistributorID, 
                             tblCostCentre_1.Name AS Distributor, 
                             tblCostCentre_2.Name AS Producer, 
                             tblCostCentre_2.CostCentreType AS ProducerID, 
                             dbo.tblCostCentre.Id AS SalesManID, 
                             dbo.tblProduct.Description AS Product, 
                             dbo.tblProductBrand.name AS Brand, 
                             dbo.tblLineItems.ProductDiscount,
                             dbo.tblDocument.SaleDiscount,
                             case when dbo.tblProduct.ProductTypeId = dbo.tblProductType.id then (select dbo.tblProductType.name 
                                                                                from dbo.tblProductType 
                                                                                where dbo.tblProductType.id = dbo.tblProduct.ProductTypeId)
                                   when dbo.tblProduct.ProductTypeId is null then 'Returnable ProductType' end as productTypeName,
                              case when dbo.tblProduct.ProductTypeId = dbo.tblProductType.id then LOWER(convert (nvarchar(50),(select dbo.tblProductType.id 
                                                                                from dbo.tblProductType 
                                                                                where dbo.tblProductType.id = dbo.tblProduct.ProductTypeId)))
                                   when dbo.tblProduct.ProductTypeId is null then 'EMPTY' end as productTypeId,
                            -- dbo.tblProductType.id as productTypeId,
                            -- dbo.tblProductType.name as productTypeName,
                            (dbo.tblLineItems.ProductDiscount *  dbo.tblLineItems.Quantity) as TotalProductDiscount,
                             (select top(1) dbo.tblDocument.SaleDiscount)as SaleValueDiscount,
							 	  ROUND(((dbo.tblLineItems.Quantity)*(dbo.tblLineItems.Value + dbo.tblLineItems.Vat)) +  (dbo.tblLineItems.ProductDiscount*dbo.tblLineItems.Quantity) ,2,1) as GrossAmount,
							   dbo.udf_D_RoundOff((dbo.tblLineItems.Quantity*dbo.tblLineItems.Value)) as NetRoundOff,
							   ROUND((dbo.tblLineItems.Quantity*dbo.tblLineItems.Value),2,1) as NetTruncate,
							   dbo.udf_D_RoundOff(((dbo.tblLineItems.Quantity)*(dbo.tblLineItems.Value + dbo.tblLineItems.Vat))) as GrossRoundOff

                      FROM    dbo.tblDocument INNER JOIN
                              dbo.tblLineItems ON dbo.tblDocument.Id = dbo.tblLineItems.DocumentID INNER JOIN
                              dbo.tblCostCentre ON dbo.tblDocument.DocumentRecipientCostCentre = dbo.tblCostCentre.Id INNER JOIN
                              dbo.tblCostCentre AS tblCostCentre_1 ON dbo.tblCostCentre.ParentCostCentreId = tblCostCentre_1.Id INNER JOIN
                              dbo.tblCostCentre AS tblCostCentre_2 ON tblCostCentre_1.ParentCostCentreId = tblCostCentre_2.Id INNER JOIN
                              dbo.tblProduct ON dbo.tblLineItems.ProductID = dbo.tblProduct.id INNER JOIN
                              dbo.tblProductBrand ON dbo.tblProduct.BrandId = dbo.tblProductBrand.id,dbo.tblProductType
                              --INNER JOIN dbo.tblProductType on dbo.tblProduct.ProductTypeId = dbo.tblProductType.id
                              
                      WHERE  ((dbo.tblProduct.ProductTypeId = dbo.tblProductType.id) or (dbo.tblProduct.ProductTypeId is null))and    
                      (CONVERT(VARCHAR(26), dbo.tblDocument.DocumentDateIssued, 23) BETWEEN @startDate AND @endDate) 
                      AND(dbo.tblCostCentre.CostCentreType = 4) 
                      AND (dbo.tblDocument.DocumentTypeId = 1) 
                      AND (dbo.tblDocument.OrderOrderTypeId = 1 OR( dbo.tblDocument.OrderOrderTypeId = 3 AND dbo.tblDocument.DocumentStatusId = 99)) 
                      AND (convert(nvarchar(50),tblCostCentre_1.Id) LIKE ISNULL(@distributorID, '%')) 
                      AND (convert(nvarchar(50),dbo.tblCostCentre.Id) LIKE ISNULL(@salesmanId, '%')) 
                      AND (convert(nvarchar(50),dbo.tblProductType.id) Like ISNULL(@productTypeId,'%'))
					  AND tblProduct.ProductTypeId IS NOT NULL 
				      AND CONVERT(NVARCHAR(50),tblProduct.ProductTypeId) <>''

UNION ALL
SELECT DISTINCT tblLineItems_1.id, 
       tblDocument_1.DocumentParentId,
       tblLineItems_1.ProductID, 
       tblLineItems_1.Description, 
       tblDocument_1.DocumentTypeId, 
       tblLineItems_1.Quantity, 
       tblLineItems_1.Value, 
       tblLineItems_1.Vat, 
       tblLineItems_1.IAN_Actual, 
       tblDocument_1.DocumentReference, 
       tblDocument_1.DocumentIssuerCostCentreId, 
       tblDocument_1.DocumentIssuerUserId, 
       tblDocument_1.DocumentDateIssued,       
       tblDocument_1.DocumentRecipientCostCentre, 
       tblDocument_1.OrderDateRequired, 
       tblDocument_1.DocumentIssuerCostCentreApplicationId, 
       tblDocument_1.OrderIssuedOnBehalfOfCC, 
       tblDocument_1.InvoiceOrderId, 
       tblDocument_1.OrderOrderTypeId, 
       tblDocument_1.SendDateTime, 
       tblDocument_1.PaymentDocId, 
       tblCostCentre_3.Name AS Salesman, 
       tblCostCentre_3.CostCentreType AS SalesmanTypeID, 
       tblCostCentre_3.ParentCostCentreId, 
       tblCostCentre_3.RouteId, 
       tblLineItems_1.DocumentID,
       tblDocument_1.OrderParentId, 
       tblDocument_1.DocumentStatusId, 
       tblLineItems_1.ApprovedQuantity, 
       tblLineItems_1.LostSaleQuantity, 
       tblLineItems_1.BackOrderQuantity, 
       tblLineItems_1.DispatchedQuantity, 
       tblCostCentre_1.Distributor_RegionId, 
       tblCostCentre_1.Id AS DistributorID, 
       tblCostCentre_1.Name AS Distributor, 
       tblCostCentre_2.Name AS Producer, 
       tblCostCentre_2.CostCentreType AS ProducerID, 
       tblCostCentre_3.Id AS SalesManID, 
       tblProduct_1.Description AS Product, 
       tblProductBrand_1.name AS Brand, 
       tblLineItems_1.ProductDiscount,
       tblDocument_1.SaleDiscount, 
       case when tblProduct_1.ProductTypeId = dbo.tblProductType.id then (select dbo.tblProductType.name 
                                                                                from dbo.tblProductType 
                                                                                where dbo.tblProductType.id = tblProduct_1.ProductTypeId)
                                   when tblProduct_1.ProductTypeId is null then 'No ProductType' end as productTypeName,
                              case when tblProduct_1.ProductTypeId = dbo.tblProductType.id then convert (nvarchar(50),(select dbo.tblProductType.id 
                                                                                from dbo.tblProductType 
                                                                                where dbo.tblProductType.id = tblProduct_1.ProductTypeId))
                                   when tblProduct_1.ProductTypeId is null then 'EMPTY' end as productTypeId,
     -- dbo.tblProductType.id as productTypeId,
      -- dbo.tblProductType.name as productTypeName,
       (tblLineItems_1.ProductDiscount *  tblLineItems_1.Quantity) as TotalProductDiscount,

       (select top(1) tblDocument_1.SaleDiscount) as SaleValueDiscount,
	   ROUND(((tblLineItems_1.Quantity)*(tblLineItems_1.Value + tblLineItems_1.Vat)) +  (tblLineItems_1.ProductDiscount*tblLineItems_1.Quantity) ,2,1) as GrossAmount,
	   dbo.udf_D_RoundOff((tblLineItems_1.Quantity*tblLineItems_1.Value)) as NetRoundOff,
	   ROUND((tblLineItems_1.Quantity*tblLineItems_1.Value),2,1) as NetTruncate,
	   dbo.udf_D_RoundOff(((tblLineItems_1.Quantity)*(tblLineItems_1.Value + tblLineItems_1.Vat))) as GrossRoundOff

FROM  dbo.tblProductBrand AS tblProductBrand_1 
 JOIN dbo.tblProduct AS tblProduct_1 ON tblProductBrand_1.id = tblProduct_1.BrandId 
 JOIN dbo.tblCostCentre AS tblCostCentre_1 
 JOIN dbo.tblCostCentre AS tblCostCentre_3 ON tblCostCentre_1.Id = tblCostCentre_3.ParentCostCentreId 
 JOIN dbo.tblCostCentre AS tblCostCentre_2 ON tblCostCentre_1.ParentCostCentreId = tblCostCentre_2.Id 
 JOIN dbo.tblDocument AS tblDocument_1 
 JOIN dbo.tblLineItems AS tblLineItems_1 ON tblDocument_1.Id = tblLineItems_1.DocumentID 
                                         ON tblCostCentre_3.Id = tblDocument_1.DocumentIssuerCostCentreId 
										 ON tblProduct_1.id = tblLineItems_1.ProductID ,dbo.tblProductType
              -- inner join dbo.tblProductType on tblProduct_1.ProductTypeId = dbo.tblProductType.id
WHERE (( tblProduct_1.ProductTypeId = dbo.tblProductType.id) or ( tblProduct_1.ProductTypeId is null))and    
 (tblDocument_1.DocumentTypeId = 1) 
       AND (tblDocument_1.OrderOrderTypeId = 1 OR (tblDocument_1.OrderOrderTypeId = 3 AND tblDocument_1.DocumentStatusId = 99))
                AND (CONVERT(VARCHAR(26), tblDocument_1.DocumentDateIssued, 23) BETWEEN @startDate AND @endDate) 
                AND (tblCostCentre_3.CostCentreType = 4) 
                AND (tblCostCentre_3.Id LIKE ISNULL(@salesmanId, '%')) 
                And (dbo.tblProductType.id Like ISNULL(@productTypeId,'%')) 
				AND tblProduct_1.ProductTypeId IS NOT NULL --
				AND CONVERT(NVARCHAR(50),tblProduct_1.ProductTypeId) <>''--
                AND (tblCostCentre_1.Id LIKE ISNULL(@distributorID, '%')) 

                 
--  Exec [dbo].[sp_D_SalesSummary_PerDistributor_PerProductType] @startDate = '01-Jan-2015',@endDate = '21-Jan-2015',@distributorID ='ALL',@salesmanId = 'ALL',@productTypeId = 'ALL'


GO
