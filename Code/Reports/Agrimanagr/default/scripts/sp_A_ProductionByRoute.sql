IF NOT EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'sp_A_ProductionByRoute')
   exec('CREATE PROCEDURE [sp_A_ProductionByRoute] AS BEGIN SET NOCOUNT ON; END')
GO
Alter PROCEDURE sp_A_ProductionByRoute
    @startDate datetime ,
    @endDate datetime,
    @hubId varchar(50)=NULL,
    @routeId varchar(50)=NULL   
       
AS
-- check calculations (on received)
BEGIN
	declare @regionId varchar(50)=null;
	declare @tab1 table(
	 RouteId varchar(50),
	 Name varchar(100) ,
	 CentreWeight decimal(18,2),
	 DeliveredWeight decimal(18,2),
	 ReceivedWeight decimal(18,2)
	);
 --   PRINT 'startDate ='+ isnull(@startdate,'NULL');
 --   PRINT 'endDate ='+ isnull(@enddate,'NULL'); 
      
	--IF(isdate(@startDate)=0) SET @startDate=null;
	--IF(isdate(@endDate)=0)	SET @endDate=null;
	IF(@hubId='ALL' or @hubId='')	SET @hubId=null;
	IF(@routeId='ALL' or @routeId='')	SET @routeId=null;
	PRINT 'Hub ='+ isnull(@hubId,'NULL'); 
	if(@hubId is not null)
	begin
		PRINT 'In';
		select @regionId= Distributor_RegionId from tblcostcentre where id=@hubId;
		PRINT 'Region ='+ isnull(@regionId,'NULL'); 
	end;
	
	INSERT @tab1 
	select distinct convert(varchar(50), r.routeid) as RouteId , 
	                                          name as RouteName,
	isnull((select sum(item.weight)  from tblSourcingDocument doc 
	 inner join tblSourcingLineItem item on doc.id = item.DocumentId
	 where doc.DocumentTypeId=13
	   and doc.routeid=r.routeid
	   AND convert(nvarchar(26),doc.DocumentDate,23) between @startDate and @endDate
	   --and  ((@startDate is null or @endDate is null) or doc.DocumentDate between @startDate and  @endDate)
	   and  (@hubId is null  or doc.documentRecipientCostCentreId=@hubId or doc.documentIssuerCostCentreId=@hubId )
	   and  (@routeId is null  or doc.routeId=@routeId)
	 ),0)  as CentreWeight ,
	 
  isnull((select sum(item.weight)  from tblSourcingDocument doc 
	 inner join tblSourcingLineItem item on doc.id = item.DocumentId
	 where doc.DocumentTypeId=16 and doc.routeid=r.routeid
	 AND convert(nvarchar(26),doc.DocumentDate,23) between @startDate and @endDate
	 --and  ((@startDate is null or @endDate is null) or doc.DocumentDate between @startDate and  @endDate)
	 and  (@hubId is null  or doc.documentRecipientCostCentreId=@hubId or doc.documentIssuerCostCentreId=@hubId )
	  and  (@routeId is null  or doc.routeId=@routeId)
	 ),0)  as DeliveredWeight ,
	 
  isnull((select sum(item.weight)  from tblSourcingDocument doc 
	 inner join tblSourcingLineItem item on doc.id = item.DocumentId
	 where doc.DocumentTypeId=17 and doc.routeid=r.routeid
	 AND convert(nvarchar(26),doc.DocumentDate,23) between @startDate and @endDate
	 --and  ((@startDate is null or @endDate is null) or doc.DocumentDate between @startDate and  @endDate)
	 and  (@hubId is null  or doc.documentRecipientCostCentreId=@hubId or doc.documentIssuerCostCentreId=@hubId )
	  and  (@routeId is null  or doc.routeId=@routeId)
	 ),0)  as ReceivedWeight
  
 from   tblroutes r
 inner join tblSourcingDocument doc on doc.routeid=r.routeid
 AND convert(nvarchar(26),doc.DocumentDate,23) between @startDate and @endDate
--and  ((@startDate is null or @endDate is null) or doc.DocumentDate between @startDate and  @endDate)
and (@regionId  is null  or r.regionid=@regionId)
where  convert(nvarchar(26),doc.DocumentDate,23) between @startDate and @endDate
--((@startDate is null or @endDate is null) or doc.DocumentDate between @startDate and  @endDate) 
	 and  (@hubId is null  or doc.documentRecipientCostCentreId=@hubId or doc.documentIssuerCostCentreId=@hubId )
	  and  (@routeId is null  or doc.routeId=@routeId);
select RouteId,Name,CentreWeight,DeliveredWeight,ReceivedWeight
,(DeliveredWeight-CentreWeight) as ValianceBA 
,(ReceivedWeight-CentreWeight) as ValianceCA 
,((ReceivedWeight-CentreWeight)/NULLIF(CentreWeight,0)) as [CA/A]
 from 	@tab1	
 order by Name	
END;
--EXEC sp_A_ProductionByRoute @startDate = '10-Jan-2013', @endDate = '10-Dec-2013',@hubId='E6834108-4BA7-4A1A-98CA-9DD4DF8D68E2'

--EXEC sp_A_ProductionByRoute @hubId='ALL', @routeId='92D8E89E-5B83-4855-B3C1-47C322C201EA'

--EXEC sp_A_ProductionByRoute @startDate = '10-Jan-2013', @endDate = '10-Dec-2011'
--EXEC sp_A_ProductionByRoute
