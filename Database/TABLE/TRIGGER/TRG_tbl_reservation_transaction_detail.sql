USE [CRS_V2]
GO
/****** Object:  Trigger [dbo].[TRG_tbl_reservation_transaction_detail]    Script Date: 6/4/2024 11:20:38 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

create trigger [dbo].[TRG_tbl_reservation_transaction_detail]
ON [dbo].[tbl_reservation_transaction_detail]
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
	SET NOCOUNT ON;

	IF EXISTS (SELECT 1 FROM INSERTED)
	BEGIN
		INSERT INTO dbo.tbl_reservation_transaction_detail_audit
		(
			Sno
		   ,ReservationTxnId
		   ,ReservationId
		   ,ReservationType
		   ,PlanAmount
		   ,TotalPlanAmount
		   ,TotalClubPlanAmount
		   ,CommissionId
		   ,AdminPlanCommissionAmount
		   ,TotalAdminPlanCommissionAmount
		   ,AdminCommissionAmount
		   ,TotalAdminCommissionAmount
		   ,TotalAdminPayableAmount
		   ,Remarks
		   ,AdminPaymentStatus
		   ,ActionUser
		   ,ActionDate
		   ,ActionIP
		   ,ActionPlatform
		   ,TriggerLogUser
		   ,TriggerAction
		   ,TriggerActionLocalDate
		   ,TriggerActionUTCDate
		)
		SELECT Sno
			   ,ReservationTxnId
			   ,ReservationId
			   ,ReservationType
			   ,PlanAmount
			   ,TotalPlanAmount
			   ,TotalClubPlanAmount
			   ,CommissionId
			   ,AdminPlanCommissionAmount
			   ,TotalAdminPlanCommissionAmount
			   ,AdminCommissionAmount
			   ,TotalAdminCommissionAmount
			   ,TotalAdminPayableAmount
			   ,Remarks
			   ,AdminPaymentStatus
			   ,ActionUser
			   ,ActionDate
			   ,ActionIP
			   ,ActionPlatform
			  ,SYSTEM_USER
			  ,CASE WHEN EXISTS (SELECT 1 FROM DELETED) THEN 'UPDATE' ELSE 'INSERT' END
			  ,GETDATE()
			  ,GETUTCDATE()
		FROM INSERTED;
		RETURN;
	END;
	ELSE IF EXISTS (SELECT 1 FROM DELETED)
	BEGIN
		INSERT INTO dbo.tbl_reservation_transaction_detail_audit
		(
			Sno
		   ,ReservationTxnId
		   ,ReservationId
		   ,ReservationType
		   ,PlanAmount
		   ,TotalPlanAmount
		   ,TotalClubPlanAmount
		   ,CommissionId
		   ,AdminPlanCommissionAmount
		   ,TotalAdminPlanCommissionAmount
		   ,AdminCommissionAmount
		   ,TotalAdminCommissionAmount
		   ,TotalAdminPayableAmount
		   ,Remarks
		   ,AdminPaymentStatus
		   ,ActionUser
		   ,ActionDate
		   ,ActionIP
		   ,ActionPlatform
		   ,TriggerLogUser
		   ,TriggerAction
		   ,TriggerActionLocalDate
		   ,TriggerActionUTCDate
		)
		SELECT Sno
			   ,ReservationTxnId
			   ,ReservationId
			   ,ReservationType
			   ,PlanAmount
			   ,TotalPlanAmount
			   ,TotalClubPlanAmount
			   ,CommissionId
			   ,AdminPlanCommissionAmount
			   ,TotalAdminPlanCommissionAmount
			   ,AdminCommissionAmount
			   ,TotalAdminCommissionAmount
			   ,TotalAdminPayableAmount
			   ,Remarks
			   ,AdminPaymentStatus
			   ,ActionUser
			   ,ActionDate
			   ,ActionIP
			   ,ActionPlatform
			  ,SYSTEM_USER
			  ,'DELETE'
			  ,GETDATE()
			  ,GETUTCDATE()
		FROM DELETED;
		RETURN;
	END
END;
