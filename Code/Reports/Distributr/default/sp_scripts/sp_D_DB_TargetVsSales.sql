drop procedure sp_D_DB_TargetVsSales
go 
create procedure sp_D_DB_TargetVsSales
@startDate date,
@endDate date,
@ccTypeId int = null,
@distributorId nvarchar(50) = null,
@outletId nvarchar(50)=null
--@ccTypeId int = null,
--@distributorId nvarchar(50),
--@outletId nvarchar(50)
as
begin
if  @distributorId='ALL'  begin set @distributorId='%' end
if  @outletId='ALL'  begin set @outletId='%' end
--if  @ccTypeId='ALL'  begin set @ccTypeId='%' end

	declare @ccId as nvarchar(50),
	        @TargetValue as decimal(18,2),
	        @Sales as decimal(18,2),
	        @ccName as nvarchar(50),
	        @TargetName as nvarchar(50);
	        
    declare @TstartDate as datetime,
            @TendDate as datetime;
	
    declare @data table(ccName nvarchar(50),
                        TargetName nvarchar(50),
                        TargetValue  decimal(18,2),
                        SalesValue decimal(18,2),
                        StartDate date,
                        EndDate date);
	
	declare target_cursor cursor for select convert(nvarchar(50),cc.Id) ,
	                                                             cc.Name,
	                                                             tp.Name,
	                                                             t.TargetValue,
	                                                             tp.StartDate,
	                                                             tp.EndDate 
	                                 from tblTarget t inner join tblTargetPeriod tp on t.PeriodId = tp.Id 
	                                                   inner join tblCostCentre cc on t.CostCentreId = cc.Id
	                                 where cc.CostCentreType = @ccTypeId
	                                  and (convert(nvarchar(50),cc.Id) like isnull(@distributorId,'%') 
	                                    or convert(nvarchar(50),cc.Id) like isnull(@outletId,'%'))
	                               --  and((convert(nvarchar(26),tp.StartDate,23)) >= @startDate  and (convert(nvarchar(26),tp.EndDate,23)) <= @endDate)  
	                                                   
	   
	open target_cursor
	fetch next from target_cursor
	into @ccId,
	     @ccName,
	     @TargetName,
	     @TargetValue,
	     @TstartDate,
	     @TendDate
	while @@FETCH_STATUS = 0
	begin
	  select @Sales = SUM(li.Quantity * li.Value) 
      from tblDocument docs inner join tblLineItems li on docs.Id = li.DocumentID
      where docs.DocumentTypeId = 1 
          and (docs.OrderOrderTypeId = 1 or (docs.OrderOrderTypeId = 3 and docs.DocumentStatusId = 99) )
          and (docs.DocumentIssuerCostCentreId = @ccId 
            or docs.DocumentRecipientCostCentre = @ccId 
            or docs.OrderIssuedOnBehalfOfCC = @ccId )
          and (docs.DocumentDateIssued between @TstartDate and @TendDate)
	
	insert into @data values (@ccName,
	                          @TargetName,
	                          @TargetValue,
	                          isnull(@Sales,0),
	                          @TstartDate,
	                          @TendDate);
		
	fetch next from target_cursor
	into @ccId,
	     @ccName,
	     @TargetName,
	     @TargetValue,
	     @TstartDate,
	     @TendDate
		
		
	end;
	 close target_cursor;
		deallocate target_cursor;
		select * from @data
	
end;

-- exec sp_D_DB_TargetVsSales  @startDate ='2013-03-27',@endDate = '2013-05-27'