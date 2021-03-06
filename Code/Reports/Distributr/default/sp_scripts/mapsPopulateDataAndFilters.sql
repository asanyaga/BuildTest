
/****** Object:  View [dbo].[vLocationResults]    Script Date: 07/18/2014 14:01:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
DROP VIEW [dbo].[vLocationResults]
GO
CREATE VIEW [dbo].[vLocationResults]
AS
SELECT     TOP (100) PERCENT ROW_NUMBER() OVER (ORDER BY dbo.tblDocument.DocumentDateIssued ASC) AS Row,dbo.tblCostCentre.Id as ResultID, tblDistributor.Id AS DistributorId, tblDistributor.Name AS Distributor, 
tblDistributor.Id AS OutletId, dbo.tblCostCentre.Name AS Outlet, dbo.tblLineItems.Quantity * (dbo.tblLineItems.Value + dbo.tblLineItems.Vat) AS SaleAmount, dbo.tblDocument.DocumentReference, 
dbo.tblDocument.DocumentDateIssued, 
isnull(dbo.tblDocument.Latitude, '') AS Latitude, 
                isnull(dbo.tblDocument.Longitude, '') AS Longitude,
 dbo.tblRoutes.RouteID AS RouteID, 
dbo.tblRoutes.Name AS Route, tblCostCentre_3.Name AS Salesman, tblCostCentre_3.Id AS SalesmanID, tblCostCentre_3.CostCentreType AS SalesmanTypeID, dbo.tblRoutes.RouteID AS RouteIDroutesTable, 
dbo.tblLineItems.ProductDiscount, dbo.tblRegion.Name AS Region, dbo.tblCountry.Name AS Country, dbo.tblDocument.SaleDiscount, 
dbo.tblLineItems.ProductDiscount * dbo.tblLineItems.Quantity AS TotalProductDiscount
FROM         dbo.tblCostCentre AS tblDistributor INNER JOIN
                      dbo.tblCostCentre ON tblDistributor.Id = dbo.tblCostCentre.ParentCostCentreId INNER JOIN
                      dbo.tblDocument INNER JOIN
                      dbo.tblLineItems ON dbo.tblDocument.Id = dbo.tblLineItems.DocumentID ON dbo.tblCostCentre.Id = dbo.tblDocument.OrderIssuedOnBehalfOfCC INNER JOIN
                      dbo.tblRoutes ON dbo.tblCostCentre.RouteId = dbo.tblRoutes.RouteID INNER JOIN
                      dbo.tblCostCentre AS tblCostCentre_3 ON dbo.tblDocument.DocumentRecipientCostCentre = tblCostCentre_3.Id INNER JOIN
                      dbo.tblRegion ON dbo.tblRoutes.RegionId = dbo.tblRegion.id INNER JOIN
                      dbo.tblCountry ON dbo.tblRegion.Country = dbo.tblCountry.id
WHERE     (dbo.tblDocument.DocumentTypeId = 1)
 --AND (dbo.tblDocument.OrderOrderTypeId = 1 OR   dbo.tblDocument.OrderOrderTypeId = 3)  AND (dbo.tblDocument.DocumentStatusId = 99)
 AND (dbo.tblCostCentre.CostCentreType = 5)
ORDER BY dbo.tblDocument.DocumentDateIssued
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[13] 4[13] 2[55] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
      Begin ColumnWidths = 12
         Width = 284
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'vLocationResults'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'vLocationResults'
GO
/****** Object:  StoredProcedure [dbo].[map_spGetSalesMen]    Script Date: 07/18/2014 14:01:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
DROP PROCEDURE [dbo].[map_spGetSalesMen]
GO
CREATE procedure [dbo].[map_spGetSalesMen](
	@sDate as datetime,
	@eDate as datetime,
	@uDistributor varchar(200)
)
as 


SELECT distinct [Salesman] , [SalesmanID] FROM [vLocationResults]
where (Latitude <> '' and Longitude <> '')
AND DistributorId = (case when @uDistributor = 'ALL' then DistributorId else @uDistributor end )
AND DocumentDateIssued between @sDate and @eDate
GO
/****** Object:  StoredProcedure [dbo].[map_spGetRoutes]    Script Date: 07/18/2014 14:01:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
DROP PROCEDURE [dbo].[map_spGetRoutes]
GO
CREATE procedure [dbo].[map_spGetRoutes](
	@sDate as datetime,
	@eDate as datetime,
	@uDistributor varchar(200),
	@uSalesman varchar(200)
)
as 

SELECT distinct [RouteID] ,[Route] FROM [vLocationResults]
where (Latitude <> '' and Longitude <> '')
AND DistributorId = (case when @uDistributor = 'ALL' then DistributorId else @uDistributor end )
AND SalesmanID = (case when @uSalesman = 'ALL' then SalesmanID else @uSalesman end )
AND DocumentDateIssued between @sDate and @eDate
GO
/****** Object:  StoredProcedure [dbo].[map_spGetOutletTransactions]    Script Date: 07/18/2014 14:01:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
DROP PROCEDURE [dbo].[map_spGetOutletTransactions]
GO
CREATE procedure [dbo].[map_spGetOutletTransactions](
	@sDate as datetime,
	@eDate as datetime,
	@sResultID varchar(200)
)
as
 
-- ResultID = Outlet ID 

SELECT ROW_NUMBER() OVER (ORDER BY DocumentDateIssued ASC) as row , ResultID , SaleAmount , DocumentReference , DocumentDateIssued , 
Outlet , Salesman ,
SaleDiscount , ProductDiscount from dbo.vLocationResults
where (Latitude <> '' and Longitude <> '')
AND ResultID = @sResultID
AND DocumentDateIssued between @sDate and @eDate
GO
/****** Object:  StoredProcedure [dbo].[map_spGetDistributors]    Script Date: 07/18/2014 14:01:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
DROP PROCEDURE [dbo].[map_spGetDistributors]
GO
CREATE procedure [dbo].[map_spGetDistributors](
	@sDate as datetime,
	@eDate as datetime
)
as 


SELECT distinct [DistributorId] ,[Distributor] FROM [vLocationResults]
where (Latitude <> '' and Longitude <> '')
AND DocumentDateIssued between @sDate and @eDate
GO
/****** Object:  StoredProcedure [dbo].[map_GetSalesPoints]    Script Date: 07/18/2014 14:01:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
DROP PROCEDURE [dbo].[map_GetSalesPoints]
GO
CREATE procedure [dbo].[map_GetSalesPoints](
	@sDate as datetime,
	@eDate as datetime,
	@uDistributor varchar(200),
	@uSalesman varchar(200),
	@uRoute varchar(200),
	@uOutlet varchar(200)
)
as 

SELECT ResultID , DistributorId ,Distributor ,OutletId ,Outlet ,SaleAmount ,DocumentReference ,DocumentDateIssued ,
Latitude ,Longitude ,RouteID ,Route ,Salesman ,SalesmanID ,SalesmanTypeID ,RouteIDroutesTable ,
ProductDiscount ,Region ,Country ,SaleDiscount ,TotalProductDiscount
FROM vLocationResults
WHERE (Latitude <> '' and Longitude <> '')
AND DistributorId = (case when @uDistributor = 'ALL' then DistributorId else @uDistributor end )
AND SalesmanID = (case when @uSalesman = 'ALL' then SalesmanID else @uSalesman end )
AND RouteID = (case when @uRoute = 'ALL' then RouteID else @uRoute end )
AND OutletId = (case when @uOutlet = 'ALL' then OutletId else @uOutlet end )
AND DocumentDateIssued between @sDate and @eDate
order by DocumentDateIssued asc
GO
/****** Object:  StoredProcedure [dbo].[map_GetOutlets]    Script Date: 07/18/2014 14:01:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
DROP PROCEDURE [dbo].[map_GetOutlets]
GO
CREATE procedure [dbo].[map_GetOutlets](
	@sDate as datetime,
	@eDate as datetime,
	@uDistributor varchar(200),
	@uSalesman varchar(200),
	@uRoute varchar(200)
)
as 

SELECT distinct [OutletId] ,[Outlet] FROM [vLocationResults]
WHERE (Latitude <> '' and Longitude <> '')
AND DistributorId = (case when @uDistributor = 'ALL' then DistributorId else @uDistributor end )
AND SalesmanID = (case when @uSalesman = 'ALL' then SalesmanID else @uSalesman end )
AND RouteID = (case when @uRoute = 'ALL' then RouteID else @uRoute end )
AND DocumentDateIssued between @sDate and @eDate
GO
