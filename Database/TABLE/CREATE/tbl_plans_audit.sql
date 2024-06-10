USE [CRS_V2]
GO

/****** Object:  Table [dbo].[tbl_plans_audit]    Script Date: 6/4/2024 12:31:32 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[tbl_plans_audit](
	[AuditId] [bigint] IDENTITY(1,2) PRIMARY KEY NOT NULL,
	[Sno] VARCHAR(10) NULL,
	[PlanId] [bigint] NULL,
	[PlanName] [nvarchar](250) NULL,
	[PlanType] [varchar](10) NULL,
	[PlanTime] [varchar](10) NULL,
	[Price] [decimal](18, 2) NULL,
	[StrikePrice] [decimal](18, 2) NULL,
	[Liquor] [varchar](10) NULL,
	[Nomination] [varchar](10) NULL,
	[Remarks] [nvarchar](500) NULL,
	[PlanStatus] [char](1) NULL,
	[PlanImage] [varchar](512) NULL,
	[AdditionalValue1] [nvarchar](300) NULL,
	[AdditionalValue2] [nvarchar](300) NULL,
	[AdditionalValue3] [nvarchar](300) NULL,
	[PlanImage2] [varchar](512) NULL,
	[NoOfPeople] [int] NULL,
	[PlanCategory] [varchar](10) NULL,
	[IsStrikeOut] [char](1) NULL,
	[ActionUser] [varchar](200) NULL,
	[ActionIp] [varchar](50) NULL,
	[ActionPlatform] [varchar](20) NULL,
	[ActionDate] [datetime] NULL,
	TriggerLogUser	nvarchar(200) NULL,
	TriggerAction	nvarchar(100) NULL,
	TriggerActionLocalDate	datetime NULL,
	TriggerActionUTCDate	datetime NULL
)

