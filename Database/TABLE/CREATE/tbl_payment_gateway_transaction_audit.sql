USE [CRS_V2]
GO

/****** Object:  Table [dbo].[tbl_payment_gateway_transaction_audit]    Script Date: 6/4/2024 12:12:09 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[tbl_payment_gateway_transaction_audit](
	[AuditId] [bigint] IDENTITY(1,2) NOT NULL,
	[Sno] VARCHAR(10) NULL,
	[PaymentTxnId] [bigint] NULL,
	[PartnerTxnId] [bigint] NULL,
	[InvoiceId] [nvarchar](100) NULL,
	[ProcessId] [varchar](100) NULL,
	[FromAgentId] [varchar](10) NULL,
	[FromUserId] [varchar](10) NULL,
	[FromRoleType] [varchar](10) NULL,
	[ToAgentId] [varchar](10) NULL,
	[ToUserId] [varchar](10) NULL,
	[ToRoleType] [varchar](10) NULL,
	[Amount] [decimal](18, 3) NULL,
	[ServiceCharge] [decimal](18, 3) NULL,
	[TransactionType] [varchar](10) NULL,
	[Status] [char](1) NULL,
	[GatewayType] [nvarchar](200) NULL,
	[GatewayStatus] [nvarchar](200) NULL,
	[GatewayPaymentStatus] [nvarchar](200) NULL,
	[GatewayTxnId] [nvarchar](200) NULL,
	[GatewayRequest] [varchar](max) NULL,
	[GatewayResponse] [varchar](max) NULL,
	[GatewayProcessId] [nvarchar](200) NULL,
	[GatewayAdditionalDetail1] [nvarchar](max) NULL,
	[GatewayAdditionalDetail2] [nvarchar](max) NULL,
	[GatewayAdditionalDetail3] [nvarchar](max) NULL,
	[GatewayAdditionalDetail4] [nvarchar](max) NULL,
	[GatewayAdditionalDetail5] [nvarchar](max) NULL,
	[CheckCounter] [int] NULL,
	[ActionDate] [datetime] NULL,
	[ActionUTCDate] [datetime] NULL,
	[ActionUser] [nvarchar](200) NULL,
	[ActionIP] [varchar](50) NULL,
	[ActionPlatform] [varchar](20) NULL,
	TriggerLogUser	nvarchar(200) NULL,
	TriggerAction	nvarchar(100) NULL,
	TriggerActionLocalDate	datetime NULL,
	TriggerActionUTCDate	datetime NULL
)
