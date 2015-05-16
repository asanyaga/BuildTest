;with cte as (
select  (select ISNULL(sum(rl.Value ),0) as InvoiceAmount
		  from tblDocument r
		  join tblLineItems rl on rl.DocumentID=r.id
		  where r.DocumentTypeId=8 and r.DocumentParentId=o.id
		) as ReceiptAmount,
		(select ISNULL(sum((cl.Quantity * cl.Value) +(cl.Vat * cl.Quantity)),0) as CreditAmount
		  from tblDocument c
		  join tblLineItems cl on cl.DocumentID=c.id
		  where c.DocumentTypeId=10 and c.DocumentParentId=o.Id
		  ) CreditAmount
 from tblDocument o
 where o.DocumentTypeId=1
 and o.Id=o.OrderParentId 
  and o.id='{0}'
 )
 select c.ReceiptAmount+c.CreditAmount from cte c   
