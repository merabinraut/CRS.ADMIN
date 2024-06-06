USE [CRS.UAT_V2]
GO

/****** Object:  Table [dbo].[tbl_customer_audit]    Script Date: 6/3/2024 4:08:30 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[tbl_customer_audit](
	[AuditId] [bigint] IDENTITY(1,2) PRIMARY KEY NOT NULL,
	[Sno] VARCHAR(10) NULL,
	[AgentId] [bigint] NULL,
	[NickName] [nvarchar](200) NULL,
	[FirstName] [nvarchar](100) NULL,
	[LastName] [nvarchar](100) NULL,
	[MobileNumber] [varchar](15) NULL,
	[DOB] [varchar](20) NULL,
	[EmailAddress] [varchar](512) NULL,
	[Gender] [varchar](10) NULL,
	[PreferredLocation] [varchar](10) NULL,
	[PostalCode] [varchar](100) NULL,
	[Prefecture] [varchar](10) NULL,
	[City] [varchar](100) NULL,
	[Street] [varchar](100) NULL,
	[ResidenceNumber] [varchar](100) NULL,
	[ProfileImage] [varchar](500) NULL,
	[PointsCategoryId] [bigint] NULL,
	[ActionUser] [varchar](200) NULL,
	[ActionIP] [varchar](50) NULL,
	[ActionPlatform] [varchar](20) NULL,
	[ActionDate] [datetime] NULL
	,TriggerLogUser NVARCHAR(200) NULL
    ,TriggerAction NVARCHAR(100) NULL
    ,TriggerActionLocalDate DATETIME NULL
    ,TriggerActionUTCDate DATETIME NULL
)
