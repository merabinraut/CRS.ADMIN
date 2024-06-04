USE [CRS_V2]
GO

/****** Object:  Table [dbo].[tbl_customer_hold_audit]    Script Date: 6/3/2024 4:16:05 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[tbl_customer_hold_audit](
	AuditId [bigint] IDENTITY(1,2) PRIMARY KEY NOT NULL,
	[Sno] VARCHAR(10) NULL,
	[NickName] [nvarchar](400) NULL,
	[MobileNumber] [varchar](15) NULL,
	[Status] [char](1) NULL,
	[CreatedBy] [varchar](200) NULL,
	[CreatedIP] [varchar](50) NULL,
	[CreatedPlatform] [varchar](20) NULL,
	[CreatedDate] [datetime] NULL,
	[UpdatedBy] [varchar](200) NULL,
	[UpdatedIP] [varchar](50) NULL,
	[UpdatedPlatform] [varchar](20) NULL,
	[UpdatedDate] [datetime] NULL
	,TriggerLogUser NVARCHAR(200) NULL
		   ,TriggerAction NVARCHAR(100) NULL
		   ,TriggerActionLocalDate DATETIME NULL
		   ,TriggerActionUTCDate DATETIME NULL
)

