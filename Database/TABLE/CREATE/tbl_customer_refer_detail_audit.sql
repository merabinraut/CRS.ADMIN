USE [CRS_V2]
GO

/****** Object:  Table [dbo].[tbl_customer_refer_detail_audit]    Script Date: 6/3/2024 5:20:26 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[tbl_customer_refer_detail_audit](
	[AuditId] [bigint] IDENTITY(1,2) NOT NULL,
	[Sno] VARCHAR(10) NULL,
	[ReferDetailId] [bigint] NULL,
	[ReferId] [bigint] NULL,
	[AffiliateId] [bigint] NULL,
	[CustomerId] [bigint] NULL,
	[Status] [char](1) NULL,
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

