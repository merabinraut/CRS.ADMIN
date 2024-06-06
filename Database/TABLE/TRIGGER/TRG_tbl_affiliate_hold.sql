use [CRS_V2]
go


ALTER trigger TRG_tbl_affiliate_hold
ON tbl_affiliate_hold
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
	SET NOCOUNT ON;

	IF EXISTS (SELECT 1 FROM INSERTED)
	BEGIN
		INSERT INTO tbl_affiliate_hold_audit
		(
			Sno
		   ,HoldAgentId
		   ,FirstName
		   ,LastName
		   ,MobileNumber
		   ,EmailAddress
		   ,DOB
		   ,Gender
		   ,SnsType
		   ,SnsURLLink
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
			  ,HoldAgentId
			  ,FirstName
			  ,LastName
			  ,MobileNumber
			  ,EmailAddress
			  ,DOB
			  ,Gender
			  ,SnsType
			  ,SnsURLLink
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
	END
	ELSE IF EXISTS (SELECT 1 FROM DELETED)
	BEGIN
		INSERT INTO tbl_affiliate_hold_audit
		(
			Sno
		   ,HoldAgentId
		   ,FirstName
		   ,LastName
		   ,MobileNumber
		   ,EmailAddress
		   ,DOB
		   ,Gender
		   ,SnsType
		   ,SnsURLLink
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
			  ,HoldAgentId
			  ,FirstName
			  ,LastName
			  ,MobileNumber
			  ,EmailAddress
			  ,DOB
			  ,Gender
			  ,SnsType
			  ,SnsURLLink
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
END
GO