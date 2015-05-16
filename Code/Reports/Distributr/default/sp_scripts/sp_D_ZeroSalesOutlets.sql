DROP PROCEDURE [dbo].[sp_D_ZeroSalesOutlets]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_D_ZeroSalesOutlets]
(
@startDate as Date,
@endDate as Date,
@distributorId AS NVARCHAR(50),
@routeId AS NVARCHAR(50),
@outletId AS NVARCHAR(50)

)
AS 
if  ltrim(rtrim(@distributorId))='ALL'  begin set @distributorId='%' end
if  ltrim(rtrim(@routeId))='ALL'  begin set @routeId='%' end
if  ltrim(rtrim(@outletId))='ALL'  begin set @outletId='%' end

SELECT     dbo.tblCostCentre.Id AS OutletId, 
           dbo.tblCostCentre.Name AS OutletName, 
           dbo.tblCostCentre.CostCentreType AS OutletCctype, 
           dbo.tblRoutes.RouteID AS RouteId, 
           dbo.tblRoutes.Name AS RouteName, 
           tblCostCentre_1.Id AS DistributorId, 
           tblCostCentre_1.Name AS DistributorName
FROM         dbo.tblCostCentre INNER JOIN
             dbo.tblRoutes ON dbo.tblCostCentre.RouteId = dbo.tblRoutes.RouteID INNER JOIN
             dbo.tblCostCentre AS tblCostCentre_1 ON dbo.tblCostCentre.ParentCostCentreId = tblCostCentre_1.Id
WHERE     (dbo.tblCostCentre.CostCentreType = 5)
          AND(CONVERT(NVARCHAR(50),tblCostCentre_1.Id) LIKE ISNULL(@distributorId,N'%')) 
          AND(CONVERT(NVARCHAR(50), dbo.tblRoutes.RouteID) LIKE ISNULL(@routeId,N'%')) 
          AND(CONVert(NVARCHAR(50), dbo.tblCostCentre.Id ) LIKE ISNULL(@outletId,N'%'))
          AND(dbo.tblCostCentre.Id
           NOT IN(SELECT      dbo.tblDocument.OrderIssuedOnBehalfOfCC
                  FROM        dbo.tblDocument INNER JOIN
                              dbo.tblLineItems as line ON dbo.tblDocument.Id = line.DocumentID
                  WHERE      (dbo.tblDocument.DocumentTypeId = 1)
                        AND convert(nvarchar(26),(dbo.tblDocument.DocumentDateIssued),23) between @startDate and @endDate 
                        AND ((dbo.tblDocument.OrderOrderTypeId = 1)OR(dbo.tblDocument.OrderOrderTypeId = 3 AND dbo.tblDocument.DocumentStatusId = 99))))                        
        --  Exec [dbo].[sp_D_ZeroSalesOutlets] @distributorId='ALL',@routeId='ALL', @startDate ='30-july-2013',@endDate ='31-july-2013'
GO