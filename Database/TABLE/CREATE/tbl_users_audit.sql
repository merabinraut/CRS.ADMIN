USE [CRS_V2]
GO

/****** Object:  Table [dbo].[tbl_users_audit]    Script Date: 6/4/2024 3:03:32 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[tbl_users_audit](
	[AuditId] [bigint] IDENTITY(1,2) PRIMARY KEY NOT NULL,
	[Sno] VARCHAR(10) NULL,
	[UserId] [bigint] NULL,
	[RoleId] [bigint] NULL,
	[AgentId] [bigint] NULL,
	[LoginId] [varchar](200) NULL,
	[Password] [varbinary](max) NULL,
	[Status] [char](1) NULL,
	[IsPrimary] [char](1) NULL,
	[FailedLoginAttempt] [int] NULL,
	[IsPasswordForceful] [char](1) NULL,
	[LastPasswordChangedDate] [datetime] NULL,
	[LastLoginDate] [datetime] NULL,
	[Session] [varchar](512) NULL,
	[RoleType] [bigint] NULL,
	[IsForcefulLogout] [char](1) NULL,
	[ActionUser] [varchar](200) NULL,
	[ActionIP] [varchar](50) NULL,
	[ActionPlatform] [varchar](20) NULL,
	[ActionDate] [datetime] NULL,
	TriggerLogUser	nvarchar(200) NULL,
	TriggerAction	nvarchar(100) NULL,
	TriggerActionLocalDate	datetime NULL,
	TriggerActionUTCDate	datetime NULL
)