use [CRS_V2]
go

ALTER TRIGGER [dbo].[TRG_tbl_affiliate]
ON dbo.tbl_affiliate
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
	SET NOCOUNT ON;

	IF EXISTS (SELECT 1 FROM INSERTED)
	BEGIN
		INSERT INTO dbo.tbl_affiliate_audit
		(
			Sno
		   ,AgentId
		   ,FirstName
		   ,LastName
		   ,MobileNumber
		   ,EmailAddress
		   ,DOB
		   ,Gender
		   ,ProfileImage
		   ,PointsCategoryId
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
			  ,AgentId
			  ,FirstName
			  ,LastName
			  ,MobileNumber
			  ,EmailAddress
			  ,DOB
			  ,Gender
			  ,ProfileImage
			  ,PointsCategoryId
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
	END
	ELSE IF EXISTS (SELECT 1 FROM DELETED)
	BEGIN
		INSERT INTO dbo.tbl_affiliate_audit
		(
			Sno
		   ,AgentId
		   ,FirstName
		   ,LastName
		   ,MobileNumber
		   ,EmailAddress
		   ,DOB
		   ,Gender
		   ,ProfileImage
		   ,PointsCategoryId
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
			  ,AgentId
			  ,FirstName
			  ,LastName
			  ,MobileNumber
			  ,EmailAddress
			  ,DOB
			  ,Gender
			  ,ProfileImage
			  ,PointsCategoryId
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
END
GO