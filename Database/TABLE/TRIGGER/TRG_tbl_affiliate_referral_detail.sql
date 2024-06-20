use [crs_v2]
go

ALTER TRIGGER TRG_tbl_affiliate_referral_detail
ON tbl_affiliate_referral_detail
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
	IF EXISTS (SELECT 1 FROM INSERTED)
	BEGIN
		INSERT INTO tbl_affiliate_referral_detail_audit
		(
			Sno
		   ,ReferralId
		   ,ReferId
		   ,ReferDetailId
		   ,AffiliateId
		   ,CustomerId
		   ,ReservationId
		   ,PaymentId
		   ,CommissionAmount
		   ,Status
		   ,Remarks
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
			  ,ReferralId
			  ,ReferId
			  ,ReferDetailId
			  ,AffiliateId
			  ,CustomerId
			  ,ReservationId
			  ,PaymentId
			  ,CommissionAmount
			  ,Status
			  ,Remarks
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
		INSERT INTO tbl_affiliate_referral_detail_audit
		(
			Sno
		   ,ReferralId
		   ,ReferId
		   ,ReferDetailId
		   ,AffiliateId
		   ,CustomerId
		   ,ReservationId
		   ,PaymentId
		   ,CommissionAmount
		   ,Status
		   ,Remarks
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
			  ,ReferralId
			  ,ReferId
			  ,ReferDetailId
			  ,AffiliateId
			  ,CustomerId
			  ,ReservationId
			  ,PaymentId
			  ,CommissionAmount
			  ,Status
			  ,Remarks
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