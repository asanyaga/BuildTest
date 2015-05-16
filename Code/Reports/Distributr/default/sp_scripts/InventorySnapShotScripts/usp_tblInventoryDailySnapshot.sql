IF OBJECT_ID('usp_tblInventoryDailySnapshot') IS NOT NULL
	DROP PROC usp_tblInventoryDailySnapshot
GO

CREATE PROCEDURE usp_tblInventoryDailySnapshot
AS
BEGIN
BEGIN TRANSACTION  
	BEGIN TRY
		IF NOT EXISTS(SELECT * FROM sys.objects WHERE Name LIKE N'%tblDailyInventorySnapShot%')  
		BEGIN  
			CREATE TABLE tblDailyInventorySnapShot(  
			id uniqueidentifier NOT NULL,  
			WareHouseId uniqueidentifier NOT NULL,  
			ProductId uniqueidentifier NOT NULL,  
			Balance decimal(20, 2) NULL,  
			Value money NULL,  
			UnavailableBalance decimal(18, 2) NOT NULL,
			DateOfEntry DATETIME NOT NULL
			CONSTRAINT PK_tempTblInventorySnapshot PRIMARY KEY (id))   
		END 

		IF NOT EXISTS(SELECT DateOfEntry FROM tblDailyInventorySnapShot WHERE DATEDIFF(DD,GETDATE(),DateOfEntry) = 0)
		BEGIN
			INSERT INTO tblDailyInventorySnapShot  
			SELECT NEWID(), WareHouseId, ProductId, Balance, Value, UnavailableBalance, CONVERT(datetime, GETDATE(), 120)
			 FROM tblInventory 
		END
		ELSE
		BEGIN
			INSERT INTO tblDailyInventorySnapShot  
			SELECT NEWID(), WareHouseId, ProductId, Balance, Value, UnavailableBalance, CONVERT(datetime, GETDATE(), 120)
			 FROM tblInventory 
		END
		COMMIT
	END TRY  
	BEGIN CATCH  
		IF @@TRANCOUNT > 0
		BEGIN
			ROLLBACK;
		END
	END CATCH
END
GO