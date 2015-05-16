--;with cte as (select  o.Id, 
-- o.DocumentDateIssued,
-- o.DocumentReference,
-- o.SaleDiscount ,
-- (select ISNULL(sum(round((il.Quantity * il.Value) + (il.Vat * il.Quantity),2,1)),0) as InvoiceAmount
--  from tblDocument i
--  join tblLineItems il on il.DocumentID=i.id
--  where i.DocumentTypeId=5 and i.DocumentParentId=o.id) InvoiceAmount,
-- (select ISNULL(sum(rl.Value ),0) as InvoiceAmount
--  from tblDocument r
--  join tblLineItems rl on rl.DocumentID=r.id
--  where r.DocumentTypeId=8 and r.DocumentParentId=o.id) as ReceiptAmount,
--    (select ISNULL(sum((cl.Quantity * cl.Value) +(cl.Vat * cl.Quantity)),0) as CreditAmount
--  from tblDocument c
--  join tblLineItems cl on cl.DocumentID=c.id
--  where c.DocumentTypeId=10 and c.DocumentParentId=o.Id) CreditAmount
-- from tblDocument o
--where o.DocumentTypeId=1

-- and o.Id=o.OrderParentId 
-- and o.DocumentDateIssued between '{0}' and '{1}'

-- )
-- select c.Id from cte c   
--where (c.SaleDiscount +c.ReceiptAmount  + c.CreditAmount) < c.InvoiceAmount and (c.InvoiceAmount > 0)

;with cte as (select  o.Id, 
 o.DocumentDateIssued,
 o.DocumentReference,
 o.SaleDiscount ,
 (select ISNULL(sum(round((il.Quantity * il.Value) + (il.Vat * il.Quantity),2,1)),0) as InvoiceAmount
  from tblDocument i
  join tblLineItems il on il.DocumentID=i.id
  where i.DocumentTypeId=5 and i.DocumentParentId=o.id) InvoiceAmount,
 (select ISNULL(sum(rl.Value ),0) as InvoiceAmount
  from tblDocument r
  join tblLineItems rl on rl.DocumentID=r.id
  where r.DocumentTypeId=8 and r.DocumentParentId=o.id) as ReceiptAmount,
    (select ISNULL(sum((cl.Quantity * cl.Value) +(cl.Vat * cl.Quantity)),0) as CreditAmount
  from tblDocument c
  join tblLineItems cl on cl.DocumentID=c.id
  where c.DocumentTypeId=10 and c.DocumentParentId=o.Id) CreditAmount
 from tblDocument o
where o.DocumentTypeId=1

 and o.Id=o.OrderParentId 
 and o.DocumentDateIssued between '{0}' and '{1}'

 )
 select c.Id  from cte c   
where c.ReceiptAmount  < --(c.InvoiceAmount-c.SaleDiscount-c.CreditAmount)
CASE WHEN (CONVERT(DECIMAL(18,2),ROUND((c.InvoiceAmount-c.SaleDiscount-c.CreditAmount),2,1))%1) >= 0.04 THEN CEILING((c.InvoiceAmount-c.SaleDiscount-c.CreditAmount)) ELSE FLOOR ((c.InvoiceAmount-c.SaleDiscount-c.CreditAmount)) END 
and (c.InvoiceAmount > 0)
