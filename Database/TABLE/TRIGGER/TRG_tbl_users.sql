USE [CRS_V2]
GO
/****** Object:  Trigger [dbo].[TRG_tbl_users]    Script Date: 6/4/2024 11:20:38 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

alter trigger [dbo].[TRG_tbl_users]
ON [dbo].[tbl_users]
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
	SET NOCOUNT ON;

	IF EXISTS (SELECT 1 FROM INSERTED)
	BEGIN
		INSERT INTO dbo.tbl_users_audit
		(
			Sno
		   ,UserId
		   ,RoleId
		   ,AgentId
		   ,LoginId
		   ,Password
		   ,Status
		   ,IsPrimary
		   ,FailedLoginAttempt
		   ,IsPasswordForceful
		   ,LastPasswordChangedDate
		   ,LastLoginDate
		   ,Session
		   ,RoleType
		   ,IsForcefulLogout
		   ,ActionUser
		   ,ActionIP
		   ,ActionPlatform
		   ,ActionDate
		   ,TriggerLogUser
		   ,TriggerAction
		   ,TriggerActionLocalDate
		   ,TriggerActionUTCDate
		)
		SELECT Sno
		   ,UserId
		   ,RoleId
		   ,AgentId
		   ,LoginId
		   ,Password
		   ,Status
		   ,IsPrimary
		   ,FailedLoginAttempt
		   ,IsPasswordForceful
		   ,LastPasswordChangedDate
		   ,LastLoginDate
		   ,Session
		   ,RoleType
		   ,IsForcefulLogout
		   ,ActionUser
		   ,ActionIP
		   ,ActionPlatform
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
		INSERT INTO dbo.tbl_users_audit
		(
			Sno
		   ,UserId
		   ,RoleId
		   ,AgentId
		   ,LoginId
		   ,Password
		   ,Status
		   ,IsPrimary
		   ,FailedLoginAttempt
		   ,IsPasswordForceful
		   ,LastPasswordChangedDate
		   ,LastLoginDate
		   ,Session
		   ,RoleType
		   ,IsForcefulLogout
		   ,ActionUser
		   ,ActionIP
		   ,ActionPlatform
		   ,ActionDate
		   ,TriggerLogUser
		   ,TriggerAction
		   ,TriggerActionLocalDate
		   ,TriggerActionUTCDate
		)
		SELECT Sno
		   ,UserId
		   ,RoleId
		   ,AgentId
		   ,LoginId
		   ,Password
		   ,Status
		   ,IsPrimary
		   ,FailedLoginAttempt
		   ,IsPasswordForceful
		   ,LastPasswordChangedDate
		   ,LastLoginDate
		   ,Session
		   ,RoleType
		   ,IsForcefulLogout
		   ,ActionUser
		   ,ActionIP
		   ,ActionPlatform
		   ,ActionDate
			  ,SYSTEM_USER
			  ,'DELETE'
			  ,GETDATE()
			  ,GETUTCDATE()
		FROM DELETED;
		RETURN;
	END
END;
