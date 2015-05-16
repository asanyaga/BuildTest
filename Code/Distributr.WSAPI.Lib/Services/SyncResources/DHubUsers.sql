select u.id from tblCostCentre cc
right join tblUsers  u on cc.id=u.CostCenterId
where  (cc.id='{0}' or cc.ParentCostCentreId='{0}')
