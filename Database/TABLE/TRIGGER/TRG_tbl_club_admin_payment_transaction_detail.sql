use [CRS_V2]
go


CREATE trigger [dbo].[TRG_tbl_club_admin_payment_transaction_detail]
ON [dbo].[tbl_club_admin_payment_transaction_detail]
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
	SET NOCOUNT ON;

	IF EXISTS (SELECT 1 FROM INSERTED)
	BEGIN
		INSERT INTO dbo.tbl_club_admin_payment_transaction_detail_audit
		(
			Sno
		   ,AgentId
		   ,UserId
		   ,TxnId
		   ,PaymentTxnId
		   ,ProcessId
		   ,PaymentMethod
		   ,Amount
		   ,Status
		   ,ImageURL
		   ,Remarks
		   ,CreatedBy
		   ,CreatedDate
		   ,CreatedUTCDate
		   ,CreatedIP
		   ,CreatedPlatform
		   ,UpdatedBy
		   ,UpdatedDate
		   ,UpdatedUTCDate
		   ,UpdatedIP
		   ,UpdatedPlatform
		   ,TriggerLogUser
		   ,TriggerAction
		   ,TriggerActionLocalDate
		   ,TriggerActionUTCDate
		)
		SELECT Sno
			   ,AgentId
			   ,UserId
			   ,TxnId
			   ,PaymentTxnId
			   ,ProcessId
			   ,PaymentMethod
			   ,Amount
			   ,Status
			   ,ImageURL
			   ,Remarks
			   ,CreatedBy
			   ,CreatedDate
			   ,CreatedUTCDate
			   ,CreatedIP
			   ,CreatedPlatform
			   ,UpdatedBy
			   ,UpdatedDate
			   ,UpdatedUTCDate
			   ,UpdatedIP
			   ,UpdatedPlatform
			  ,SYSTEM_USER
			  ,CASE WHEN EXISTS (SELECT 1 FROM DELETED) THEN 'UPDATE' ELSE 'INSERT' END
			  ,GETDATE()
			  ,GETUTCDATE()
		FROM INSERTED;
		RETURN;
	END;
	ELSE IF EXISTS (SELECT 1 FROM DELETED)
	BEGIN
		INSERT INTO dbo.tbl_club_admin_payment_transaction_detail_audit
		(
			Sno
		   ,AgentId
		   ,UserId
		   ,TxnId
		   ,PaymentTxnId
		   ,ProcessId
		   ,PaymentMethod
		   ,Amount
		   ,Status
		   ,ImageURL
		   ,Remarks
		   ,CreatedBy
		   ,CreatedDate
		   ,CreatedUTCDate
		   ,CreatedIP
		   ,CreatedPlatform
		   ,UpdatedBy
		   ,UpdatedDate
		   ,UpdatedUTCDate
		   ,UpdatedIP
		   ,UpdatedPlatform
		   ,TriggerLogUser
		   ,TriggerAction
		   ,TriggerActionLocalDate
		   ,TriggerActionUTCDate
		)
		SELECT Sno
			   ,AgentId
			   ,UserId
			   ,TxnId
			   ,PaymentTxnId
			   ,ProcessId
			   ,PaymentMethod
			   ,Amount
			   ,Status
			   ,ImageURL
			   ,Remarks
			   ,CreatedBy
			   ,CreatedDate
			   ,CreatedUTCDate
			   ,CreatedIP
			   ,CreatedPlatform
			   ,UpdatedBy
			   ,UpdatedDate
			   ,UpdatedUTCDate
			   ,UpdatedIP
			   ,UpdatedPlatform
			  ,SYSTEM_USER
			  ,'DELETE'
			  ,GETDATE()
			  ,GETUTCDATE()
		FROM DELETED;
		RETURN;
	END
END;
GO
