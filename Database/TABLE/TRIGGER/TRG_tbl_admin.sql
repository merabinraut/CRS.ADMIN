use [CRS_V2]
go


ALTER TRIGGER [dbo].[TRG_tbl_admin]
ON [dbo].[tbl_admin]
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
	SET NOCOUNT ON;

	IF EXISTS (SELECT 1 FROM INSERTED)
	BEGIN
		INSERT INTO dbo.tbl_admin_audit
		(
			Id
		   ,RoleId
		   ,Username
		   ,FullName
		   ,EmailAddress
		   ,MobileNumber
		   ,Password
		   ,Status
		   ,ProfileImage
		   ,Session
		   ,AllowMultipleLogin
		   ,FailedLoginAttempt
		   ,IsPasswordForceful
		   ,LastPasswordChangedDate
		   ,LastLoginDate
		   ,ActionUser
		   ,ActionDate
		   ,ActionIP
		   ,TriggerLogUser
		   ,TriggerAction
		   ,TriggerActionLocalDate
		   ,TriggerActionUTCDate
		)

		SELECT Id
			  ,RoleId
			  ,Username
			  ,FullName
			  ,EmailAddress
			  ,MobileNumber
			  ,Password
			  ,Status
			  ,ProfileImage
			  ,Session
			  ,AllowMultipleLogin
			  ,FailedLoginAttempt
			  ,IsPasswordForceful
			  ,LastPasswordChangedDate
			  ,LastLoginDate
			  ,ActionUser
			  ,ActionDate
			  ,ActionIP
			  ,SYSTEM_USER
			  ,CASE WHEN EXISTS (SELECT 1 FROM DELETED) THEN 'UPDATE' ELSE 'INSERT' END
			  ,GETDATE()
			  ,GETUTCDATE()
		FROM INSERTED;
		RETURN;
	END
	ELSE IF EXISTS (SELECT 1 FROM DELETED)
	BEGIN
		INSERT INTO dbo.tbl_admin_audit
		(
			Id
		   ,RoleId
		   ,Username
		   ,FullName
		   ,EmailAddress
		   ,MobileNumber
		   ,Password
		   ,Status
		   ,ProfileImage
		   ,Session
		   ,AllowMultipleLogin
		   ,FailedLoginAttempt
		   ,IsPasswordForceful
		   ,LastPasswordChangedDate
		   ,LastLoginDate
		   ,ActionUser
		   ,ActionDate
		   ,ActionIP
		   ,TriggerLogUser
		   ,TriggerAction
		   ,TriggerActionLocalDate
		   ,TriggerActionUTCDate
		)

		SELECT Id
			  ,RoleId
			  ,Username
			  ,FullName
			  ,EmailAddress
			  ,MobileNumber
			  ,Password
			  ,Status
			  ,ProfileImage
			  ,Session
			  ,AllowMultipleLogin
			  ,FailedLoginAttempt
			  ,IsPasswordForceful
			  ,LastPasswordChangedDate
			  ,LastLoginDate
			  ,ActionUser
			  ,ActionDate
			  ,ActionIP
			  ,SYSTEM_USER
			  ,'DELETE'
			  ,GETDATE()
			  ,GETUTCDATE()
		FROM DELETED;
		RETURN;
	END
END
GO

