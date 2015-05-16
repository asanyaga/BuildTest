
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_D_SVD]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[usp_D_SVD]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_D_SVD]
(
@startDate as datetime,
@endDate as datetime,
@distributorId nvarchar(50),
@salesmanId nvarchar(50),
@countryId nvarchar(50),
@regionId nvarchar(50),
@outletId nvarchar(50),
@routeId nvarchar(50),
@brandId nvarchar(50),
@subBrandId nvarchar(50),
@productId nvarchar(50),
@productTypeId nvarchar(50)
)
AS 
--SET @distributorID='ALL' SET @salesmanId='ALL' SET @countryId='ALL' SET @regionId='ALL'	SET	@outletId='ALL'	SET	@routeId='ALL' SET	@brandId='ALL' SET	@subBrandId='ALL' SET	@productId='ALL' SET	@productTypeId='ALL'
		

IF ltrim(rtrim(@brandId))='ALL' AND ltrim(rtrim(@productId))='ALL' AND ltrim(rtrim(@subBrandId))='ALL' AND ltrim(rtrim(@productTypeId))='ALL'
	Exec [dbo].[usp_D_SaleValueDiscount] @startDate, @endDate,@distributorId,@salesmanId,@countryId,@regionId,@outletId,@routeId,@brandId,@subBrandId,@productId,@productTypeId
ELSE
	PRINT 0

GO
-- EXEC [dbo].[usp_D_SaleValueDiscount] @startDate ='2015-1-22',@endDate ='2015-01-22',@distributorID=' ALL',@salesmanId=' ALL',@countryId=' ALL',@regionId=' ALL',@outletId=' ALL',@routeId=' ALL',@brandId=' ALL',@subBrandId=' ALL',@productId=' ALL',@productTypeId=' ALL'
