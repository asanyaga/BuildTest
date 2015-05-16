select inv.id from tblCostCentre cc
 join tblInventory  inv on cc.id=inv.WareHouseId
 join tblProduct p on p.id=inv.ProductId
where  (cc.id='{0}' or cc.ParentCostCentreId='{0}')
and p.IM_Status=1