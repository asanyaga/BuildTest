SELECT t.id
  FROM [dbo].[tblTarget] t
  left join tblCostCentre cc on cc.id=t.CostCentreId
  where cc.ParentCostCentreId='{0}'