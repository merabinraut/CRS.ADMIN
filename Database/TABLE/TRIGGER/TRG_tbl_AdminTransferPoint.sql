use [CRS_V2]
go

create trigger [dbo].[TRG_tbl_AdminTransferPoint]
ON [dbo].[tbl_AdminTransferPoint]
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
	SET NOCOUNT ON;

	IF EXISTS (SELECT 1 FROM INSERTED)
	BEGIN
		INSERT INTO dbo.tbl_AdminTransferPoint_audit
		(
			Id
		   ,TransactionId
		   ,AgentId
		   ,Amount
		   ,Remark
		   ,TrancationType
		   ,Image
		   ,Status
		   ,ActionDate
		   ,ActionUser
		   ,TriggerLogUser
		   ,TriggerAction
		   ,TriggerActionLocalDate
		   ,TriggerActionUTCDate
		)
		SELECT Id
			  ,TransactionId
			  ,AgentId
			  ,Amount
			  ,Remark
			  ,TrancationType
			  ,Image
			  ,Status
			  ,ActionDate
			  ,ActionUser
			  ,SYSTEM_USER
			  ,CASE WHEN EXISTS (SELECT 1 FROM DELETED) THEN 'UPDATE' ELSE 'INSERT' END
			  ,GETDATE()
			  ,GETUTCDATE()
		FROM INSERTED;
		RETURN;
	END;
	ELSE IF EXISTS (SELECT 1 FROM DELETED)
	BEGIN
		INSERT INTO dbo.tbl_AdminTransferPoint_audit
		(
			Id
		   ,TransactionId
		   ,AgentId
		   ,Amount
		   ,Remark
		   ,TrancationType
		   ,Image
		   ,Status
		   ,ActionDate
		   ,ActionUser
		   ,TriggerLogUser
		   ,TriggerAction
		   ,TriggerActionLocalDate
		   ,TriggerActionUTCDate
		)
		SELECT Id
			  ,TransactionId
			  ,AgentId
			  ,Amount
			  ,Remark
			  ,TrancationType
			  ,Image
			  ,Status
			  ,ActionDate
			  ,ActionUser
			  ,SYSTEM_USER
			  ,'DELETE'
			  ,GETDATE()
			  ,GETUTCDATE()
		FROM DELETED;
		RETURN;
	END
END;
GO
