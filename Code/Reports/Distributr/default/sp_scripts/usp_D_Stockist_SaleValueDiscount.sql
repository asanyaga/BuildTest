IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_D_Stockist_SaleValueDiscount]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[usp_D_Stockist_SaleValueDiscount]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[usp_D_Stockist_SaleValueDiscount]
(
@startDate as datetime,
@endDate as datetime,
@distributorId AS NVARCHAR(50),
@stockistId AS NVARCHAR(50)
--@docId AS NVARCHAR(50)
)
as 
if  RTRIM(LTRIM(@distributorId))='ALL'  begin set @distributorId='%' end
if  RTRIM(LTRIM(@stockistId))='ALL'  begin set @stockistId='%' end
--if  RTRIM(LTRIM(@docId))='ALL'  begin set @docId='%' end

SELECT  
		dist.Id DistributorId,dist.Name DistributorName,
		stockist.Id SalesmanId,stockist.Name SalesmanName,
		sale.Id as saleId, 
		sale.SaleDiscount AS SaleValueDiscounts, 
		sale.DocumentDateIssued as SaleDate,
		sale.DocumentReference as SaleRef

FROM     dbo.tblDocument sale
	JOIN dbo.tblcostcentre stockist ON (sale.DocumentIssuerCostCentreId = stockist.id or sale.DocumentRecipientCostCentre = stockist.Id)
	JOIN dbo.tblcostcentre dist ON stockist.ParentCostCentreId = dist.Id
WHERE     (sale.OrderOrderTypeId = 1) 
     AND ((sale.DocumentTypeId = 1)OR(sale.OrderOrderTypeId = 3) AND (sale.DocumentStatusId = 99))
	 AND dist.CostCentreType = 2
	 AND stockist.CostCentreType = 4 and stockist.CostCentreType2 = 1

	 AND CONVERT(NVARCHAR(50),dist.id) LIKE ISNULL(@distributorId,'%')
	 AND CONVERT(NVARCHAR(50),stockist.id) LIKE ISNULL(@stockistId,'%')
     AND (CONVERT(NVARCHAR(26),DocumentDateIssued ,23)) between @startDate and @endDate


 -- EXEC [dbo].[usp_D_Stockist_SaleValueDiscount] @startDate='2014-12-01',@endDate='2014-12-22',@countryId=' ALL',@regionId=' ALL',@distributorId=' ALL',@salesmanId=' ALL',@routeId=' ALL',@outletId=' ALL',@docId=' ALL'
 

