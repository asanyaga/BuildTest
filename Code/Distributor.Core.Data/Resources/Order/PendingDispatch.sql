 Select Distinct o.OrderParentId
 from [tblDocument] as o 
 join tblLineItems items on items.DocumentID=o.Id 
 where o.DocumentDateIssued between '{0}' and '{1}'
 and items.LineItemStatusId=3