/****** Object:  StoredProcedure [dbo].[sp_D_SalesSummaryByDate]    Script Date: 07/24/2013 08:51:32 ******/
DROP PROCEDURE [dbo].[sp_D_SalesSummaryByDate]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[sp_D_SalesSummaryByDate]
(


@startDate as datetime,
@endDate as datetime,
@distributorID AS NVARCHAR(50),
@salesManID AS NVARCHAR(50),
@brandId AS NVARCHAR(50),
@productId AS NVARCHAR(50)


)
as 
if LTRIM(RTRIM(@distributorID))='ALL' begin set @distributorID='%' end
if LTRIM(RTRIM(@salesManID))='ALL' begin set @salesManID='%' end
if LTRIM(RTRIM(@brandId))='ALL' begin set @brandId='%' end
if LTRIM(RTRIM(@productId))='ALL' begin set @productId='%' end

SELECT dbo.tblLineItems.id, dbo.tblLineItems.ProductID, dbo.tblLineItems.Description, dbo.tblDocument.DocumentTypeId, dbo.tblLineItems.Quantity, 
               dbo.tblLineItems.Value, dbo.tblLineItems.Vat, dbo.tblDocument.DocumentIssuerCostCentreId, dbo.tblDocument.DocumentIssuerUserId, 
               dbo.tblDocument.DocumentDateIssued, dbo.tblDocument.DocumentRecipientCostCentre, dbo.tblDocument.DocumentIssuerCostCentreApplicationId, 
               dbo.tblDocument.OrderIssuedOnBehalfOfCC, dbo.tblDocument.OrderOrderTypeId, dbo.tblDocument.SendDateTime, dbo.tblCostCentre.Name AS Salesman, 
               dbo.tblCostCentre.CostCentreType AS SalesmanTypeID, dbo.tblCostCentre.ParentCostCentreId, dbo.tblCostCentre.RouteId, dbo.tblLineItems.DocumentID, 
               dbo.tblDocument.DocumentStatusId, tblCostCentre_1.Id AS DistributorID, tblCostCentre_1.Name AS Distributor, tblCostCentre_2.Name AS Producer, 
               tblCostCentre_2.CostCentreType AS ProducerID, dbo.tblCostCentre.Id AS SalesManID, dbo.tblProduct.Description AS Product, 
               dbo.tblProductBrand.name AS Brand,dbo.tblProductBrand.id AS BrandId,  dbo.tblLineItems.ProductDiscount, tblDocument.SaleDiscount,
               (dbo.tblLineItems.ProductDiscount * dbo.tblLineItems.Quantity) as TotalProductDiscount,
			   ROUND(((dbo.tblLineItems.Quantity)*(dbo.tblLineItems.Value + dbo.tblLineItems.Vat)) +  (dbo.tblLineItems.ProductDiscount*dbo.tblLineItems.Quantity) ,2,1) as GrossAmount,
			   dbo.udf_D_RoundOff((dbo.tblLineItems.Quantity*dbo.tblLineItems.Value)) as NetRoundOff,
			   ROUND((dbo.tblLineItems.Quantity*dbo.tblLineItems.Value),2,1) as NetTruncate,
			   dbo.udf_D_RoundOff(((dbo.tblLineItems.Quantity)*(dbo.tblLineItems.Value + dbo.tblLineItems.Vat))) as GrossRoundOff

FROM  dbo.tblDocument INNER JOIN
               dbo.tblLineItems ON dbo.tblDocument.Id = dbo.tblLineItems.DocumentID INNER JOIN
               dbo.tblCostCentre ON (dbo.tblDocument.DocumentRecipientCostCentre = dbo.tblCostCentre.Id or dbo.tblDocument.DocumentIssuerCostCentreId = dbo.tblCostCentre.Id )INNER JOIN
               dbo.tblCostCentre AS tblCostCentre_1 ON dbo.tblCostCentre.ParentCostCentreId = tblCostCentre_1.Id INNER JOIN
               dbo.tblCostCentre AS tblCostCentre_2 ON tblCostCentre_1.ParentCostCentreId = tblCostCentre_2.Id INNER JOIN
               dbo.tblProduct ON dbo.tblLineItems.ProductID = dbo.tblProduct.id INNER JOIN
               dbo.tblProductBrand ON dbo.tblProduct.BrandId = dbo.tblProductBrand.id
WHERE (dbo.tblDocument.DocumentTypeId = 1)
 AND (dbo.tblDocument.OrderOrderTypeId = 1 OR (dbo.tblDocument.OrderOrderTypeId = 3 AND dbo.tblDocument.DocumentStatusId = 99)) 
 AND (CONVERT(VARCHAR(26),dbo.tblDocument.DocumentDateIssued,23) BETWEEN @startDate AND @endDate)
 AND (CONVERT(NVARCHAR(50),tblCostCentre_1.Id) LIKE ISNULL(@distributorID, '%')) 
 AND (dbo.tblCostCentre.CostCentreType = 4)
 AND (CONVERT(NVARCHAR(50),dbo.tblCostCentre.Id) LIKE ISNULL(@salesManID, '%')) 
 AND (CONVERT(NVARCHAR(50),dbo.tblProductBrand.id) LIKE ISNULL(@brandId, '%'))
 AND (CONVERT(NVARCHAR(50),dbo.tblProduct.id) LIKE ISNULL(@productId, '%'))
Order by dbo.tblDocument.DocumentDateIssued desc

--UNION 
--SELECT tblLineItems_1.id, tblLineItems_1.ProductID, tblLineItems_1.Description, tblDocument_1.DocumentTypeId, tblLineItems_1.Quantity, tblLineItems_1.Value, 
--               tblLineItems_1.Vat, tblDocument_1.DocumentIssuerCostCentreId, tblDocument_1.DocumentIssuerUserId, tblDocument_1.DocumentDateIssued, 
--               tblDocument_1.DocumentRecipientCostCentre, tblDocument_1.DocumentIssuerCostCentreApplicationId, tblDocument_1.OrderIssuedOnBehalfOfCC, 
--               tblDocument_1.OrderOrderTypeId, tblDocument_1.SendDateTime, tblCostCentre_3.Name AS Salesman, tblCostCentre_3.CostCentreType AS SalesmanTypeID, 
--               tblCostCentre_3.ParentCostCentreId, tblCostCentre_3.RouteId, tblLineItems_1.DocumentID, tblDocument_1.DocumentStatusId, 
--               tblCostCentre_1.Id AS DistributorID, tblCostCentre_1.Name AS Distributor, tblCostCentre_2.Name AS Producer, tblCostCentre_2.CostCentreType AS ProducerID, 
--               tblCostCentre_3.Id AS SalesManID, tblProduct_1.Description AS Product, tblProductBrand_1.name AS Brand, tblLineItems_1.ProductDiscount
--FROM  dbo.tblProductBrand AS tblProductBrand_1 INNER JOIN
--               dbo.tblProduct AS tblProduct_1 ON tblProductBrand_1.id = tblProduct_1.BrandId INNER JOIN
--               dbo.tblCostCentre AS tblCostCentre_1 INNER JOIN
--               dbo.tblCostCentre AS tblCostCentre_3 ON tblCostCentre_1.Id = tblCostCentre_3.ParentCostCentreId INNER JOIN
--               dbo.tblCostCentre AS tblCostCentre_2 ON tblCostCentre_1.ParentCostCentreId = tblCostCentre_2.Id INNER JOIN
--               dbo.tblDocument AS tblDocument_1 INNER JOIN
--               dbo.tblLineItems AS tblLineItems_1 ON tblDocument_1.Id = tblLineItems_1.DocumentID ON tblCostCentre_3.Id = tblDocument_1.DocumentIssuerCostCentreId ON 
--               tblProduct_1.id = tblLineItems_1.ProductID
--WHERE (tblDocument_1.DocumentTypeId = 1) 
--				AND (tblDocument_1.OrderOrderTypeId = 1 
--				OR tblDocument_1.OrderOrderTypeId = 3) 
--               AND (CONVERT(VARCHAR(26),tblDocument_1.SendDateTime,23) BETWEEN @startDate AND @endDate)
--                AND (CONVERT(NVARCHAR(50),tblCostCentre_1.Id) LIKE ISNULL(@distributorID, '%'))
--                AND (tblCostCentre_3.CostCentreType = 4) 
--                AND (CONVERT(NVARCHAR(50),tblCostCentre_3.Id) LIKE ISNULL(@salesManID, '%'))
--                AND (tblDocument_1.DocumentStatusId = 99)


--Exec [dbo].[sp_D_SalesSummaryByDate] @startDate = '22-July-2013',@endDate = '22-July-2013',@distributorID ='ALL',@salesManID = 'ALL',@brandId='ALL'

GO
