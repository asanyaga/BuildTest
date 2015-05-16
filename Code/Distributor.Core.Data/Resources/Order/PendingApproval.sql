declare @startdate  datetime,@enddate  datetime;
declare @ordertypeId  int,@pageStart int,@pageEnd int;
set @pageStart='{0}';
set @pageEnd='{1}';
set @ordertypeId='{2}';
set @startdate='{3}';
set @enddate='{4}'

;with cteorder as (
 select ROW_NUMBER() Over (order by o.DocumentDateIssued desc) as [Row],COUNT(*) OVER() AS [RowCount], o.* from tblDocument o 
 where o.DocumentTypeId=1
  and o.DocumentStatusId=1  
  and o.OrderOrderTypeId =@ordertypeId
  and o.Id=o.OrderParentId 
  and o.DocumentDateIssued between @startdate and @enddate
 
 ),
  cte as (select o.[Row],o.[RowCount],  o.Id as OrderId, 
 o.DocumentDateIssued as [Required],
 o.DocumentReference as OrderReference,
 o.ExtDocumentReference as ExternalRefNo,
 ISNULL(o.OrderIssuedOnBehalfOfCC,'00000000-0000-0000-0000-000000000000') as OutletId,
 o.DocumentStatusId as [Status],
  ISNULL((select  Id from tblCostCentre where CostCentreType=4 and  (o.DocumentIssuerCostCentreId=id or o.DocumentRecipientCostCentre=id)),'00000000-0000-0000-0000-000000000000')as SalesmanId,
 (select Name from tblCostCentre where CostCentreType=4 and  (o.DocumentIssuerCostCentreId=id or o.DocumentRecipientCostCentre=id))as Salesman,
 (select Name from tblCostCentre where CostCentreType=2 and  (o.DocumentIssuerCostCentreId=id or o.DocumentRecipientCostCentre=id))as Distributor,
 (select Name from tblCostCentre where CostCentreType=5 and  o.OrderIssuedOnBehalfOfCC=id )as Outlet,

 o.SaleDiscount, 
  ISNULL((select ISNULL(sum(round((il.Quantity * il.Value) + (il.Vat * il.Quantity),2,1)),0) 
 from tblDocument i
    join tblLineItems il on il.DocumentID=i.id
     where i.DocumentTypeId=1 and i.DocumentParentId=o.id)-o.SaleDiscount,0) as NetAmount, --InvoiceAmount
(SELECT ISNULL(SUM(l.Vat * l.Quantity),0)  FROM tblDocument d
 JOIN tblLineItems l ON l.DocumentID = d.Id
 where d.DocumentTypeId=1 and d.DocumentParentId=o.id) AS TotalVat,
(select ISNULL(sum(round((il.Quantity * il.Value) + (il.Vat * il.Quantity),2,1)),0) 
 from tblDocument i
 join tblLineItems il on il.DocumentID=i.id
 where i.DocumentTypeId=1 and i.DocumentParentId=o.id) as GrossAmount,
 (select ISNULL(sum(rl.Value ),0)
  from tblDocument r
  join tblLineItems rl on rl.DocumentID=r.id
  where r.DocumentTypeId=8 and r.DocumentParentId=o.id) as PaidAmount,
    (select ISNULL(sum((cl.Quantity * cl.Value) +(cl.Vat * cl.Quantity)),0)
  from tblDocument c
  join tblLineItems cl on cl.DocumentID=c.id
  where c.DocumentTypeId=10 and c.DocumentParentId=o.Id) as CreditAmount
 from cteorder o
  where o.Row between @pageStart and @pageEnd 

 )

 select * from cte c  order by c.[Row] asc