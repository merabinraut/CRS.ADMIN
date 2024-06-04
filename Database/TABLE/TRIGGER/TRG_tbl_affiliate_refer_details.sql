use [CRS_V2]
go

ALTER TRIGGER TRG_tbl_affiliate_refer_details
ON tbl_affiliate_refer_details
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
	SET NOCOUNT ON;

	IF EXISTS (SELECT 1 FROM INSERTED)
	BEGIN
		INSERT INTO tbl_affiliate_refer_details_audit
		(
			Sno
		   ,ReferId
		   ,AffiliateId
		   ,SnsId
		   ,TotalClickCount
		   ,Status
		   ,CreatedBy
		   ,CreatedDate
		   ,CreatedIP
		   ,CreatedPlatform
		   ,UpdatedBy
		   ,UpdatedDate
		   ,UpdatedIP
		   ,UpdatedPlatform
		   ,TriggerLogUser
		   ,TriggerAction
		   ,TriggerActionLocalDate
		   ,TriggerActionUTCDate
		)
		SELECT Sno
		      ,ReferId
		      ,AffiliateId
		      ,SnsId
		      ,TotalClickCount
		      ,Status
		      ,CreatedBy
		      ,CreatedDate
		      ,CreatedIP
		      ,CreatedPlatform
		      ,UpdatedBy
		      ,UpdatedDate
		      ,UpdatedIP
		      ,UpdatedPlatform
			  ,SYSTEM_USER
			  ,CASE WHEN EXISTS (SELECT 1 FROM DELETED) THEN 'UPDATE' ELSE 'INSERT' END
			  ,GETDATE()
			  ,GETUTCDATE()
		FROM INSERTED;
		RETURN;
	END
	ELSE IF EXISTS (SELECT 1 FROM DELETED)
	BEGIN
			INSERT INTO tbl_affiliate_refer_details_audit
		(
			Sno
		   ,ReferId
		   ,AffiliateId
		   ,SnsId
		   ,TotalClickCount
		   ,Status
		   ,CreatedBy
		   ,CreatedDate
		   ,CreatedIP
		   ,CreatedPlatform
		   ,UpdatedBy
		   ,UpdatedDate
		   ,UpdatedIP
		   ,UpdatedPlatform
		   ,TriggerLogUser
		   ,TriggerAction
		   ,TriggerActionLocalDate
		   ,TriggerActionUTCDate
		)
		SELECT Sno
		      ,ReferId
		      ,AffiliateId
		      ,SnsId
		      ,TotalClickCount
		      ,Status
		      ,CreatedBy
		      ,CreatedDate
		      ,CreatedIP
		      ,CreatedPlatform
		      ,UpdatedBy
		      ,UpdatedDate
		      ,UpdatedIP
		      ,UpdatedPlatform
			  ,SYSTEM_USER
			  ,'DELETE'
			  ,GETDATE()
			  ,GETUTCDATE()
		FROM DELETED;
		RETURN;
	END
END
GO