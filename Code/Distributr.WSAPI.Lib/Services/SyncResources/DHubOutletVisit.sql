select ovd.id from tblCostCentre cc
right join tblOutletVisitDay ovd on ovd.OutletId=cc.Id
where cc.CostCentreType=5
and (cc.id='{0}' or cc.ParentCostCentreId='{0}')