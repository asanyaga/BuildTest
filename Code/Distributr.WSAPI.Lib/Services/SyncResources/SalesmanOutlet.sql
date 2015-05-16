select distinct cc.id,cc.Name from tblSalemanRoute sr
right join tblCostCentre cc on sr.RouteId=cc.RouteId
where sr.SalemanId='{0}'