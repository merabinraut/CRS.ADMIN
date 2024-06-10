use [CRS_V2]
go


/****** Object:  Table [dbo].[tbl_club_admin_payment_transaction_detail_audit]    Script Date: 6/3/2024 3:39:43 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[tbl_club_admin_payment_transaction_detail_audit](
	[AuditId] [bigint] IDENTITY(1,2) PRIMARY KEY NOT NULL,
	[Sno] VARCHAR(10) NULL,
	[AgentId] [varchar](10) NULL,
	[UserId] [varchar](10) NULL,
	[TxnId] [varchar](20) NULL,
	[PaymentTxnId] [varchar](10) NULL,
	[ProcessId] [varchar](100) NULL,
	[PaymentMethod] [varchar](10) NULL,
	[Amount] [decimal](18, 2) NULL,
	[Status] [char](1) NULL,
	[ImageURL] [varchar](512) NULL,
	[Remarks] [nvarchar](512) NULL,
	[CreatedBy] [nvarchar](200) NULL,
	[CreatedDate] [datetime] NULL,
	[CreatedUTCDate] [datetime] NULL,
	[CreatedIP] [nvarchar](50) NULL,
	[CreatedPlatform] [nvarchar](20) NULL,
	[UpdatedBy] [nvarchar](200) NULL,
	[UpdatedDate] [datetime] NULL,
	[UpdatedUTCDate] [datetime] NULL,
	[UpdatedIP] [nvarchar](50) NULL,
	[UpdatedPlatform] [nvarchar](20) NULL
	 ,TriggerLogUser NVARCHAR(200) NULL
		   ,TriggerAction NVARCHAR(100) NULL
		   ,TriggerActionLocalDate DATETIME NULL
		   ,TriggerActionUTCDate DATETIME NULL
)


