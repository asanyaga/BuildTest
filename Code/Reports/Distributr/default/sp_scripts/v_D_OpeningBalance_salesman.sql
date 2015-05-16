DROP VIEW [dbo].[v_D_OpeningBalance_salesman]
GO
CREATE VIEW [dbo].[v_D_OpeningBalance_salesman]
AS

select  Sum(i.Balance) Qty,
            i.DateOfEntry saleDate,
            i.WarehouseId CostCentreId,
            sm.Name  Salesman,
			sm.Id  SalesmanId,
			p.id ProductId,
			p.Description ProductName,
			p.ProductCode 
--from dbo.tblInventoryDailySnapshot i
from dbo.tblDailyInventorySnapShot i
 join dbo.tblcostcentre sm on i.WarehouseId = sm.id
 join dbo.tblProduct p on i.ProductId = p.id
where sm.CostCentreType = 4
group by i.DateOfEntry,i.WarehouseId,sm.Name,sm.Id,p.id,p.Description,p.ProductCode

-- select * from v_D_OpeningBalance_salesman

