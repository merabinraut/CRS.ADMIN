USE [CRS_V2]
GO

/****** Object:  Table [dbo].[tbl_reservation_transaction_detail_audit]    Script Date: 6/4/2024 2:50:37 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[tbl_reservation_transaction_detail_audit](
	[AuditId] [bigint] IDENTITY(1,2) PRIMARY KEY NOT NULL,
	[Sno] VARCHAR(10) NULL,
	[ReservationTxnId] [bigint] NULL,
	[ReservationId] [bigint] NULL,
	[ReservationType] [varchar](10) NULL,
	[PlanAmount] [decimal](18, 2) NULL,
	[TotalPlanAmount] [decimal](18, 2) NULL,
	[TotalClubPlanAmount] [decimal](18, 2) NULL,
	[CommissionId] [varchar](10) NULL,
	[AdminPlanCommissionAmount] [decimal](18, 2) NULL,
	[TotalAdminPlanCommissionAmount] [decimal](18, 2) NULL,
	[AdminCommissionAmount] [decimal](18, 2) NULL,
	[TotalAdminCommissionAmount] [decimal](18, 2) NULL,
	[TotalAdminPayableAmount] [decimal](18, 2) NULL,
	[Remarks] [nvarchar](512) NULL,
	[AdminPaymentStatus] [char](1) NULL,
	[ActionUser] [nvarchar](200) NULL,
	[ActionDate] [datetime] NULL,
	[ActionIP] [varchar](50) NULL,
	[ActionPlatform] [nvarchar](20) NULL,
	TriggerLogUser	nvarchar(200) NULL,
	TriggerAction	nvarchar(100) NULL,
	TriggerActionLocalDate	datetime NULL,
	TriggerActionUTCDate	datetime NULL
)

