DROP PROCEDURE [dbo].[sp_D_DSS_DaysRecollectionReturns]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[sp_D_DSS_DaysRecollectionReturns]
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

SELECT  
         COALESCE(recItem.Amount,0) AS RecollectedAmount

FROM         dbo.tblRecollection rec 
LEFT OUTER JOIN   dbo.tblRecollectionItem recItem ON rec.Id = recItem.RecollectionId 
INNER JOIN   dbo.tblCostCentre salesman ON rec.CostCentreId = salesman.Id
Inner join dbo.tblCostCentre distributor on salesman.ParentCostCentreId = distributor.Id
INNER JOIN   dbo.tblCostCentre outlet  ON rec.FromCostCentreId = outlet.Id
WHERE --(recItem.Amount < rec.Amount or recItem.Amount = 0)
     convert(nvarchar(50),Salesman.Id) like isnull(@salesmanId,'%')
 and convert(nvarchar(50),distributor.Id) like isnull(@distributorId,'%')
 and convert(nvarchar(50),outlet.Id) like isnull(@outletId,'%')
 and CONVERT(nvarchar(26),rec.DateInserted,23) between @TransactionDate and @TransactionDate 
GROUP BY rec.Id,rec.Description,rec.Amount,recItem.DateInserted,rec.IsReceived,recItem.Amount,rec.DateInserted,recItem.RecollectionId,recItem.IsComfirmed,recItem.CollectionModeId,salesman.Id,salesman.Name,outlet.Id,outlet.Name,outlet.Cost_Centre_Code
--HAVING ((recItem.Amount) < (rec.Amount) ) or (recItem.Amount is null)
 
-- Exec [dbo].[sp_D_DSS_DaysRecollectionReturns] @TransactionDate = '2013-10-31', @salesmanId = '382acae3-1d88-4765-b998-122d199243f2',@outletId = 'a6f4ff50-c931-4a98-b7bd-a8705fda6b8e'

-- Exec [dbo].[sp_D_DSS_DaysRecollectionReturns] @TransactionDate = '2013-12-10', @salesmanId = 'ALL',@outletId = 'ALL',@distributorId='ALL'







GO
