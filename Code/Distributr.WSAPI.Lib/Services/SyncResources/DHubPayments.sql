select pt.id from tblCostCentre cc
 join tblPaymentTracker  pt on cc.id=pt.costcenterId
where  (cc.id='{0}' or cc.ParentCostCentreId='{0}')