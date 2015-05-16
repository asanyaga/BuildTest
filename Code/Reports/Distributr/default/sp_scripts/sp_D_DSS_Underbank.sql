DROP PROCEDURE [dbo].[sp_D_DSS_Underbank]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[sp_D_DSS_Underbank]
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

SELECT   rec.Id, 
		rec.DateInserted,
         rec.Description, 
         dbo.udf_D_RoundOff(COALESCE(rec.Amount,0)) AS  Amount,
         rec.IsReceived, 
         COALESCE(recItem.Amount,0) AS RecollectedAmount, 
         recItem.DateInserted as RecollectionDate,
         (SELECT    sum(rec.Amount - recItem.Amount) as RecollectionBalance         
          FROM         dbo.tblRecollection rec 
           INNER JOIN    dbo.tblRecollectionItem recItem ON rec.Id = recItem.RecollectionId 
           INNER JOIN   dbo.tblCostCentre salesman ON rec.CostCentreId = salesman.Id         
           INNER JOIN   dbo.tblCostCentre outlet  ON rec.FromCostCentreId = outlet.Id
          WHERE recItem.Amount < rec.Amount
            and convert(nvarchar(50),Salesman.Id) like isnull(@salesmanId,'%')
            and convert(nvarchar(50),outlet.Id) like isnull(@outletId,'%'))as RecolectionBalance ,
         recItem.IsComfirmed,
         recItem.CollectionModeId as PaymentType, 
         salesman.Id AS SalemanId, 
         salesman.Name as SalesmanName,
         outlet.Id AS CustomerId, 
         outlet.Name AS CustomerName,
         
         outlet.Cost_Centre_Code as CustomerCode,
         case when recItem.CollectionModeId = 1 then recItem.Amount else 0 end  as CashRecollected,
         case when recItem.CollectionModeId = 2 then recItem.Amount else 0 end as ChequeRecollected,
         case when recItem.CollectionModeId = 3 then recItem.Amount else 0 end as MmoneyRecollected,
         case when recItem.CollectionModeId IS NULL then recItem.Amount else 0 end as Undefined
FROM         dbo.tblRecollection rec 
LEFT OUTER JOIN   dbo.tblRecollectionItem recItem ON rec.Id = recItem.RecollectionId 
INNER JOIN   dbo.tblCostCentre salesman ON rec.CostCentreId = salesman.Id
Inner join dbo.tblCostCentre distributor on salesman.ParentCostCentreId = distributor.Id
INNER JOIN   dbo.tblCostCentre outlet  ON rec.FromCostCentreId = outlet.Id
WHERE --(recItem.Amount < rec.Amount or recItem.Amount = 0)
  convert(nvarchar(50),Salesman.Id) like isnull(@salesmanId,'%')
 and convert(nvarchar(50),distributor.Id) like isnull(@distributorId,'%')
 and convert(nvarchar(50),outlet.Id) like isnull(@outletId,'%')
 --and CONVERT(nvarchar(26),rec.DateInserted,23) between @TransactionDate and @TransactionDate  --<<REVISED 24FEB2015 Date Filter Included 
 and CONVERT(nvarchar(26),rec.DateInserted,23) <= @TransactionDate --<<REVISED 2April2015 Date Filter Excluded
GROUP BY rec.Id,rec.DateInserted,rec.Description,rec.Amount,recItem.DateInserted,rec.IsReceived,recItem.Amount,rec.DateInserted,recItem.RecollectionId,recItem.IsComfirmed,recItem.CollectionModeId,salesman.Id,salesman.Name,outlet.Id,outlet.Name,outlet.Cost_Centre_Code
HAVING ((recItem.Amount) < (rec.Amount) ) or (recItem.Amount is null)
ORDER BY recItem.DateInserted DESC
-- Exec [dbo].[sp_D_DSS_Underbank] @TransactionDate = '2013-10-31', @salesmanId = '382acae3-1d88-4765-b998-122d199243f2',@outletId = 'a6f4ff50-c931-4a98-b7bd-a8705fda6b8e'

-- Exec [dbo].[sp_D_DSS_Underbank] @TransactionDate = '2015-4-24', @salesmanId = 'ALL',@outletId = 'ALL',@distributorId='ALL'







GO
