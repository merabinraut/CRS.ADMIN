use [CRS_V2]
go

CREATE trigger [dbo].[TRG_tbl_Agent_balance]
ON [dbo].[tbl_Agent_balance]
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
	SET NOCOUNT ON;

	IF EXISTS (SELECT 1 FROM INSERTED)
	BEGIN
	INSERT INTO tbl_Agent_balance_audit
	(
		Id
	   ,AgentId
	   ,Amount
	   ,creditLimit
	   ,CreditPointUse
	   ,Status
	   ,RoleType
	   ,ActionUser
	   ,ActionDate
	   ,TriggerLogUser
	   ,TriggerAction
	   ,TriggerActionLocalDate
	   ,TriggerActionUTCDate
	)
	SELECT
	 	       Id
	          ,AgentId
	          ,Amount
	          ,creditLimit
	          ,CreditPointUse
	          ,Status
	          ,RoleType
	          ,ActionUser
	          ,ActionDate
			  ,SYSTEM_USER
			  ,CASE WHEN EXISTS (SELECT 1 FROM DELETED) THEN 'UPDATE' ELSE 'INSERT' END
			  ,GETDATE()
			  ,GETUTCDATE()
		FROM INSERTED;
		RETURN;
	END;
	ELSE IF EXISTS (SELECT 1 FROM DELETED)
	BEGIN
		INSERT INTO tbl_Agent_balance_audit
		(
			Id
		   ,AgentId
		   ,Amount
		   ,creditLimit
		   ,CreditPointUse
		   ,Status
		   ,RoleType
		   ,ActionUser
		   ,ActionDate
		   ,TriggerLogUser
		   ,TriggerAction
		   ,TriggerActionLocalDate
		   ,TriggerActionUTCDate
		)
		SELECT
	 	       Id
	          ,AgentId
	          ,Amount
	          ,creditLimit
	          ,CreditPointUse
	          ,Status
	          ,RoleType
	          ,ActionUser
	          ,ActionDate
			  ,SYSTEM_USER
			  ,'DELETE'
			  ,GETDATE()
			  ,GETUTCDATE()
		FROM DELETED;
		RETURN;
	END
END;
GO