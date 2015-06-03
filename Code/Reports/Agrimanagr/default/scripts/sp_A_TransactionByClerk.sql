IF NOT EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'sp_A_TransactionByClerk')
   exec('CREATE PROCEDURE [sp_A_TransactionByClerk] AS BEGIN SET NOCOUNT ON; END')
GO
Alter PROCEDURE sp_A_TransactionByClerk
    @startDate datetime,
    @endDate datetime,
    @hubId varchar(50)=NULL ,
    @clerkId varchar(50)=NULL 
      
AS
BEGIN
    
    IF(@hubId='ALL' or @hubId='')	SET @hubId=Null;
    IF(@clerkId='ALL' or @clerkId='')	SET @clerkId=Null;    
	IF(isdate(@startDate)=0) SET @startDate=null;
	IF(isdate(@endDate)=0)	SET @endDate=null;
			
	select  doc.documentReference as TransactionRef,doc.DocumentDate as DocumentDate, 
	cc.name as ClerkName,
	(select  Code from tblCommodityOwner where Id = doc.CommodityOwnerId) as FarmerCode,
	(select Name from tblcostcentre where id=cc.parentcostcentreId)
	 as Factory,	
	sum(item.weight) as TotalWeight
	from tblcostcentre cc
	left JOIN tblSourcingDocument  doc ON (cc.costcentreType=8 
	and (cc.Id = doc.DocumentIssuerCostCentreId or cc.Id = doc.DocumentRecipientCostCentreId))
	left JOIN tblSourcingLineItem  item ON doc.Id = item.DocumentId 
	left JOIN tblCommodityOwner  farmer ON farmer.Id=doc.CommodityOwnerId 
	
	where doc.DocumentTypeId = 13 
	AND convert(nvarchar(26),doc.DocumentDate,23) between @startDate and @endDate
	--and  ((@startDate is null or @endDate is null) or doc.DocumentDate between @startDate and  @endDate)
	and  (@hubId is null  or cc.Id=@hubId)
	--and  (@clerkId is null  or (doc.DocumentIssuerCostCentreId=@clerkId or doc.DocumentRecipientCostCentreId=@clerkId))
	and  (@clerkId is null  or (doc.DocumentIssuerUserId=@clerkId or doc.DocumentRecipientCostCentreId=@clerkId))
	Group by doc.DocumentDate, doc.DocumentReference,doc.CommodityOwnerId,
	cc.parentcostcentreId,cc.name
	
END;


-- EXEC sp_A_TransactionByClerk @startDate = '10-Jan-2015', @endDate = '10-Dec-2013' ,@routeid='9E67C9B7-0660-406D-B17D-9E227C58F424'

-- EXEC sp_A_TransactionByClerk @startDate = '2015-03-01', @endDate = '2015-03-26' ,@hubId='ALL',@clerkId='ALL'
