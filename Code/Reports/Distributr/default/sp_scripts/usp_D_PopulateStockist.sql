IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_D_PopulateStockist]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[usp_D_PopulateStockist]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_D_PopulateStockist]
AS
SELECT ' ALL' StockistId,' ALL' StockistName
UNION ALL
SELECT CONVERT(NVARCHAR(50),stockist.id) StockistId,
       stockist.Name StockistName
FROM tblCostCentre stockist
WHERE stockist.CostCentreType = 4
  AND stockist.CostCentreType2 = 1


-- EXEC usp_D_PopulateStockist
