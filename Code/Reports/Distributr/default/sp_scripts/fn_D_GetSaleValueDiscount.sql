DROP FUNCTION [dbo].[fn_D_GetSaleValueDiscount] 
GO


CREATE FUNCTION [dbo].[fn_D_GetSaleValueDiscount] 
(
@distributorId nvarchar(100),
@salesmanId nvarchar(100),
@TransactionDate date
)

RETURNS int


BEGIN
if  @distributorId='ALL' begin set @distributorId='%' end
if  @salesmanId='ALL' begin set @salesmanId='%' end

     DECLARE @TotalSaleValueDiscount decimal;
           SET @TotalSaleValueDiscount = (SELECT  cast((sum(saleItems.Quantity * (saleItems.Value + saleItems.Vat)) - sale.SaleDiscount) as decimal(18,2)) as NetSaleValue
                                          FROM        dbo.tblDocument sale
                                              INNER JOIN  dbo.tblLineItems saleItems ON sale.Id = saleItems.DocumentID
                                          WHERE     (sale.DocumentTypeId = 1)
                                              AND ((sale.OrderOrderTypeId = 1) OR (sale.OrderOrderTypeId = 3 AND sale.DocumentStatusId = 99))
                                              AND   CONVERT(nvarchar(26),sale.DocumentDateIssued,23) like @TransactionDate
                                              AND ((CONVERT(nvarchar(50),sale.DocumentIssuerCostCentreId) like ISNULL(@salesmanId,'%')) or 
                                                   (CONVERT(nvarchar(50),sale.DocumentRecipientCostCentre) like ISNULL(@salesmanId,'%')))
                                          GROUP BY sale.SaleDiscount);
         RETURN(@TotalSaleValueDiscount);
END;
GO



-- SELECT [dbo].[fn_D_GetSaleValueDiscount] (NULL, '6466992D-B8DC-4E64-81A9-FD90D8497577','2013-11-18')
--  select dbo.fn_D_GetSaleValueDiscount(NULL,NULL,'2013-11-18')



          