IF NOT EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'sp_A_CommodityInventoryByStore')
   exec('CREATE PROCEDURE [sp_A_CommodityInventoryByStore] AS BEGIN SET NOCOUNT ON; END')
GO
Alter PROCEDURE sp_A_CommodityInventoryByStore
	@startDate varchar(50)=NULL,
    @endDate varchar(50)=NULL,
	@hubId varchar(50)=NULL ,   
    @storeId varchar(50)=NULL ,
    @commodityId varchar(50)=NULL,
    @gradeId varchar(50)=NULL
AS
BEGIN
    
    PRINT 'storeId ='+ isnull(@storeId,'NULL');
    IF(@hubId='ALL' or @hubId='')	SET @hubId=Null;
    IF(@storeId='ALL' or @storeId='')	SET @storeId=Null;
    IF(@commodityId='ALL' or @commodityId='')	SET @commodityId=Null;
    IF(@gradeId='ALL' or @gradeId='')	SET @gradeId=Null;
	select  cc.name as store ,
	como.name as commodity,grade.name as grade,sum(item.weight) as totalWeight
	from tblcostcentre cc
	inner join tblsourcingdocument doc on doc.documenttypeid=15 and doc.[DocumentRecipientCostCentreId]=cc.id
	inner join tblsourcinglineitem item on item.documentid=doc.id
	inner join tblcommodity como on como.id=item.commodityid
	inner join tblcommoditygrade grade on grade.id=item.gradeid
	where cc.costcentretype=11
	and  ((@startDate is null or @endDate is null)) or doc.DateIssued between @startDate and @endDate	
	and  (@hubId is null  or cc.parentCostCentreId=@hubId )
	and  (@storeId is null  or cc.id=@storeId )
	and  (@commodityId is null  or item.commodityid=@commodityId )
	and  (@gradeId is null  or item.gradeid=@gradeId )
	group by  como.id,cc.name,como.name,grade.name
		
	
	
	
END;
--EXEC sp_A_CommodityInventoryByStore @storeId='905EE8FD-59A3-46EE-B024-6A72303B42BC'
--EXEC sp_A_CommodityInventoryByStore 
