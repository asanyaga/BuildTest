
if not exists(select column_name from information_schema.columns where table_name='tblDocument' and column_name='SaleDiscount')
	alter table tblDocument add SaleDiscount money
	
if not exists(select column_name from information_schema.columns where table_name='tblLineItems' and column_name='ProductDiscount')
	alter table tblLineItems add ProductDiscount money