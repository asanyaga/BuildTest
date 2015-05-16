
if not exists(select column_name from information_schema.columns where table_name = 'tblCostCentre' and column_name = 'PaybillNumber')
	alter table tblCostCentre add PaybillNumber int
if not exists(select column_name from information_schema.columns where table_name = 'tblCostCentre' and column_name = 'MerchantNumber')
	alter table tblCostCentre add MerchantNumber int