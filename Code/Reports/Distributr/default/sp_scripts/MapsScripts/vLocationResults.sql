/****** Object:  View [dbo].[vLocationResults]    Script Date: 12/10/2014 16:29:46 ******/
IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[vLocationResults]'))
DROP VIEW [dbo].[vLocationResults]
GO

/****** Object:  View [dbo].[vLocationResults]    Script Date: 12/10/2014 16:29:47 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE VIEW [dbo].[vLocationResults]
AS
SELECT TOP (100) PERCENT ROW_NUMBER() OVER (ORDER BY dbo.tblDocument.DocumentDateIssued ASC) AS Row,
        outlet.Id as ResultID, 
        tblDistributor.Id AS DistributorId,
        tblDistributor.Name AS Distributor, 
        outlet.Id AS OutletId, 
        outlet.Name AS Outlet, 
        dbo.tblLineItems.Quantity * (dbo.tblLineItems.Value + dbo.tblLineItems.Vat) AS SaleAmount, 
        dbo.tblDocument.DocumentReference, 
        dbo.tblDocument.DocumentDateIssued, 
        isnull(dbo.tblDocument.Latitude, '') AS Latitude,
        isnull(dbo.tblDocument.Longitude, '') AS Longitude,
		dbo.tblRoutes.RouteID AS RouteID, 
		dbo.tblRoutes.Name AS Route,
    	 salesman.Name AS Salesman, 
		 salesman.Id AS SalesmanID, salesman.CostCentreType AS SalesmanTypeID, dbo.tblRoutes.RouteID AS RouteIDroutesTable, 
		dbo.tblLineItems.ProductDiscount, dbo.tblRegion.Name AS Region, dbo.tblCountry.Name AS Country, dbo.tblDocument.SaleDiscount, 
		dbo.tblLineItems.ProductDiscount * dbo.tblLineItems.Quantity AS TotalProductDiscount
FROM  dbo.tblCostCentre AS tblDistributor 
JOIN  dbo.tblCostCentre outlet ON tblDistributor.Id = outlet.ParentCostCentreId 
JOIN  dbo.tblDocument 
JOIN  dbo.tblLineItems ON dbo.tblDocument.Id = dbo.tblLineItems.DocumentID ON outlet.Id = dbo.tblDocument.OrderIssuedOnBehalfOfCC 
JOIN  dbo.tblRoutes ON outlet.RouteId = dbo.tblRoutes.RouteID 
JOIN  dbo.tblCostCentre AS salesman ON dbo.tblDocument.DocumentRecipientCostCentre = salesman.Id or  dbo.tblDocument.DocumentIssuerCostCentreId = salesman.Id
JOIN  dbo.tblRegion ON dbo.tblRoutes.RegionId = dbo.tblRegion.id 
JOIN  dbo.tblCountry ON dbo.tblRegion.Country = dbo.tblCountry.id
WHERE(dbo.tblDocument.DocumentTypeId = 1)
 --AND (dbo.tblDocument.OrderOrderTypeId = 1 OR   dbo.tblDocument.OrderOrderTypeId = 3)  AND (dbo.tblDocument.DocumentStatusId = 99)
 AND tblDistributor.CostCentreType = 2
 AND  salesman.CostCentreType = 4
 AND (outlet.CostCentreType = 5)
ORDER BY dbo.tblDocument.DocumentDateIssued


GO
