;with cte as (select  o.Id, 
 o.DocumentDateIssued as [Required],
 o.DocumentReference,
 o.ExtDocumentReference,
 (select Name from tblCostCentre where CostCentreType=4 and  (o.DocumentIssuerCostCentreId=id or o.DocumentRecipientCostCentre=id))as Salesman,
 (select Name from tblCostCentre where CostCentreType=2 and  (o.DocumentIssuerCostCentreId=id or o.DocumentRecipientCostCentre=id))as Distributor,
 (select Name from tblCostCentre where CostCentreType=5 and  o.OrderIssuedOnBehalfOfCC=id )as Outlet,

 o.SaleDiscount ,
  (select ISNULL(sum((il.Quantity * il.Value)+ (il.Vat * il.Quantity) - i.SaleDiscount),0) 
  from tblDocument i
  join tblLineItems il on il.DocumentID=i.id
  where i.DocumentTypeId=1 and i.DocumentParentId=o.id) as NetAmount, --InvoiceAmount
    (SELECT ISNULL(SUM(l.Vat * l.Quantity),0)
  FROM tblDocument d
  JOIN tblLineItems l ON l.DocumentID = d.Id
  where d.DocumentTypeId=1 and d.DocumentParentId=o.id) AS TotalVat,
  (select ISNULL(sum((il.Quantity * il.Value) + (il.Vat * il.Quantity)),0) 
  from tblDocument i
  join tblLineItems il on il.DocumentID=i.id
  where i.DocumentTypeId=1 and i.DocumentParentId=o.id) as GrossAmount,
 (select ISNULL(sum(rl.Value ),0)
  from tblDocument r
  join tblLineItems rl on rl.DocumentID=r.id
  where r.DocumentTypeId=8 and r.DocumentParentId=o.id) as ReceiptAmount,
    (select ISNULL(sum((cl.Quantity * cl.Value) +(cl.Vat * cl.Quantity)),0)
  from tblDocument c
  join tblLineItems cl on cl.DocumentID=c.id
  where c.DocumentTypeId=10 and c.DocumentParentId=o.Id) as CreditAmount,
  o.DocumentStatusId
 from tblDocument o
where o.DocumentTypeId = 1
and o.DocumentStatusId = 5

 and o.Id=o.OrderParentId 
 and o.DocumentDateIssued between '{0}' and '{1}'
 and o.OrderIssuedOnBehalfOfCC='{2}'
  
 )
 select *,NetAmount - (c.ReceiptAmount  + c.CreditAmount) as OutstandingAmount from cte c  