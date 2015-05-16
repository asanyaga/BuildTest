select con.id from tblSalemanRoute sr
right join tblCostCentre cc on sr.RouteId=cc.RouteId
right join tblContact  con on cc.id=con.CostCenterId
where  sr.SalemanId='{0}'