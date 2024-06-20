use [CRS_V2]
go
/****** Object:  Table [dbo].[tbl_affiliate_referral_detail_audit]    Script Date: 6/3/2024 3:21:05 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[tbl_affiliate_referral_detail_audit](
	AuditId [bigint] IDENTITY(1,2) NOT NULL,
	[Sno] VARCHAR(10) NULL,
	[ReferralId] [bigint] NULL,
	[ReferId] [bigint] NULL,
	[ReferDetailId] [bigint] NULL,
	[AffiliateId] [bigint] NULL,
	[CustomerId] [bigint] NULL,
	[ReservationId] [bigint] NULL,
	[PaymentId] [bigint] NULL,
	[CommissionAmount] [decimal](18, 2) NULL,
	[Status] [char](1) NULL,
	[Remarks] [nvarchar](1000) NULL,
	[CreatedBy] [varchar](200) NULL,
	[CreatedDate] [datetime] NULL,
	[CreatedIP] [varchar](50) NULL,
	[CreatedPlatform] [varchar](20) NULL,
	[UpdatedBy] [varchar](200) NULL,
	[UpdatedDate] [datetime] NULL,
	[UpdatedIP] [varchar](50) NULL,
	[UpdatedPlatform] [varchar](20) NULL
	,TriggerLogUser NVARCHAR(200) NULL
		   ,TriggerAction NVARCHAR(100) NULL
		   ,TriggerActionLocalDate DATETIME NULL
		   ,TriggerActionUTCDate DATETIME NULL
)

