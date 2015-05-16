DROP PROCEDURE [dbo].[sp_D_DSS_Sales]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[sp_D_DSS_Sales]
(
@TransactionDate as datetime,
@salesmanId AS NVARCHAR(50),
@distributorId AS NVARCHAR(50),
@outletId AS NVARCHAR(50)
)
as 
if ltrim(rtrim(@outletId))='ALL'  begin set @outletId='%' end
if ltrim(rtrim(@distributorId))='ALL'  begin set @distributorId='%' end
if ltrim(rtrim(@salesmanId))='ALL'  begin set @salesmanId='%' end

BEGIN
DECLARE @DaysTransValue Table(SaleRef nvarchar(50),
							  Distributor nvarchar(50),
                              Salesman nvarchar(50),
                              GrossSales decimal(18,2),
                              TotalSaleValueDiscount decimal(18,2)
)
INSERT @DaysTransValue
SELECT     sale.DocumentReference  as SaleRef,dist.Name as DistributorName,
           salesman.Name as SalesmanName,
           dbo.udf_D_RoundOff(Sum(saleItems.Quantity * (saleItems.Value + saleItems.Vat))) as DaysTransValue, 
           sale.SaleDiscount as SaleValueDiscount
    --       sum(saleItems.Quantity * saleItems.Vat) as TotalVat                 

                         
FROM       dbo.tblDocument  sale
INNER JOIN dbo.tblLineItems saleItems ON sale.Id = saleItems.DocumentID
INNER JOIN dbo.tblCostCentre dist on sale.DocumentIssuerCostCentreId = dist.Id or sale.DocumentRecipientCostCentre = dist.Id
INNER JOIN dbo.tblCostCentre salesman  on sale.DocumentIssuerCostCentreId = salesman.Id or sale.DocumentRecipientCostCentre = salesman.Id


WHERE     (CONVERT(nvarchar(26), sale.DocumentDateIssued, 23) BETWEEN @TransactionDate AND @TransactionDate) 
                            AND CONVERT(NVARCHAR(50),dist.Id) like ISNULL(@distributorId,'%')
                            AND CONVERT(NVARCHAR(50),salesman.Id) like ISNULL(@salesmanId,'%')
                            AND CONVERT(NVARCHAR(50),sale.OrderIssuedOnBehalfOfCC) like ISNULL(@outletId,'%')
                            AND (sale.DocumentTypeId = 1)
                            AND ((sale.OrderOrderTypeId = 1) OR (sale.OrderOrderTypeId = 3 AND sale.DocumentStatusId = 99)) 
                            AND dist.CostCentreType = 2
                            AND salesman.CostCentreType = 4
Group by sale.DocumentReference,sale.SaleDiscount,dist.Name,salesman.Name
Order by salesman.Name asc
END;
SELECT SUM(GrossSales)- SUM(TotalSaleValueDiscount) AS DaysTransactionValue
FROM @DaysTransValue


                      
-- Exec [dbo].[sp_D_DSS_Sales] @TransactionDate = '2013-10-31', @salesmanId = '382acae3-1d88-4765-b998-122d199243f2',@outletId = 'a6f4ff50-c931-4a98-b7bd-a8705fda6b8e'

-- Exec [dbo].[sp_D_DSS_Sales] @TransactionDate = '2014-04-05', @salesmanId = 'ALL',@outletId = 'ALL',@distributorId='BF9B69F7-0778-4F46-B9CB-37973B1B9F17'

-- Exec [dbo].[sp_D_DSS_Sales] @TransactionDate = '2015-04-13', @salesmanId = 'ALL',@outletId = 'ALL',@distributorId='ALL'


GO


