 Select Distinct o.OrderParentId
 from [tblDocument] as o 
 join tblLineItems items on items.DocumentID=o.Id 
  where o.DocumentTypeId=1 and o.OrderOrderTypeId=2 and o.DocumentDateIssued between '{0}' and '{1}'
 and items.LineItemStatusId=3