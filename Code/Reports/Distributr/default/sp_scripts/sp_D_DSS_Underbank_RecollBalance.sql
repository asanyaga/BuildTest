DROP PROCEDURE [dbo].[sp_D_DSS_Underbank_RecollBalance]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[sp_D_DSS_Underbank_RecollBalance]
(
@TransactionDate as datetime,
@distributorId as nvarchar(50),
@salesmanId AS NVARCHAR(50),
@outletId AS NVARCHAR(50)
)
as 
if ltrim(rtrim(@outletId))='ALL'  begin set @outletId='%' end
if ltrim(rtrim(@distributorId))='ALL'  begin set @distributorId='%' end
if ltrim(rtrim(@salesmanId))='ALL'  begin set @salesmanId='%' end


SELECT 
         dbo.udf_D_RoundOff(COALESCE(rec.Amount,0)- COALESCE(recItem.Amount,0)) AS RecollectionBalance
FROM         dbo.tblRecollection rec 
LEFT OUTER JOIN   dbo.tblRecollectionItem recItem ON rec.Id = recItem.RecollectionId 
INNER JOIN   dbo.tblCostCentre salesman ON rec.CostCentreId = salesman.Id
Inner join dbo.tblCostCentre distributor on salesman.ParentCostCentreId = distributor.Id
INNER JOIN   dbo.tblCostCentre outlet  ON rec.FromCostCentreId = outlet.Id
WHERE
     convert(nvarchar(50),Salesman.Id) like isnull(@salesmanId,'%')
 and convert(nvarchar(50),distributor.Id) like isnull(@distributorId,'%')
 and convert(nvarchar(50),outlet.Id) like isnull(@outletId,'%')
 and convert(nvarchar(26),rec.DateInserted,23) <= @TransactionDate
-- and convert(nvarchar(26),rec.DateInserted,23) between '2013-12-10' and '2013-12-10'  
GROUP BY rec.Id,rec.Description,rec.Amount,recItem.DateInserted,rec.IsReceived,recItem.Amount,rec.DateInserted,recItem.RecollectionId,recItem.IsComfirmed,recItem.CollectionModeId,salesman.Id,salesman.Name,outlet.Id,outlet.Name,outlet.Cost_Centre_Code
HAVING ((recItem.Amount) < (rec.Amount) ) or (recItem.Amount is null)
 
 
-- Exec [dbo].[sp_D_DSS_Underbank_RecollBalance] @TransactionDate='2015-04-24', @salesmanId = 'ALL',@outletId = 'ALL',@distributorId='ALL'


go