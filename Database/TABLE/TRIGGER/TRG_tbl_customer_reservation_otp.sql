use [CRS_V2]
go

ALTER trigger [dbo].[TRG_tbl_customer_reservation_otp]
ON [dbo].[tbl_customer_reservation_otp]
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
	SET NOCOUNT ON;

	IF EXISTS (SELECT 1 FROM INSERTED)
	BEGIN
		INSERT INTO dbo.tbl_customer_reservation_otp_audit
		(
			Sno
		   ,ReservationId
		   ,SMSId
		   ,CustomerId
		   ,MobileNumber
		   ,FullName
		   ,OTPCode
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
		SELECT  Sno
			   ,ReservationId
			   ,SMSId
			   ,CustomerId
			   ,MobileNumber
			   ,FullName
			   ,OTPCode
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
		INSERT INTO dbo.tbl_customer_reservation_otp_audit
		(
			Sno
		   ,ReservationId
		   ,SMSId
		   ,CustomerId
		   ,MobileNumber
		   ,FullName
		   ,OTPCode
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
		SELECT  Sno
			   ,ReservationId
			   ,SMSId
			   ,CustomerId
			   ,MobileNumber
			   ,FullName
			   ,OTPCode
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
