IF NOT EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'sp_A_TransactionCummulative')
   exec('CREATE PROCEDURE [sp_A_TransactionCummulative] AS BEGIN SET NOCOUNT ON; END')
GO
Alter PROCEDURE sp_A_TransactionCummulative
    @startDate varchar(50)=NULL,
    @endDate varchar(50)=NULL ,
    @hubId varchar(50)=NULL ,
    @routeId varchar(50)=NULL ,
    @centreId varchar(50)=NULL ,
    @ownerId varchar(50)=NULL
AS
BEGIN
    
    IF(@hubId='ALL' or @hubId='')	SET @hubId=Null;
    IF(@routeId='ALL' or @routeId='')	SET @routeId=Null;
    IF(@centreId='ALL' or @centreId='')	SET @centreId=Null;
    IF(@ownerId='ALL' or @ownerId='')	SET @ownerId=Null;
	IF(isdate(@startDate)=0) SET @startDate=null;
	IF(isdate(@endDate)=0)	SET @endDate=null;
			
	
	DECLARE @_ownerId uniqueidentifier,@_centreId uniqueidentifier,@_hubId uniqueidentifier;
	DECLARE @ownername varchar(100),
	@ownercode varchar(100),
	@docId varchar(100),
	@docref varchar(100),
	@hubname varchar(100),
	@centername varchar(100);
	declare @totalWeight decimal(18,2),@cummulativeWeight decimal(18,2) ;
	declare @purchasedate date;
	Declare @ownercount int=0;
	declare @data table(
	ownerId varchar(50),
	ownerName varchar(100),
	ownerCode varchar(100),
	hub varchar(100),
	centre varchar(100),
	docref varchar(100),
	totalWeight decimal(20,2),
	cummulativeWeight decimal(20,2),
	purchasedate date
	);
	select @cummulativeWeight=0;
	DECLARE owner_cursor CURSOR FOR 
	SELECT distinct convert(varchar(50), o.id) as id,(isnull(o.surname,'')+' '+isnull(o.firstname,'')+' '+isnull(o.lastname,'')) as name,o.code as hub from tblCommodityOwner o
	inner join tblSourcingDocument doc on doc.CommodityOwnerId=o.id
	inner join tblcostcentre cc on ((cc.id =doc.DocumentIssuerCostCentreId or  cc.id=doc.DocumentRecipientCostCentreId) 
	and cc.costcentretype=8)
	where 1=1
	and  ((@startDate is null or @endDate is null) or doc.DocumentDate between @startDate and  @endDate)
	and  (@routeId is null  or doc.RouteId=@routeId)
	and  (@hubId is null  or cc.id=@hubId)
	and  (@centreId is null  or doc.centreid=@centreId)
	and  (@ownerId is null  or doc.commodityOwnerId=@ownerId)
	ORDER BY name;

	OPEN owner_cursor

	FETCH NEXT FROM owner_cursor 
	INTO @_ownerId,@ownername,@ownercode
	select @cummulativeWeight=0;
	WHILE @@FETCH_STATUS = 0
	BEGIN
		set @ownercount=@ownercount+1;
		select @cummulativeWeight=0;
		PRINT convert(varchar(50),@_ownerId)+' -'+convert(varchar(50),@cummulativeWeight)+'- '+@ownername+' '+ convert(varchar(50),@ownercount);
		DECLARE doc_cursor CURSOR FOR 
		SELECT doc.id,doc.documentreference,doc.centreid,
		(select name from  tblcostcentre where CostCentreType=8 
		and(  id=doc.DocumentRecipientCostCentreId or id =doc.DocumentIssuerCostCentreId)
		)as Hub,convert(date,doc.documentdate)
		 from tblSourcingDocument doc
		 inner join tblcostcentre cc on ((cc.id =doc.DocumentIssuerCostCentreId or  cc.id=doc.DocumentRecipientCostCentreId) 
		and cc.costcentretype=8)
		where doc.CommodityOwnerId=@_ownerId
		and  ((@startDate is null or @endDate is null) or doc.DocumentDate between @startDate and  @endDate)
		and  (@routeId is null  or doc.RouteId=@routeId)
		and  (@hubId is null  or cc.id=@hubId)
		and  (@centreId is null  or doc.centreid=@centreId)
		
		order by doc.documentdate asc;
	   
		OPEN doc_cursor
		FETCH NEXT FROM doc_cursor 	INTO @docId,@docref,@centreId,@hubname,@purchasedate
		
		WHILE @@FETCH_STATUS = 0
		BEGIN
			select @centername=name from tblcentre where id=@centreid;	 
			select @totalWeight=sum(weight) from tblSourcingLineItem 		
			where documentid=@docId group by documentid;
			select @cummulativeWeight=@cummulativeWeight+@totalWeight;	
			insert into @data 
			values(@_ownerId,@ownername,@ownercode,@hubname,@centername,@docref,@totalWeight,@cummulativeWeight,@purchasedate);	
			FETCH NEXT FROM doc_cursor 	INTO @docId,@docref,@centreId,@hubname,@purchasedate
		END;
		CLOSE doc_cursor;
		DEALLOCATE doc_cursor;
		FETCH NEXT FROM owner_cursor INTO @_ownerId,@ownername,@ownercode
		
	END;
	CLOSE owner_cursor;
	DEALLOCATE owner_cursor;
	select * from @data;
	   
	END;
--EXEC sp_A_TransactionCummulative @startDate = '10-Jan-2013', @endDate = '10-Dec-2013',@routeId='    '
--EXEC sp_A_TransactionCummulative @startDate = '     ', @endDate = '',@routeId='    '
--EXEC sp_A_TransactionCummulative 
--EXEC sp_A_TransactionCummulative @startDate = '10-Jan-2013', @endDate = '10-Dec-2013' ,@_routeId='9E67C9B7-0660-406D-B17D-9E227C58F424'
--EXEC sp_A_TransactionCummulative @hubId='E6834108-4BA7-4A1A-98CA-9DD4DF8D68E3'