CREATE TABLE tblDistributrCommand(
	Id bigint primary Key IDENTITY(1,1) NOT NULL,	
	CommandId uniqueidentifier not null,
	DocumentId uniqueidentifier not null,
	DateCommandInserted datetime not null,
	CommandGeneratedByCostCentreApplicationId int not null,
	CommandGeneratedByUserId int not null,
	CommandType nvarchar(50)  not null,
	JsonCommand nvarchar(2000)  not null   
	
 ); 
 CREATE TABLE tblRoutingCentre(
	Id bigint primary Key IDENTITY(1,1) NOT NULL,
	DistributorCommandId bigint not null,	
	RoutingCostCentreId int not null
 );
 Alter table tblRoutingCentre add foreign key (DistributorCommandId) References tblDistributrCommand(Id);
 CREATE TABLE tblRoutingStatus(
	Id bigint primary Key IDENTITY(1,1) NOT NULL,
	DistributorCommandId bigint not null,	
	DestinationCostCentreApplicationId int not null,
	Delivered bit not null,
	DateDelivered DateTime null,
	Executed bit not null,
	DateExecuted DateTime null
 ); 
 Alter table tblRoutingStatus add foreign key (DistributorCommandId) References tblDistributrCommand(Id);