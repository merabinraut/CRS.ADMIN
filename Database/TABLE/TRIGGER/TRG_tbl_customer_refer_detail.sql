use [CRS_V2]
go

ALTER trigger [dbo].[TRG_tbl_customer_refer_detail]
ON [dbo].[tbl_customer_refer_detail]
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
	SET NOCOUNT ON;

	IF EXISTS (SELECT 1 FROM INSERTED)
	BEGIN
		INSERT INTO dbo.tbl_customer_refer_detail_audit
		(
			Sno
		   ,ReferDetailId
		   ,ReferId
		   ,AffiliateId
		   ,CustomerId
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
		   ,ReferDetailId
		   ,ReferId
		   ,AffiliateId
		   ,CustomerId
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
	END;
	ELSE IF EXISTS (SELECT 1 FROM DELETED)
	BEGIN
		INSERT INTO dbo.tbl_customer_refer_detail_audit
		(
			Sno
		   ,ReferDetailId
		   ,ReferId
		   ,AffiliateId
		   ,CustomerId
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
		   ,ReferDetailId
		   ,ReferId
		   ,AffiliateId
		   ,CustomerId
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
END;
GO
