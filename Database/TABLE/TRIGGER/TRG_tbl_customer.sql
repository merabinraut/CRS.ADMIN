use [CRS_V2]
go

ALTER trigger [dbo].[TRG_tbl_customer]
ON [dbo].[tbl_customer]
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
	SET NOCOUNT ON;

	IF EXISTS (SELECT 1 FROM INSERTED)
	BEGIN
		INSERT INTO dbo.tbl_customer_audit
		(
			Sno
		   ,AgentId
		   ,NickName
		   ,FirstName
		   ,LastName
		   ,MobileNumber
		   ,DOB
		   ,EmailAddress
		   ,Gender
		   ,PreferredLocation
		   ,PostalCode
		   ,Prefecture
		   ,City
		   ,Street
		   ,ResidenceNumber
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
			   ,NickName
			   ,FirstName
			   ,LastName
			   ,MobileNumber
			   ,DOB
			   ,EmailAddress
			   ,Gender
			   ,PreferredLocation
			   ,PostalCode
			   ,Prefecture
			   ,City
			   ,Street
			   ,ResidenceNumber
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
	END;
	ELSE IF EXISTS (SELECT 1 FROM DELETED)
	BEGIN
		INSERT INTO dbo.tbl_customer_audit
		(
			Sno
		   ,AgentId
		   ,NickName
		   ,FirstName
		   ,LastName
		   ,MobileNumber
		   ,DOB
		   ,EmailAddress
		   ,Gender
		   ,PreferredLocation
		   ,PostalCode
		   ,Prefecture
		   ,City
		   ,Street
		   ,ResidenceNumber
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
			   ,NickName
			   ,FirstName
			   ,LastName
			   ,MobileNumber
			   ,DOB
			   ,EmailAddress
			   ,Gender
			   ,PreferredLocation
			   ,PostalCode
			   ,Prefecture
			   ,City
			   ,Street
			   ,ResidenceNumber
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
END;
GO
