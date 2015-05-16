IF NOT EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'sp_A_CommodityCollectionByVehicle')
   exec('CREATE PROCEDURE [sp_A_CommodityCollectionByVehicle] AS BEGIN SET NOCOUNT ON; END')
GO
Alter PROCEDURE sp_A_CommodityCollectionByVehicle
    @startDate varchar(50)=NULL,
    @endDate varchar(50)=NULL,
    @hubId varchar(50)=NULL
    
AS
BEGIN
    
    IF(@hubId='ALL' or @hubId='')	SET @hubId=Null;
    IF(isdate(@startDate)=0) SET @startDate=null;
	IF(isdate(@endDate)=0)	SET @endDate=null;
	select  convert(varchar(50),doc.VehicleRegNo) as VehicleRegNo, 
	(select convert(varchar(50), Id) from tblEquipment where EquipmentNumber = doc.VehicleRegNo) as VehicleId,
	sum(item.weight) as TotalWeight,
	SUM(convert(int,item.NoOfContainer)) as TotalContainers
	from tblcostcentre cc 
	left JOIN tblSourcingDocument  doc ON cc.Id = doc.DocumentIssuerCostCentreId or  cc.Id =DocumentRecipientCostCentreId
	left JOIN tblSourcingLineItem  item ON doc.Id = item.DocumentId 
	left join tblEquipment vehicle on vehicle.EquipmentNumber = doc.VehicleRegNo
	where cc.costcentretype= 8 and doc.DocumentTypeId = 16
	and  ((@startDate is null or @endDate is null) or doc.DocumentDate between @startDate and  @endDate)
		and  (@hubId is null  or cc.id=@hubId)	
	Group by  doc.VehicleRegNo
END;

--EXEC sp_A_CommodityCollectionByVehicle @startDate = '10-Jan-2013', @endDate = '10-Dec-2013',@routeid='    '
--EXEC sp_A_CommodityCollectionByVehicle @startDate = '     ', @endDate = '',@centreId='    '
--EXEC sp_A_CommodityCollectionByVehicle 
--EXEC sp_A_CommodityCollectionByVehicle @startDate = '10-Jan-2013', @endDate = '10-Dec-2013' ,@centreId='B4E16809-E97F-4E77-BC0F-E77A0946B941'