
DELETE FROM cokeunittests.[dbo].[tblLineItems]; 
DELETE FROM cokeunittests.[dbo].[tblDocument];
DELETE FROM cokeunittests.[dbo].[tblInventoryTransaction];
DELETE FROM cokeunittests.[dbo].[tblInventory];

DELETE FROM [DistributrLocal].[dbo].[tblLineItems]; 
DELETE FROM [DistributrLocal].[dbo].[tblDocument];
DELETE FROM [DistributrLocal].[dbo].[tblInventoryTransaction];
DELETE FROM [DistributrLocal].[dbo].[tblInventory];

delete from [DistributrLocalSetup].[dbo].OutgoingCommandQueueItemLocals
delete from [DistributrLocalSetup].[dbo].IncomingCommandQueueItemLocals