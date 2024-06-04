USE [CRS_V2]
GO

/****** Object:  Table [dbo].[tbl_point_category_sub_details_audit]    Script Date: 6/4/2024 12:52:13 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[tbl_point_category_sub_details_audit](
	[AuditId] [bigint] IDENTITY(1,2) PRIMARY KEY NOT NULL,
	[Id] VARCHAR(10) NULL,
	[CategoryId] [bigint] NULL,
	[PointValue2] [decimal](18, 2) NULL,
	[PointValue3] [decimal](18, 2) NULL,
	[PointType2] [varchar](10) NULL,
	[PointType3] [varchar](10) NULL,
	[MinValue2] [decimal](18, 2) NULL,
	[MaxValue2] [decimal](18, 2) NULL,
	[MinValue3] [decimal](18, 2) NULL,
	[MaxValue3] [decimal](18, 2) NULL,
	[ActionDate] [datetime] NULL,
	[ActionIp] [varchar](20) NULL,
	TriggerLogUser	nvarchar(200) NULL,
	TriggerAction	nvarchar(100) NULL,
	TriggerActionLocalDate	datetime NULL, 
	TriggerActionUTCDate	datetime NULL
)


