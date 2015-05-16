IF NOT EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'sp_A_ProductionByCentre')
   exec('CREATE PROCEDURE [sp_A_ProductionByCentre] AS BEGIN SET NOCOUNT ON; END')
GO
Alter PROCEDURE sp_A_ProductionByCentre
    @startDate datetime,
    @endDate datetime ,
    @hubId varchar(50)=NULL  ,
    @routeId varchar(50)=NULL ,
    @centreId varchar(50)=NULL       
      
AS
-- check calculations (on received)
BEGIN
	declare @tab1 table(
	CentreId varchar(50),
	CentreName varchar(100) ,
	CentreWeight decimal(18,2),
	DeliveredWeight decimal(18,2),
	ReceivedWeight decimal(18,2)
	);
    --PRINT 'startDate ='+ isnull(@startdate,'NULL');
    --PRINT 'endDate ='+ isnull(@enddate,'NULL');   
	--IF(isdate(@startDate)=0) SET @startDate=null;
	--IF(isdate(@endDate)=0)	SET @endDate=null;
	IF(@hubId='ALL' or @hubId='')	SET @hubId=null;
	IF(@routeId='ALL' or @routeId='')	SET @routeId=null;
	IF(@centreId='ALL' or @centreId='')	SET @centreId=null;

insert @tab1 	
select distinct c.id as CentreId,  
                name as CentreName,
            	isnull((select sum(item.weight)  
from tblSourcingDocument doc 
inner join tblSourcingLineItem item on doc.id = item.DocumentId
where doc.DocumentTypeId=13 
  and doc.centreid=c.id
  AND convert(nvarchar(26),doc.DocumentDate,23) between @startDate and @endDate
	 --and  ((@startDate is null or @endDate is null) or doc.DocumentDate between @startDate and  @endDate)
  and  (@routeId is null  or doc.routeId=@routeId)
  and  (@centreId is null  or c.Id=@centreId) --ADDED FILTER
  and  (@hubId is null  or doc.documentRecipientCostCentreId=@hubId or doc.documentIssuerCostCentreId=@hubId )),0) as CentreWeight ,
 isnull((select sum(item.weight)  from tblSourcingDocument doc 
			inner join tblSourcingLineItem item on doc.id = item.DocumentId
	     where doc.DocumentTypeId=16 and doc.centreid=c.id
		 AND convert(nvarchar(26),doc.DocumentDate,23) between @startDate and @endDate
		 --and  ((@startDate is null or @endDate is null) or doc.DocumentDate between @startDate and  @endDate)
		 and  (@routeId is null  or doc.routeId=@routeId)
		 and  (@centreId is null  or c.Id=@centreId) --ADDED FILTER
		 and  (@hubId is null  or doc.documentRecipientCostCentreId=@hubId or doc.documentIssuerCostCentreId=@hubId)),0) as DeliveredWeight ,
  isnull((select sum(item.weight)  from tblSourcingDocument doc 
	 inner join tblSourcingLineItem item on doc.id = item.DocumentId
	 where doc.DocumentTypeId=17 and doc.centreid=c.id
	 AND convert(nvarchar(26),doc.DocumentDate,23) between @startDate and @endDate
	 --and  ((@startDate is null or @endDate is null) or doc.DocumentDate between @startDate and  @endDate)
	 and  (@routeId is null  or doc.routeId=@routeId)
	 and  (@centreId is null  or c.Id=@centreId) --ADDED FILTER
	 and  (@hubId is null  or doc.documentRecipientCostCentreId=@hubId or doc.documentIssuerCostCentreId=@hubId)),0) as ReceivedWeight
  
 from   tblCentre c
 inner join tblSourcingDocument doc on doc.centreid=c.id
 AND convert(nvarchar(26),doc.DocumentDate,23) between @startDate and @endDate
--and  ((@startDate is null or @endDate is null) or doc.DocumentDate between @startDate and  @endDate)
and  (@routeId is null  or doc.routeId=@routeId)
and  (@centreId is null  or c.Id=@centreId) --ADDED FILTER
and  (@hubId is null  or c.hubid=@hubId)

select CentreId,CentreName ,CentreWeight,DeliveredWeight,ReceivedWeight
,(DeliveredWeight-CentreWeight) as ValianceBA 
,(ReceivedWeight-CentreWeight) as ValianceCA 
,((ReceivedWeight-CentreWeight)/NULLIF(CentreWeight,0)) as [CA/A]
from @tab1	
order by CentreName
	
END;


--EXEC sp_A_ProductionByCentre @startDate = '10-Jan-2013', @endDate = '10-Dec-2013',@hubId='ALL'
--EXEC sp_A_ProductionByCentre 
--EXEC sp_A_ProductionByCentre @routeId='9B3FE18E-0BBA-471C-A4C6-96EA9EBD0276' 
--EXEC sp_A_ProductionByCentre @startDate = '10-Jan-2013', @endDate = '10-Dec-2011'
--EXEC sp_A_ProductionByCentre @hubId='79B5E17F-6504-47F7-A431-1A2C6A749A99'

-- EXEC sp_A_ProductionByCentre @startDate = '2015-03-01', @endDate = '2015-03-26', @hubId='ALL'
