DROP PROCEDURE [dbo].[sp_D_SalesDateException]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[sp_D_SalesDateException]
(
@startDate as datetime,
@endDate as datetime,
@distributorId AS NVARCHAR(50),
@salesmanId AS NVARCHAR(50),
@variance as int,
@filterBy as int
)
as 
if  rtrim(ltrim(@distributorId))='ALL'  begin set @distributorId='%' end
if  rtrim(ltrim(@salesmanId))='ALL'  begin set @salesmanId='%' end
if  @variance=null  begin set @variance='%' end


select  sales.DocumentReference as SalesRef,
        distributor.Name as DistributorName,
        salesman.Name as SalesmanName,
        case when sales.DocumentStatusId = 0 then 'New'  
             when sales.DocumentStatusId = 1 then 'Confirmed' 
             when sales.DocumentStatusId = 4 then 'Approved' 
             when sales.DocumentStatusId = 5 then 'Rejected'
             when sales.DocumentStatusId = 99 then 'Closed'
             else 'Unknown' end as SaleStatus,
       (salesItems.Quantity * (salesItems.Value + salesItems.Vat)) as GrossAmount,
        salesItems.ProductDiscount,
        sales.SaleDiscount,
        convert(nvarchar(26),sales.DocumentDateIssued,23) as SaleDate,
        convert(nvarchar(26),sales.IM_DateCreated,23) as SendDate,
        cast(DATEDIFF(DAY,sales.DocumentDateIssued,sales.IM_DateCreated) as nvarchar(50))+' Days' as Variance
        
from tblDocument sales
inner join tblLineItems salesItems on sales.Id = salesItems.DocumentID
inner join tblCostCentre distributor on (sales.DocumentIssuerCostCentreId = distributor.Id or sales.DocumentRecipientCostCentre = distributor.Id)
inner join tblCostCentre salesman on (sales.DocumentIssuerCostCentreId = salesman.Id or sales.DocumentRecipientCostCentre = salesman.Id)
where DocumentTypeId = 1 
and (OrderOrderTypeId = 1 or (OrderOrderTypeId = 3 or DocumentTypeId = 99))
and distributor.CostCentreType = 2
and salesman.CostCentreType = 4
and ((@variance is not null and DATEDIFF(DAY,sales.DocumentDateIssued,sales.IM_DateCreated) = @variance) or
     (@variance is null and DATEDIFF(DAY,sales.DocumentDateIssued,sales.IM_DateCreated)>= 0))
and CONVERT(nvarchar(50),distributor.Id) like ISNULL(@distributorId,'%')
and CONVERT(nvarchar(50),salesman.Id) like ISNULL(@salesmanId,'%')
and((@filterBy = 1 and CONVERT(nvarchar(26),sales.DocumentDateIssued,23) between @startDate and @endDate) 
 or (@filterBy = 0 and CONVERT(nvarchar(26),sales.IM_DateCreated,23) between @startDate and @endDate))

AND   DATEDIFF(DAY,sales.DocumentDateIssued,sales.IM_DateCreated) <>  0

-- Exec   [dbo].[sp_D_SalesDateException] @distributorId='ALL' , @salesmanId='ALL' ,@startDate='2013-01-23', @endDate='2013-11-28',@variance=0 , @filterBy = 0

order by sales.DocumentDateIssued desc