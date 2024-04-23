USE [CRS_V2]
GO

/****** Object:  Table [dbo].[tbl_point_category_details_audit]    Script Date: 4/23/2024 3:38:16 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[tbl_point_category_details_audit](
	[AuditId] [bigint] IDENTITY(1,1) NOT NULL,
	[Id] [bigint] NULL,
	[CategoryId] [bigint] NULL,
	[FromAmount] [decimal](18, 2) NULL,
	[ToAmount] [decimal](18, 2) NULL,
	[PointType] [varchar](20) NULL,
	[PointValue] [decimal](18, 2) NULL,
	[MinValue] [decimal](18, 2) NULL,
	[MaxValue] [decimal](18, 2) NULL,
	[ActionDate] [datetime] NULL,
	[ActionIp] [varchar](20) NULL,
	[ActionUser] [nvarchar](255) NULL,
	[Status] [varchar](5) NULL,
	[TriggerLogUser] [nvarchar](200) NULL,
	[TriggerAction] [varchar](100) NULL,
	[TriggerActionLocalDate] [datetime] NULL,
 CONSTRAINT [PK_tbl_PointCategory_Details_Audit] PRIMARY KEY CLUSTERED 
(
	[AuditId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO


