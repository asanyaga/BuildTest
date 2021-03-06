/****** Object:  StoredProcedure [dbo].[sp_D_Sales_By_Route]    Script Date: 09/10/2013 10:12:03 ******/

DROP PROCEDURE [dbo].[sp_D_Sales_By_Route]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[sp_D_Sales_By_Route]
(
@startDate as datetime,
@endDate as datetime, 
@countryId as nvarchar(50),
@regionId as nvarchar(100),
@distributorID as nvarchar(50),
@routeId as nvarchar(50)

)
as 
if  RTRIM(LTRIM(@regionId))='ALL'   begin set @regionId='%' end
if  RTRIM(LTRIM(@countryId))='ALL'  begin set @countryId='%' end
if  RTRIM(LTRIM(@distributorID))='ALL'  begin set @distributorID='%' end
if  RTRIM(LTRIM(@routeId))='ALL'  begin set @routeId='%' end

SELECT     saleItem.ProductID, 
           saleItem.Description, 
           saleItem.Quantity, 
           saleItem.ProductDiscount, 
           (saleItem.ProductDiscount * saleItem.Quantity) as TotalProductDiscount,
           saleItem.Value, 
           saleItem.Vat, 
           sale.DocumentReference, 
           salesman.Id AS SalesManID, 
           salesman.Name AS Salesman, 
           distributor.Id AS DistributorID, 
           distributor.Name AS Distributor, 
           HQ.Name AS Producer, 
           product.Description AS Product, 
           brand.name AS Brand,
          (SELECT     TOP (1) sale.SaleDiscount) AS SaleValueDiscount, 
          sale.DocumentDateIssued, 
          outlet.Id as OutletId, 
          outlet.Name as OutletName, 
          [route].RouteID AS RouteId,
          [route].Name AS Route,
		  ROUND(((saleItem.Quantity)*(saleItem.Value + saleItem.Vat)) +  (saleItem.ProductDiscount*saleItem.Quantity) ,2,1) as GrossAmount,
		  dbo.udf_D_RoundOff((saleItem.Quantity*saleItem.Value)) as NetRoundOff,
		  ROUND((saleItem.Quantity*saleItem.Value),2,1) as NetTruncate,
		  dbo.udf_D_RoundOff(((saleItem.Quantity)*(saleItem.Value + saleItem.Vat))) as GrossRoundOff

FROM         dbo.tblDocument AS sale INNER JOIN
                      dbo.tblLineItems AS saleItem ON sale.Id = saleItem.DocumentID INNER JOIN
                      dbo.tblCostCentre AS salesman ON sale.DocumentRecipientCostCentre = salesman.Id OR sale.DocumentIssuerCostCentreId = salesman.Id INNER JOIN
                      dbo.tblCostCentre AS distributor ON salesman.ParentCostCentreId = distributor.Id INNER JOIN
                      dbo.tblCostCentre AS HQ ON distributor.ParentCostCentreId = HQ.Id INNER JOIN
                      dbo.tblProduct AS product ON saleItem.ProductID = product.id INNER JOIN
                      dbo.tblProductBrand AS brand ON product.BrandId = brand.id INNER JOIN
                      dbo.tblCostCentre outlet ON sale.OrderIssuedOnBehalfOfCC =outlet.Id INNER JOIN
                      dbo.tblRoutes [route] ON outlet.RouteId = [route].RouteID INNER JOIN
                      dbo.tblRegion region ON distributor.Distributor_RegionId = region.id INNER JOIN
                      dbo.tblCountry country ON region.Country = country.id

WHERE (salesman.CostCentreType = 4) 
 AND (sale.DocumentTypeId = 1) 
 AND ((sale.OrderOrderTypeId = 1) OR (sale.OrderOrderTypeId = 3 AND sale.DocumentStatusId = 99))
 AND  (CONVERT(VARCHAR(26), sale.DocumentDateIssued, 23) BETWEEN @startDate AND @endDate)                 
 AND (CONVERT(NVARCHAR(50), distributor.Id) LIKE ISNULL(@distributorID, N'%')) 
 AND (CONVERT(NVARCHAR(50), [route].RouteID) LIKE ISNULL(@routeId, N'%'))
 AND (CONVERT(NVARCHAR(50), country.id) LIKE ISNULL(@countryId, N'%'))
 AND (CONVERT(NVARCHAR(50), region.id) LIKE ISNULL(@regionId, N'%'))

                
                
                
 --  Exec  [dbo].[sp_D_Sales_By_Route] @startDate='2014-09-28',@endDate='2015-01-15',@distributorID='ALL',@routeId='ALL',@countryId='ALL',@regionId='ALL'
                
  GO