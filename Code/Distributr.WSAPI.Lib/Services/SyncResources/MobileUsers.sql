declare @costcentreid uniqueidentifier ;
set @costcentreid='{0}';
declare @otherypeid int ;
set @otherypeid='{1}';
if(@otherypeid=0)
	begin
		select u.id from tblCostCentre cc
		right join tblUsers  u on cc.id=u.CostCenterId
		where  cc.id=@costcentreid
	end
else if(@otherypeid=1)
	begin
		select u.id from tblCostCentre cc
		right join tblUsers  u on cc.id=u.CostCenterId
		where  cc.id=@costcentreid
		union all
		select u.id from tblUsers u	
	   join tblCostCentre cc on cc.Id=u.CostCenterId	
		where  cc.CostCentreType2=2 and  u.CostCenterId 
		in (select sr.SalemanId from tblSalemanRoute sr where sr.RouteId 
        in(select r.RouteId from tblSalemanRoute r where r.SalemanId=@costcentreid)	)
	end
	
else if(@otherypeid=2)
	begin
		select u.id from tblCostCentre cc
		right join tblUsers  u on cc.id=u.CostCenterId
		where  cc.id=@costcentreid
		union all
		select u.id from tblUsers u	
	   join tblCostCentre cc on cc.Id=u.CostCenterId	
		where  cc.CostCentreType2=1 and  u.CostCenterId 
		in (select sr.SalemanId from tblSalemanRoute sr where sr.RouteId 
        in(select r.RouteId from tblSalemanRoute r where r.SalemanId=@costcentreid)	)
	end
	
		
