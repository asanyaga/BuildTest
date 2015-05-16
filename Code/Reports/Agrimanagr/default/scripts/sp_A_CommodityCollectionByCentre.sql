IF NOT EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'sp_A_CommodityCollectionByCentre')
   exec('CREATE PROCEDURE [sp_A_CommodityCollectionByCentre] AS BEGIN SET NOCOUNT ON; END')
GO
Alter PROCEDURE sp_A_CommodityCollectionByCentre
    @startDate datetime,
    @endDate datetime,
    @hubId varchar(50)=NULL,
	@centreId varchar(50)=NULL
    
AS
BEGIN
    IF(@hubId='ALL' or @hubId='')	SET @hubId=Null;
	IF(@centreId='ALL' or @hubId='')	SET @centreId=Null;
 --	IF(isdate(@startDate)=0) SET @startDate=null;
	--IF(isdate(@endDate)=0)	SET @endDate=null;
			
	select 
	(select name from tblcentre where id= doc.centreid) as centreName, 	
	convert(varchar(50),doc.CentreId) as centreId,	
	sum(item.weight) as TotalWeight,
	sUM (convert(int,item.NoOfContainer)) as TotalContainers
	from tblcostcentre cc 
	left JOIN tblSourcingDocument  doc ON cc.Id = doc.DocumentIssuerCostCentreId or  cc.Id =DocumentRecipientCostCentreId
	left JOIN tblSourcingLineItem  item ON doc.Id = item.DocumentId 
	where cc.costcentretype= 8 
	and doc.DocumentTypeId = 16 
	AND convert(nvarchar(26),doc.DocumentDate,23) between @startDate and @endDate	
	--and  ((@startDate is null or @endDate is null) or doc.DocumentDate between @startDate and  @endDate)
	and  (@hubId is null  or cc.id=@hubId)
	and  (@centreId is null  or doc.CentreId=@centreId)	
	group by doc.CentreId
	order by centreName
END;
--EXEC sp_A_CommodityCollectionByCentre @startDate = '10-Jan-2013', @endDate = '10-Dec-2013',@routeid='    '
--EXEC sp_A_CommodityCollectionByCentre @startDate = '     ', @endDate = '',@centreId='    '
--EXEC sp_A_CommodityCollectionByCentre 
--EXEC sp_A_CommodityCollectionByCentre @startDate = '10-Jan-2015', @endDate = '10-Dec-2015' ,@centreId='C6A6ACC0-9148-4302-B07D-DE10988CE275'
--EXEC sp_A_CommodityCollectionByCentre @centreId='B4E16809-E97F-4E77-BC0F-E77A0946B941'