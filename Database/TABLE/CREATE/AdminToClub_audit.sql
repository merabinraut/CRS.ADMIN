USE [CRS_V2]
GO

/****** Object:  Table [dbo].[AdminToClub_audit]    Script Date: 6/4/2024 3:08:21 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[AdminToClub_audit](
	[AuditId] [bigint] IDENTITY(1,1) NOT NULL,
	[TransctionId] VARCHAR(10) NULL,
	[Amount] [decimal](16, 2) NULL,
	[Points] [decimal](16, 2) NULL,
	[ClubId] [bigint] NULL,
	[BankBoucher] [varchar](1000) NULL,
	[Remark] [nvarchar](1000) NULL,
	[Status] [char](1) NULL,
	[ActionUserId] [bigint] NULL,
	[ActionDate] [datetime] NULL,
	TriggerLogUser	nvarchar(200) NULL,
	TriggerAction	nvarchar(100) NULL,
	TriggerActionLocalDate	datetime NULL,
	TriggerActionUTCDate	datetime NULL
)

