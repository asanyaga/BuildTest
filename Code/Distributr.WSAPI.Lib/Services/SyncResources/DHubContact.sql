select con.id from tblCostCentre cc
right join tblContact  con on cc.id=con.CostCenterId
where  (cc.id='{0}' or cc.ParentCostCentreId='{0}')
