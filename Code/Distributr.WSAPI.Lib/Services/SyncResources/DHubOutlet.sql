select cc.id from tblCostCentre cc
where cc.CostCentreType=5
and (cc.id='{0}' or cc.ParentCostCentreId='{0}')