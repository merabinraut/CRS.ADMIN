USE [CRS_V2]
GO

/****** Object:  Table [dbo].[tbl_host_details_audit]    Script Date: 6/4/2024 11:22:41 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[tbl_host_details_audit](
	[AuditId] [bigint] IDENTITY(1,2) NOT NULL,
	[Sno] VARCHAR(10) NULL,
	[AgentId] [bigint] NULL,
	[HostId] [bigint] NULL,
	[HostName] [nvarchar](512) NULL,
	[Position] [nvarchar](200) NULL,
	[Rank] [varchar](10) NULL,
	[DOB] [varchar](15) NULL,
	[ConstellationGroup] [varchar](10) NULL,
	[Height] [varchar](5) NULL,
	[BloodType] [varchar](10) NULL,
	[PreviousOccupation] [varchar](10) NULL,
	[LiquorStrength] [varchar](10) NULL,
	[Status] [char](1) NULL,
	[ImagePath] [varchar](512) NULL,
	[Address] [nvarchar](200) NULL,
	[HostNameJapanese] [nvarchar](512) NULL,
	[HostIntroduction] [nvarchar](1000) NULL,
	[Title] [nvarchar](250) NULL,
	[OtherPosition] [nvarchar](50) NULL,
	[Thumbnail] [nvarchar](1000) NULL,
	[Icon] [nvarchar](1000) NULL,
	[ActionUser] [varchar](200) NULL,
	[ActionIP] [varchar](50) NULL,
	[ActionPlatform] [varchar](20) NULL,
	[ActionDate] [datetime] NULL,
	TriggerLogUser	nvarchar(200) NULL,
	TriggerAction	nvarchar(100) NULL,
	TriggerActionLocalDate	datetime NULL,
	TriggerActionUTCDate	datetime NULL
)


