use [CRS_V2]
go

alter trigger [dbo].[TRG_tbl_customer_hold]
ON [dbo].[tbl_customer_hold]
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
	SET NOCOUNT ON;

	IF EXISTS (SELECT 1 FROM INSERTED)
	BEGIN
		INSERT INTO dbo.tbl_customer_hold_audit
		(
			Sno
		   ,NickName
		   ,MobileNumber
		   ,Status
		   ,CreatedBy
		   ,CreatedIP
		   ,CreatedPlatform
		   ,CreatedDate
		   ,UpdatedBy
		   ,UpdatedIP
		   ,UpdatedPlatform
		   ,UpdatedDate
		   ,TriggerLogUser
		   ,TriggerAction
		   ,TriggerActionLocalDate
		   ,TriggerActionUTCDate
		)
		SELECT Sno
			   ,NickName
			   ,MobileNumber
			   ,Status
			   ,CreatedBy
			   ,CreatedIP
			   ,CreatedPlatform
			   ,CreatedDate
			   ,UpdatedBy
			   ,UpdatedIP
			   ,UpdatedPlatform
			   ,UpdatedDate
			  ,SYSTEM_USER
			  ,CASE WHEN EXISTS (SELECT 1 FROM DELETED) THEN 'UPDATE' ELSE 'INSERT' END
			  ,GETDATE()
			  ,GETUTCDATE()
		FROM INSERTED;
		RETURN;
	END;
	ELSE IF EXISTS (SELECT 1 FROM DELETED)
	BEGIN
		INSERT INTO dbo.tbl_customer_hold_audit
		(
			Sno
		   ,NickName
		   ,MobileNumber
		   ,Status
		   ,CreatedBy
		   ,CreatedIP
		   ,CreatedPlatform
		   ,CreatedDate
		   ,UpdatedBy
		   ,UpdatedIP
		   ,UpdatedPlatform
		   ,UpdatedDate
		   ,TriggerLogUser
		   ,TriggerAction
		   ,TriggerActionLocalDate
		   ,TriggerActionUTCDate
		)
		SELECT Sno
			   ,NickName
			   ,MobileNumber
			   ,Status
			   ,CreatedBy
			   ,CreatedIP
			   ,CreatedPlatform
			   ,CreatedDate
			   ,UpdatedBy
			   ,UpdatedIP
			   ,UpdatedPlatform
			   ,UpdatedDate
			  ,SYSTEM_USER
			  ,'DELETE'
			  ,GETDATE()
			  ,GETUTCDATE()
		FROM DELETED;
		RETURN;
	END
END;
GO
