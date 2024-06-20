use [CRS_V2]
go


/****** Object:  Table [dbo].[tbl_affiliate_hold_audit]    Script Date: 6/3/2024 3:01:25 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[tbl_affiliate_hold_audit](
	AuditId [bigint] IDENTITY(1,2) PRIMARY KEY NOT NULL,
	[Sno] VARCHAR(10) NULL,
	[HoldAgentId] [bigint] NULL,
	[FirstName] [nvarchar](200) NULL,
	[LastName] [nvarchar](200) NULL,
	[MobileNumber] [varchar](15) NULL,
	[EmailAddress] [varchar](512) NULL,
	[DOB] [varchar](20) NULL,
	[Gender] [varchar](10) NULL,
	[SnsType] [varchar](10) NULL,
	[SnsURLLink] [varchar](max) NULL,
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

