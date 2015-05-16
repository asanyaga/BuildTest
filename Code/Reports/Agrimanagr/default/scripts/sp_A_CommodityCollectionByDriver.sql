IF NOT EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'sp_A_CommodityCollectionByDriver')
   exec('CREATE PROCEDURE [sp_A_CommodityCollectionByDriver] AS BEGIN SET NOCOUNT ON; END')
GO
Alter PROCEDURE sp_A_CommodityCollectionByDriver
    @startDate datetime,
    @endDate datetime,
    @hubId varchar(50)=NULL,
	@driverId varchar(50)=NULL
    
AS
BEGIN
    
    IF(@hubId='ALL' or @hubId='')	SET @hubId=Null;
	IF(@driverId='ALL' or @driverId='')	SET @driverId=Null;
    --IF(isdate(@startDate)=0) SET @startDate=null;
	--IF(isdate(@endDate)=0)	SET @endDate=null;
	select  convert(varchar(50),doc.DriverName) as driverName, 
	(select convert(varchar(50), Id) from tblUsers where UserName = doc.DriverName) as driverId,
	sum(item.weight) as TotalWeight,
	SUM(convert(int,item.NoOfContainer)) as TotalContainers
	from tblcostcentre cc 
	left JOIN tblSourcingDocument  doc ON cc.Id = doc.DocumentIssuerCostCentreId or  cc.Id =DocumentRecipientCostCentreId
	left JOIN tblSourcingLineItem  item ON doc.Id = item.DocumentId 
	left join tblUsers usr on usr.UserName = doc.drivername
	where cc.costcentretype= 8 and doc.DocumentTypeId = 16
	AND convert(nvarchar(26),doc.DocumentDate,23) between @startDate and @endDate
	--and  ((@startDate is null or @endDate is null) or doc.DocumentDate between @startDate and  @endDate)
	and  (@hubId is null  or cc.id=@hubId)	
	and  (@driverId is null  or convert(varchar(50),usr.Id)=@driverId)
	Group by  doc.DriverName
END;

--EXEC sp_A_CommodityCollectionByDriver @startDate = '10-Jan-2013', @endDate = '10-Dec-2013',@routeid='    '
--EXEC sp_A_CommodityCollectionByDriver @startDate = '     ', @endDate = '',@centreId='    '
--EXEC sp_A_CommodityCollectionByDriver 
--EXEC sp_A_CommodityCollectionByDriver @startDate = '10-Jan-2015', @endDate = '10-Dec-2015' ,@centreId='B4E16809-E97F-4E77-BC0F-E77A0946B941'

-- EXEC sp_A_CommodityCollectionByDriver @startDate = '10-Jan-2015', @endDate = '10-Dec-2015' ,@driverId='d4014658-d7a6-4410-b5ad-8a51151190df'
