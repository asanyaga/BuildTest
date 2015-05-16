USE MASTER
GO
ALTER DATABASE hq SET ENABLE_BROKER WITH ROLLBACK IMMEDIATE
/*
--Make sure there are no uncommited transactions as they will be rolled back.
*/
GO
USE hq
GO
IF object_id('ScheduledJobs') IS NOT NULL
	DROP TABLE ScheduledJobs	
CREATE TABLE ScheduledJobs
(
	ID INT IDENTITY(1,1), 
	ScheduledSql nvarchar(max) NOT NULL, 
	FirstRunOn datetime NOT NULL, 
	LastRunOn datetime, 
	LastRunOK BIT NOT NULL DEFAULT (0), 
	IsRepeatable BIT NOT NULL DEFAULT (0), 
	IsEnabled BIT NOT NULL DEFAULT (0), 
	ConversationHandle uniqueidentifier NULL
)
GO

IF object_id('ScheduledJobsErrors') IS NOT NULL
	DROP TABLE ScheduledJobsErrors	
CREATE TABLE ScheduledJobsErrors
(
	Id BIGINT IDENTITY(1, 1) PRIMARY KEY,
	ErrorLine INT,
	ErrorNumber INT,
	ErrorMessage NVARCHAR(MAX),
	ErrorSeverity INT,
	ErrorState INT,
	ScheduledJobId INT,
	ErrorDate DATETIME NOT NULL DEFAULT GETUTCDATE()
)
 GO

IF object_id('tblInventoryDailySnapshot') IS NOT NULL
	DROP TABLE tblInventoryDailySnapshot	
CREATE TABLE tblInventoryDailySnapshot
(  
	id uniqueidentifier NOT NULL,  
	WareHouseId uniqueidentifier NOT NULL,  
	ProductId uniqueidentifier NOT NULL,  
	Balance decimal(20, 2) NULL,  
	Value money NULL,  
	UnavailableBalance decimal(18, 2) NOT NULL,
	DateOfEntry DATETIME NOT NULL
	CONSTRAINT PK_tempTblInventory PRIMARY KEY (id)
)
 GO
 
IF OBJECT_ID('usp_RemoveScheduledJob') IS NOT NULL
	DROP PROC usp_RemoveScheduledJob

GO
CREATE PROC usp_RemoveScheduledJob
	@ScheduledJobId INT
AS	
	BEGIN TRANSACTION
	BEGIN TRY
		DECLARE @ConversationHandle UNIQUEIDENTIFIER
		SELECT	@ConversationHandle = ConversationHandle
		FROM	ScheduledJobs 
		WHERE	Id = @ScheduledJobId 
		
		IF @@ROWCOUNT = 0
			RETURN;
		
		IF EXISTS (SELECT * FROM sys.conversation_endpoints WHERE conversation_handle = @ConversationHandle)
			END CONVERSATION @ConversationHandle
		
		DELETE ScheduledJobs WHERE Id = @ScheduledJobId
		COMMIT	
	END TRY
	BEGIN CATCH
		IF @@TRANCOUNT > 0
		BEGIN 
			ROLLBACK;
		END
		INSERT INTO ScheduledJobsErrors (
				ErrorLine, ErrorNumber, ErrorMessage, 
				ErrorSeverity, ErrorState, ScheduledJobId)
		SELECT	ERROR_LINE(), ERROR_NUMBER(), 'usp_RemoveScheduledJob: ' + ERROR_MESSAGE(), 
				ERROR_SEVERITY(), ERROR_STATE(), @ScheduledJobId
	END CATCH

GO

IF OBJECT_ID('usp_AddScheduledJob') IS NOT NULL
	DROP PROC usp_AddScheduledJob

GO
CREATE PROC usp_AddScheduledJob
(
	@ScheduledSql NVARCHAR(MAX), 
	@FirstRunOn DATETIME, 
	@IsRepeatable BIT	
)
AS
	DECLARE @ScheduledJobId INT, @TimeoutInSeconds INT, @ConversationHandle UNIQUEIDENTIFIER	
	BEGIN TRANSACTION
	BEGIN TRY
		INSERT INTO ScheduledJobs(ScheduledSql, FirstRunOn, IsRepeatable, ConversationHandle)
		VALUES (@ScheduledSql, @FirstRunOn, @IsRepeatable, NULL)
		SELECT @ScheduledJobId = SCOPE_IDENTITY()
		
		SELECT @TimeoutInSeconds = DATEDIFF(s, GETDATE(), @FirstRunOn);
		BEGIN DIALOG CONVERSATION @ConversationHandle
			FROM SERVICE   [//ScheduledJobService]
			TO SERVICE      '//ScheduledJobService', 
							'CURRENT DATABASE'
			ON CONTRACT     [//ScheduledJobContract]
			WITH ENCRYPTION = OFF;

		BEGIN CONVERSATION TIMER (@ConversationHandle)
		TIMEOUT = @TimeoutInSeconds;
		UPDATE	ScheduledJobs
		SET		ConversationHandle = @ConversationHandle, 
				IsEnabled = 1
		WHERE	ID = @ScheduledJobId 
		IF @@TRANCOUNT > 0
		BEGIN 
			COMMIT;
		END
	END TRY
	BEGIN CATCH
		IF @@TRANCOUNT > 0
		BEGIN 
			ROLLBACK;
		END
		INSERT INTO ScheduledJobsErrors (
				ErrorLine, ErrorNumber, ErrorMessage, 
				ErrorSeverity, ErrorState, ScheduledJobId)
		SELECT	ERROR_LINE(), ERROR_NUMBER(), 'usp_AddScheduledJob: ' + ERROR_MESSAGE(), 
				ERROR_SEVERITY(), ERROR_STATE(), @ScheduledJobId
	END CATCH

GO
IF OBJECT_ID('usp_RunScheduledJob') IS NOT NULL
	DROP PROC usp_RunScheduledJob

GO
CREATE PROC usp_RunScheduledJob
AS
	DECLARE @ConversationHandle UNIQUEIDENTIFIER, @ScheduledJobId INT, @LastRunOn DATETIME, @IsEnabled BIT, @LastRunOK BIT
	
	SELECT	@LastRunOn = GETDATE(), @IsEnabled = 0, @LastRunOK = 0
	BEGIN TRY
		DECLARE @message_type_name sysname;			
		RECEIVE TOP(1) 
			    @ConversationHandle = conversation_handle,
			    @message_type_name = message_type_name
		FROM ScheduledJobQueue
	
		IF @@ROWCOUNT = 0 OR ISNULL(@message_type_name, '') != 'http://schemas.microsoft.com/SQL/ServiceBroker/DialogTimer'
			RETURN;
		
		DECLARE @ScheduledSql NVARCHAR(MAX), @IsRepeatable BIT				
		SELECT	@ScheduledJobId = ID, @ScheduledSql = ScheduledSql, @IsRepeatable = IsRepeatable
		FROM	ScheduledJobs 
		WHERE	ConversationHandle = @ConversationHandle AND IsEnabled = 1
					
		IF @IsRepeatable = 0
		BEGIN			
			END CONVERSATION @ConversationHandle
			SELECT @IsEnabled = 0
		END
		ELSE
		BEGIN 
			BEGIN CONVERSATION TIMER (@ConversationHandle)
				TIMEOUT = 86400; -- 1 day
			SELECT @IsEnabled = 1
		END

		EXEC (@ScheduledSql)
		
		SELECT @LastRunOK = 1
	END TRY
	BEGIN CATCH		
		SELECT @IsEnabled = 0
		
		INSERT INTO ScheduledJobsErrors (
				ErrorLine, ErrorNumber, ErrorMessage, 
				ErrorSeverity, ErrorState, ScheduledJobId)
		SELECT	ERROR_LINE(), ERROR_NUMBER(), 'usp_RunScheduledJob: ' + ERROR_MESSAGE(), 
				ERROR_SEVERITY(), ERROR_STATE(), @ScheduledJobId
		
		IF @ConversationHandle != NULL		
		BEGIN
			IF EXISTS (SELECT * FROM sys.conversation_endpoints WHERE conversation_handle = @ConversationHandle)
				END CONVERSATION @ConversationHandle
		END
			
	END CATCH;
	UPDATE	ScheduledJobs
	SET		LastRunOn = @LastRunOn,
			IsEnabled = @IsEnabled,
			LastRunOK = @LastRunOK
	WHERE	ID = @ScheduledJobId
GO
IF OBJECT_ID('sp_tblInventoryDailySnapshot') IS NOT NULL
	DROP PROC sp_tblInventoryDailySnapshot
GO

CREATE PROCEDURE sp_tblInventoryDailySnapshot
AS
BEGIN
BEGIN TRANSACTION  
	BEGIN TRY
		IF NOT EXISTS(SELECT * FROM sys.objects WHERE Name LIKE N'%tblInventoryDailySnapshot%')  
		BEGIN  
			CREATE TABLE tblInventoryDailySnapshot(  
			id uniqueidentifier NOT NULL,  
			WareHouseId uniqueidentifier NOT NULL,  
			ProductId uniqueidentifier NOT NULL,  
			Balance decimal(20, 2) NULL,  
			Value money NULL,  
			UnavailableBalance decimal(18, 2) NOT NULL,
			DateOfEntry DATETIME NOT NULL
			CONSTRAINT PK_tempTblInventory PRIMARY KEY (id))   
		END 

		IF NOT EXISTS(SELECT DateOfEntry FROM tblInventoryDailySnapshot WHERE DATEDIFF(DD,GETDATE(),DateOfEntry) = 0)
		BEGIN
			DECLARE @ScheduledSql nvarchar(max), @RunOn datetime, @IsRepeatable BIT
			SELECT	@ScheduledSql = N'EXEC sp_tblInventoryDailySnapshot',
					@RunOn = DATEADD(hh,-3,DATEDIFF(d,-1,GETDATE())), 
					@IsRepeatable = 1

			EXEC usp_AddScheduledJob @ScheduledSql, @RunOn, @IsRepeatable

			INSERT INTO tblInventoryDailySnapshot  
			SELECT NEWID(), WareHouseId, ProductId, Balance, Value, UnavailableBalance, CONVERT(datetime, GETDATE(), 120)
			 FROM tblInventory 
		END
		ELSE
		BEGIN
			INSERT INTO tblInventoryDailySnapshot  
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

IF EXISTS(SELECT * FROM sys.services WHERE NAME = N'//ScheduledJobService')
	DROP SERVICE [//ScheduledJobService]

IF EXISTS(SELECT * FROM sys.service_queues WHERE NAME = N'ScheduledJobQueue')
	DROP QUEUE ScheduledJobQueue

IF EXISTS(SELECT * FROM sys.service_contracts  WHERE NAME = N'//ScheduledJobContract')
	DROP CONTRACT [//ScheduledJobContract]

GO
CREATE CONTRACT [//ScheduledJobContract]
	([http://schemas.microsoft.com/SQL/ServiceBroker/DialogTimer] SENT BY INITIATOR)

CREATE QUEUE ScheduledJobQueue 
	WITH STATUS = ON, 
	ACTIVATION (	
		PROCEDURE_NAME = usp_RunScheduledJob,
		MAX_QUEUE_READERS = 20, -- we expect max 20 jobs to start simultaneously
		EXECUTE AS SELF );

CREATE SERVICE [//ScheduledJobService]
	ON QUEUE ScheduledJobQueue ([//ScheduledJobContract])

EXEC sp_tblInventoryDailySnapshot
/*
------------------------------------------------------------
/*
Check if the agent job is running
The Column for LastRunOn should changes after every 2 minutes. and IsEnabled and IsRepeatable should always be set to 1
*/
------------------------------------------------------------

SELECT b.ID,DATEADD(hh, 3, a.dialog_timer) AS NextRun, b.FirstRunOn, b.LastRunOn, b.ScheduledSql, b.IsEnabled, b.IsRepeatable, c.ErrorMessage
 FROM sys.conversation_endpoints a 
 JOIN ScheduledJobs b ON a.conversation_handle = b.ConversationHandle
 LEFT JOIN ScheduledJobsErrors c ON b.ID = c.ScheduledJobId

SELECT * FROM tblInventoryDailySnapshot ORDER BY DateOfEntry Asc

------------------------------------------------------------
--To remove Job from queue
------------------------------------------------------------

usp_RemoveScheduledJob 2 -- 1 is the ID of the job in SELECT * FROM ScheduledJobs

Select DATEADD(hh,-3,DATEDIFF(d,-1,GETDATE())) --Change to whatever time the intetory take should run.

 */