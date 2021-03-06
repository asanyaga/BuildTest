/****** Object:  StoredProcedure [dbo].[SaleByOutlets]    Script Date: 07/24/2013 08:51:32 ******/
DROP PROCEDURE [dbo].[SaleByOutlets]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[SaleByOutlets]
(
@startDate as datetime,
@endDate as datetime,
@outLetID as nvarchar(50)

)
as 


if  @outLetID='ALL'  begin set @outLetID='%' end
--sales by saleman
--JION ON dbo.tblDocument.DocumentRecipientCostCentre = dbo.tblCostCentre.Id 
SELECT dbo.tblLineItems.id, dbo.tblDocument.DocumentParentId, dbo.tblLineItems.ProductID, dbo.tblLineItems.Description, dbo.tblDocument.DocumentTypeId, 
               dbo.tblLineItems.Quantity, dbo.tblLineItems.Value, dbo.tblLineItems.Vat, dbo.tblDocument.DocumentReference, dbo.tblDocument.DocumentIssuerUserId, 
               dbo.tblDocument.DocumentDateIssued, dbo.tblDocument.OrderDateRequired, dbo.tblDocument.DocumentIssuerCostCentreApplicationId, 
               dbo.tblDocument.OrderIssuedOnBehalfOfCC, dbo.tblDocument.OrderOrderTypeId, dbo.tblDocument.DocumentDateIssued as SendDateTime, dbo.tblCostCentre.Name AS Outlet, 
               dbo.tblCostCentre.CostCentreType AS OutletTypeID, dbo.tblCostCentre.ParentCostCentreId, dbo.tblCostCentre.RouteId, dbo.tblLineItems.DocumentID, 
               tblCostCentre_1.Id AS DistributorID, tblCostCentre_1.Name AS Distributor, dbo.tblRoutes.Name AS Route, dbo.tblDocument.DocumentIssuerCostCentreId, 
               dbo.tblDocument.DocumentRecipientCostCentre, tblCostCentre_3.Name AS Salesman, tblCostCentre_3.CostCentreType AS SalesmanTypeID, 
               dbo.tblRoutes.RouteID AS RouteIDroutesTable, dbo.tblLineItems.ProductDiscount, dbo.tblRegion.Name AS Region, dbo.tblCountry.Name AS Country, 
               dbo.tblProduct.Description AS Product, dbo.tblProductBrand.name AS Brand, dbo.tblCostCentre.Id AS OutLetID,dbo.tblDocument.SaleDiscount
FROM  dbo.tblCostCentre AS tblCostCentre_1 INNER JOIN
               dbo.tblCostCentre ON tblCostCentre_1.Id = dbo.tblCostCentre.ParentCostCentreId INNER JOIN
               dbo.tblDocument INNER JOIN
               dbo.tblLineItems ON dbo.tblDocument.Id = dbo.tblLineItems.DocumentID ON dbo.tblCostCentre.Id = dbo.tblDocument.OrderIssuedOnBehalfOfCC INNER JOIN
               dbo.tblRoutes ON dbo.tblCostCentre.RouteId = dbo.tblRoutes.RouteID INNER JOIN
               dbo.tblCostCentre AS tblCostCentre_3 ON dbo.tblDocument.DocumentRecipientCostCentre = tblCostCentre_3.Id INNER JOIN
               dbo.tblRegion ON dbo.tblRoutes.RegionId = dbo.tblRegion.id INNER JOIN
               dbo.tblCountry ON dbo.tblRegion.Country = dbo.tblCountry.id INNER JOIN
               dbo.tblProduct ON dbo.tblLineItems.ProductID = dbo.tblProduct.id INNER JOIN
               dbo.tblProductBrand ON dbo.tblProduct.BrandId = dbo.tblProductBrand.id
WHERE (dbo.tblDocument.DocumentTypeId = 1)
 AND (dbo.tblDocument.OrderOrderTypeId = 1 OR
               dbo.tblDocument.OrderOrderTypeId = 3)
                AND (dbo.tblCostCentre.CostCentreType = 5) 
                AND (dbo.tblDocument.DocumentStatusId = 99) AND 
               (CONVERT(VARCHAR(26), dbo.tblDocument.DocumentDateIssued,23) BETWEEN @startDate AND @endDate) 
               AND (CONVERT(NVARCHAR(50), dbo.tblCostCentre.Id) LIKE ISNULL(@outLetID, N'%'))
GO
