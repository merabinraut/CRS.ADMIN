use [CRS_V2]
go

/****** Object:  Table [dbo].[tbl_affiliate]    Script Date: 6/3/2024 2:49:23 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[tbl_affiliate_audit](
	[AuditId] [bigint] IDENTITY(1,2) PRIMARY KEY NOT NULL,
	[Sno] VARCHAR(10) NULL,
	[AgentId] [bigint] NULL,
	[FirstName] [nvarchar](200) NULL,
	[LastName] [nvarchar](200) NULL,
	[MobileNumber] [varchar](15) NULL,
	[EmailAddress] [varchar](512) NULL,
	[DOB] [varchar](20) NULL,
	[Gender] [varchar](10) NULL,
	[ProfileImage] [varchar](512) NULL,
	[PointsCategoryId] [bigint] NULL,
	[ActionUser] [varchar](200) NULL,
	[ActionIP] [varchar](50) NULL,
	[ActionPlatform] [varchar](20) NULL,
	[ActionDate] [datetime] NULL
   ,TriggerLogUser NVARCHAR(200) NULL
   ,TriggerAction NVARCHAR(100) NULL
   ,TriggerActionLocalDate DATETIME NULL
   ,TriggerActionUTCDate  DATETIME NULL
)


