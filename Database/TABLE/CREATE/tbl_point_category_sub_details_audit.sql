USE [CRS_V2]
GO

/****** Object:  Table [dbo].[tbl_point_category_sub_details_audit]    Script Date: 4/23/2024 3:39:05 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[tbl_point_category_sub_details_audit](
	[AuditId] [bigint] IDENTITY(1,1) NOT NULL,
	[Id] [bigint] NULL,
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
	[TriggerLogUser] [nvarchar](200) NULL,
	[TriggerAction] [varchar](100) NULL,
	[TriggerActionLocalDate] [datetime] NULL,
 CONSTRAINT [PK_tbl_point_category_sub_detail_audit] PRIMARY KEY CLUSTERED 
(
	[AuditId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO


