drop procedure sp_D_DB_TargetVsSales_PerRoute
go 
create procedure sp_D_DB_TargetVsSales_PerRoute
--@ccTypeId int = 5,
@distributorId nvarchar(50) = null,
@outletId nvarchar(50)=null,
@routesId nvarchar(50)=null
as
begin
if  @distributorId='ALL'  begin set @distributorId='%' end
if  @outletId='ALL'  begin set @outletId='%' end
if  @routesId='ALL'  begin set @routesId='%' end

	declare @ccId as nvarchar(50),
	        @TargetValue as decimal(18,2),
	        @Sales as decimal(18,2),
	        @ccName as nvarchar(50),
	        @TargetName as nvarchar(50),
	        @RouteName as nvarchar(50),
	        @RtId as nvarchar(50);
	        
    declare @TstartDate as datetime,
            @TendDate as datetime;
	
    declare @data table(ccName nvarchar(50),
                        TargetName nvarchar(50),
                        TargetValue  decimal(18,2),
                        SalesValue decimal(18,2),
                        StartDate date,
                        EndDate date,
                        RouteName nvarchar(50));
	
	declare target_cursor cursor for select convert(nvarchar(50),cc.Id) ,
	                                                             cc.Name,
	                                                             tp.Name,
	                                                             t.TargetValue,
	                                                             tp.StartDate,
	                                                             tp.EndDate,
	                                                             rt.Name 
	                                 from tblTarget t inner join tblTargetPeriod tp on t.PeriodId = tp.Id 
	                                                  inner join tblCostCentre cc on t.CostCentreId = cc.Id
	                                                  inner join tblRoutes rt on cc.RouteId = rt.RouteID
	                                 where cc.CostCentreType = 5 --@ccTypeId
	                                   and convert(nvarchar(50),rt.RouteID) = @routesId
	                                   and convert(nvarchar(50),cc.ParentCostCentreId) = @distributorId  
	                                   and convert(nvarchar(50),cc.Id) = @outletId               
	   
	open target_cursor
	fetch next from target_cursor
	into @ccId,
	     @ccName,
	     @TargetName,
	     @TargetValue,
	     @TstartDate,
	     @TendDate,
	     @RouteName
	while @@FETCH_STATUS = 0
	begin
	  select @Sales = SUM(li.Quantity * li.Value) 
      from tblDocument docs inner join tblLineItems li on docs.Id = li.DocumentID
      where    docs.DocumentTypeId = 1 
          and (docs.OrderOrderTypeId = 1 
           or (docs.OrderOrderTypeId = 3 and docs.DocumentStatusId = 99) )
          and ( convert(nvarchar(50),docs.OrderIssuedOnBehalfOfCC) = @ccId )
          and (docs.DocumentDateIssued between @TstartDate and @TendDate)
          
	
	insert into @data values (@ccName,
	                          @TargetName,
	                          @TargetValue,
	                          isnull(@Sales,0),
	                          @TstartDate,
	                          @TendDate,
	                          @RouteName);
		
	fetch next from target_cursor
	into @ccId,
	     @ccName,
	     @TargetName,
	     @TargetValue,
	     @TstartDate,
	     @TendDate,
	     @RouteName
		
		
	end;
	 close target_cursor;
		deallocate target_cursor;
		select * from @data
	
end;

