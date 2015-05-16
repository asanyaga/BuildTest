
if not exists (select column_name from information_schema.columns where table_name = 'tblLineItems' and column_name = 'ProductDiscount')
	alter table tblLineItems add ProductDiscount money
