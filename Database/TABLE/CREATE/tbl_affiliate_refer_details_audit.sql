USE [CRS_V2]
GO

/****** Object:  Table [dbo].[tbl_affiliate_refer_details]    Script Date: 6/3/2024 3:13:13 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[tbl_affiliate_refer_details_audit](
	AuditId BIGINT IDENTITY(1,2) PRIMARY KEY NOT NULL,
	[Sno] VARCHAR(20) NULL,
	[ReferId] [bigint] NULL,
	[AffiliateId] [bigint] NULL,
	[SnsId] [varchar](10) NULL,
	[TotalClickCount] [int] NULL,
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