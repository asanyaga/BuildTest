IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_D_PopulateStockistSalesman]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[usp_D_PopulateStockistSalesman]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_D_PopulateStockistSalesman]
AS
SELECT ' ALL' StockistSalesmanId,' ALL' StockistSalesmanName
UNION ALL
SELECT CONVERT(NVARCHAR(50),stockistSalesman.id) StockistSalesmanId,
       stockistSalesman.Name StockistSalesmanName
FROM tblCostCentre stockistSalesman
WHERE stockistSalesman.CostCentreType = 4
  AND stockistSalesman.CostCentreType2 = 2


-- EXEC usp_D_PopulateStockistSalesman
