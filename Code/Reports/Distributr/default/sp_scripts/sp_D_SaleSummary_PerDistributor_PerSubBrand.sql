/****** Object:  StoredProcedure [dbo].[sp_D_SaleSummary_PerDistributor_PerSubBrand]    Script Date: 07/24/2013 08:51:32 ******/
DROP PROCEDURE [dbo].[sp_D_SaleSummary_PerDistributor_PerSubBrand]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[sp_D_SaleSummary_PerDistributor_PerSubBrand]
(

@startDate as datetime,
@endDate as datetime,
@distributorID AS NVARCHAR(50),
@salesmanId AS NVARCHAR(50),
@brandId AS NVARCHAR(50),
@productTypeId as nvarchar(50),
@subBrandId as nvarchar(50)


)
as 
if LTRIM(RTRIM(@distributorID))='ALL' begin set @distributorID='%' end
if LTRIM(RTRIM(@salesmanId))='ALL' begin set @salesmanId='%' end
if LTRIM(RTRIM(@productTypeId))='ALL' begin set @productTypeId='%' end
if LTRIM(RTRIM(@brandId))='ALL' begin set @brandId='%' end
if LTRIM(RTRIM(@subBrandId))='ALL' begin set @subBrandId='%' end

SELECT dbo.tblLineItems.id, 
       dbo.tblLineItems.ProductID, 
       dbo.tblLineItems.Description, 
       dbo.tblDocument.DocumentTypeId,
       dbo.tblLineItems.Quantity, 
       dbo.tblLineItems.Value, 
       dbo.tblLineItems.Vat, 
       dbo.tblDocument.DocumentIssuerCostCentreId, 
       dbo.tblDocument.DocumentIssuerUserId, 
       dbo.tblDocument.DocumentDateIssued, 
       dbo.tblDocument.DocumentRecipientCostCentre, 
       dbo.tblDocument.DocumentIssuerCostCentreApplicationId, 
       dbo.tblDocument.OrderIssuedOnBehalfOfCC,
       dbo.tblDocument.OrderOrderTypeId, 
       dbo.tblDocument.SendDateTime, 
       dbo.tblCostCentre.Name AS Salesman, 
       dbo.tblCostCentre.CostCentreType AS SalesmanTypeID, 
       dbo.tblCostCentre.ParentCostCentreId, 
       dbo.tblCostCentre.RouteId, 
       dbo.tblLineItems.DocumentID, 
       dbo.tblDocument.DocumentStatusId, 
           tblCostCentre_1.Id AS DistributorID,
           tblCostCentre_1.Name AS Distributor, 
           tblCostCentre_2.Name AS Producer, 
           tblCostCentre_2.CostCentreType AS ProducerID, 
       dbo.tblCostCentre.Id AS SalesManID, 
       dbo.tblProduct.Description AS Product, 
       dbo.tblProductBrand.name AS Brand,
       dbo.tblProductBrand.id AS BrandId,
       dbo.tblProductFlavour.id as SubBrandId,
       dbo.tblProductFlavour.name as SubBrandName,  
       dbo.tblLineItems.ProductDiscount, 
       (dbo.tblLineItems.ProductDiscount * dbo.tblLineItems.Quantity) as TotalProductDiscount,
           tblDocument.SaleDiscount,
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
               dbo.tblProductBrand ON dbo.tblProduct.BrandId = dbo.tblProductBrand.id INNER JOIN
               dbo.tblProductType on dbo.tblProduct.ProductTypeId = dbo.tblProductType.id INNER JOIN
               dbo.tblProductFlavour ON dbo.tblProduct.FlavourId = dbo.tblProductFlavour.id
WHERE (dbo.tblDocument.DocumentTypeId = 1)
 AND (dbo.tblDocument.OrderOrderTypeId = 1 OR (dbo.tblDocument.OrderOrderTypeId = 3 AND dbo.tblDocument.DocumentStatusId = 99)) 
 AND (CONVERT(VARCHAR(26),dbo.tblDocument.DocumentDateIssued,23) BETWEEN @startDate AND @endDate)
 AND (CONVERT(NVARCHAR(50),tblCostCentre_1.Id) LIKE ISNULL(@distributorID, '%')) 
 AND (dbo.tblCostCentre.CostCentreType = 4)
 AND (CONVERT(NVARCHAR(50),dbo.tblCostCentre.Id) LIKE ISNULL(@salesmanId, '%')) 
 AND (CONVERT(NVARCHAR(50),dbo.tblProductBrand.id) LIKE ISNULL(@brandId, '%'))
 AND (CONVERT(NVARCHAR(50),dbo.tblProductType.id) LIKE ISNULL(@productTypeId, '%'))
 AND (CONVERT(NVARCHAR(50), dbo.tblProductFlavour.id) LIKE ISNULL(@subBrandId, '%'))
 
--  Exec [dbo].[sp_D_SaleSummary_PerDistributor_PerSubBrand] @startDate = '22-July-2013',@endDate = '28-Aug-2013',@distributorID ='ALL',@salesmanId = 'ALL',@brandId='ALL',@productTypeId='ALL',@subBrandId='ALL'


GO
