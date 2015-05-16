DROP VIEW [dbo].[v_D_OpeningBalance]
GO
CREATE VIEW [dbo].[v_D_OpeningBalance]
AS

select  Sum(i.Balance) OpeningBalance,
            i.DateOfEntry,
            i.WarehouseId CostCentreId,
            dist.Name  CostCentre,
			p.id ProductId,
			p.Description ProductName,
			p.ProductCode 
--from dbo.tblInventoryDailySnapshot i
from dbo.tblDailyInventorySnapShot i
 join dbo.tblcostcentre dist on i.WarehouseId = dist.id
 join dbo.tblProduct p on i.ProductId = p.id
where dist.CostCentreType = 2
group by i.DateOfEntry,i.WarehouseId,dist.Name,p.id,p.Description,p.ProductCode

-- select * from v_D_OpeningBalance