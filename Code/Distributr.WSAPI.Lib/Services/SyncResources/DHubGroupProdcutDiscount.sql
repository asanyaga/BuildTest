declare @costcentreid uniqueidentifier ;
set @costcentreid='{0}';
declare @from datetime ;
set @from='{1}';
select p.[Id] as MasterId
    ,p.[DiscountGroup] as DiscountGroupMasterId
    ,p.[IM_DateCreated] as DateCreated
    ,p.[IM_DateLastUpdated] as DateLastUpdated
    ,p.[IM_Status]  as StatusId 
    ,p.id as LineItemId
	,p.quantity as Quantity
    ,p.DiscountRate as DiscountRate
    ,p.EffectiveDate as EffectiveDate
    ,p.EndDate as EndDate
    ,p.ProductRef as ProductMasterId
 from  tblProductDiscountGroup p	
								
 where   exists (select distinct cc.Outlet_DiscountGroupId from tblCostCentre cc
						where cc.CostCentreType=5
						and (cc.id= @costcentreid or cc.ParentCostCentreId= @costcentreid) 
						and cc.Outlet_DiscountGroupId =p.DiscountGroup)
						and p.IM_DateLastUpdated>@from
						and p.ProductRef is not null
						and p.DiscountRate is not null
						
	order by p.IM_DateCreated





