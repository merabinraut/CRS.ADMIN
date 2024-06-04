USE [CRS_V2]
GO
/****** Object:  Trigger [dbo].[TRG_tbl_payment_gateway_transaction]    Script Date: 6/4/2024 11:20:38 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER trigger [dbo].[TRG_tbl_payment_gateway_transaction]
ON [dbo].[tbl_payment_gateway_transaction]
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
	SET NOCOUNT ON;

	IF EXISTS (SELECT 1 FROM INSERTED)
	BEGIN
		INSERT INTO dbo.tbl_payment_gateway_transaction_audit
		(
			Sno
		   ,PaymentTxnId
		   ,PartnerTxnId
		   ,InvoiceId
		   ,ProcessId
		   ,FromAgentId
		   ,FromUserId
		   ,FromRoleType
		   ,ToAgentId
		   ,ToUserId
		   ,ToRoleType
		   ,Amount
		   ,ServiceCharge
		   ,TransactionType
		   ,Status
		   ,GatewayType
		   ,GatewayStatus
		   ,GatewayPaymentStatus
		   ,GatewayTxnId
		   ,GatewayRequest
		   ,GatewayResponse
		   ,GatewayProcessId
		   ,GatewayAdditionalDetail1
		   ,GatewayAdditionalDetail2
		   ,GatewayAdditionalDetail3
		   ,GatewayAdditionalDetail4
		   ,GatewayAdditionalDetail5
		   ,CheckCounter
		   ,ActionDate
		   ,ActionUTCDate
		   ,ActionUser
		   ,ActionIP
		   ,ActionPlatform
		   ,TriggerLogUser
		   ,TriggerAction
		   ,TriggerActionLocalDate
		   ,TriggerActionUTCDate
		)
		SELECT Sno
			   ,PaymentTxnId
			   ,PartnerTxnId
			   ,InvoiceId
			   ,ProcessId
			   ,FromAgentId
			   ,FromUserId
			   ,FromRoleType
			   ,ToAgentId
			   ,ToUserId
			   ,ToRoleType
			   ,Amount
			   ,ServiceCharge
			   ,TransactionType
			   ,Status
			   ,GatewayType
			   ,GatewayStatus
			   ,GatewayPaymentStatus
			   ,GatewayTxnId
			   ,GatewayRequest
			   ,GatewayResponse
			   ,GatewayProcessId
			   ,GatewayAdditionalDetail1
			   ,GatewayAdditionalDetail2
			   ,GatewayAdditionalDetail3
			   ,GatewayAdditionalDetail4
			   ,GatewayAdditionalDetail5
			   ,CheckCounter
			   ,ActionDate
			   ,ActionUTCDate
			   ,ActionUser
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
		INSERT INTO dbo.tbl_payment_gateway_transaction_audit
		(
			Sno
		   ,PaymentTxnId
		   ,PartnerTxnId
		   ,InvoiceId
		   ,ProcessId
		   ,FromAgentId
		   ,FromUserId
		   ,FromRoleType
		   ,ToAgentId
		   ,ToUserId
		   ,ToRoleType
		   ,Amount
		   ,ServiceCharge
		   ,TransactionType
		   ,Status
		   ,GatewayType
		   ,GatewayStatus
		   ,GatewayPaymentStatus
		   ,GatewayTxnId
		   ,GatewayRequest
		   ,GatewayResponse
		   ,GatewayProcessId
		   ,GatewayAdditionalDetail1
		   ,GatewayAdditionalDetail2
		   ,GatewayAdditionalDetail3
		   ,GatewayAdditionalDetail4
		   ,GatewayAdditionalDetail5
		   ,CheckCounter
		   ,ActionDate
		   ,ActionUTCDate
		   ,ActionUser
		   ,ActionIP
		   ,ActionPlatform
		   ,TriggerLogUser
		   ,TriggerAction
		   ,TriggerActionLocalDate
		   ,TriggerActionUTCDate
		)
		SELECT Sno
			   ,PaymentTxnId
			   ,PartnerTxnId
			   ,InvoiceId
			   ,ProcessId
			   ,FromAgentId
			   ,FromUserId
			   ,FromRoleType
			   ,ToAgentId
			   ,ToUserId
			   ,ToRoleType
			   ,Amount
			   ,ServiceCharge
			   ,TransactionType
			   ,Status
			   ,GatewayType
			   ,GatewayStatus
			   ,GatewayPaymentStatus
			   ,GatewayTxnId
			   ,GatewayRequest
			   ,GatewayResponse
			   ,GatewayProcessId
			   ,GatewayAdditionalDetail1
			   ,GatewayAdditionalDetail2
			   ,GatewayAdditionalDetail3
			   ,GatewayAdditionalDetail4
			   ,GatewayAdditionalDetail5
			   ,CheckCounter
			   ,ActionDate
			   ,ActionUTCDate
			   ,ActionUser
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
