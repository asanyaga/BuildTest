DROP PROCEDURE [dbo].[sp_D_DSS_Payment]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[sp_D_DSS_Payment]
(
@TransactionDate as datetime,
@salesmanId AS NVARCHAR(50),
@distributorId as nvarchar(50),
@outletId AS NVARCHAR(50)
)
as 
if ltrim(rtrim(@outletId))='ALL'  begin set @outletId='%' end
if ltrim(rtrim(@distributorId))='ALL'  begin set @distributorId='%' end
if ltrim(rtrim(@salesmanId))='ALL'  begin set @salesmanId='%' end
SELECT Orders.Id,
       Orders.DocumentReference,
       Orders.DocumentDateIssued,
       case when dbo.tblLineItems.Receipt_PaymentTypeId = 1 then dbo.tblLineItems.Value  end as CashFromSale ,
       case when dbo.tblLineItems.Receipt_PaymentTypeId = 2 then dbo.tblLineItems.Value  end as ChequeFromSale ,
       case when dbo.tblLineItems.Receipt_PaymentTypeId = 3 then dbo.tblLineItems.Value  end as MmoneyFromSale  
           
     
      
FROM       dbo.tblDocument AS Orders 
INNER JOIN dbo.tblDocument AS Invoice ON Orders.Id = Invoice.InvoiceOrderId 
INNER JOIN dbo.tblDocument AS Receipts ON Invoice.Id = Receipts.InvoiceOrderId 
INNER JOIN dbo.tblLineItems ON Receipts.Id = dbo.tblLineItems.DocumentID 
INNER JOIN dbo.tblCostCentre ON Orders.OrderIssuedOnBehalfOfCC = dbo.tblCostCentre.Id 
INNER JOIN dbo.tblCostCentre AS Distributors ON Orders.DocumentIssuerCostCentreId = Distributors.Id OR Orders.DocumentRecipientCostCentre = Distributors.Id 
INNER JOIN dbo.tblCostCentre AS Salesmen ON Orders.DocumentRecipientCostCentre = Salesmen.Id OR Orders.DocumentIssuerCostCentreId = Salesmen.Id 
INNER JOIN dbo.tblCostCentre AS Hq ON Distributors.ParentCostCentreId = Hq.Id 
INNER JOIN dbo.tblRegion AS Regions ON Distributors.Distributor_RegionId = Regions.id 
INNER JOIN dbo.tblCountry AS Country ON Regions.Country = Country.id
WHERE     (Invoice.DocumentTypeId = 5) 
      AND (Receipts.DocumentTypeId = 8)
      AND (Salesmen.CostCentreType = 4)
      AND (Distributors.CostCentreType = 2) 
      --AND (CONVERT(NVARCHAR(26),Orders.DocumentDateIssued, 23) BETWEEN  @TransactionDate AND  @TransactionDate) 
      AND (CONVERT(NVARCHAR(26),Receipts.DocumentDateIssued, 23) BETWEEN  @TransactionDate AND  @TransactionDate) 

      AND convert(nvarchar(50),Distributors.Id) like ISNULL(@distributorId ,'%')
      AND convert(nvarchar(50),Salesmen.Id) like ISNULL(@salesmanId ,'%')
      AND convert(nvarchar(50),Orders.OrderIssuedOnBehalfOfCC) like ISNULL(@outletId ,'%')
    
      AND ((Orders.DocumentTypeId = 1) AND (Orders.OrderOrderTypeId = 1) OR (Orders.OrderOrderTypeId = 3 AND Orders.DocumentStatusId = 99))

-- Exec [dbo].[sp_D_DSS_Payment] @TransactionDate = '2013-10-31', @salesmanId = '382acae3-1d88-4765-b998-122d199243f2',@outletId = 'a6f4ff50-c931-4a98-b7bd-a8705fda6b8e'

-- Exec [dbo].[sp_D_DSS_Payment] @TransactionDate = '2015-04-13', @salesmanId = 'ALL',@outletId = 'ALL', @distributorId='ALL'


GO
