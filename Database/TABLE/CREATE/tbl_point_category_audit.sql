USE [CRS_V2]
GO

/****** Object:  Table [dbo].[tbl_point_category_audit]    Script Date: 4/23/2024 3:37:22 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[tbl_point_category_audit](
	[AuditId] [bigint] IDENTITY(1,1) NOT NULL,
	[Id] [bigint] NULL,
	[CategoryName] [nvarchar](100) NULL,
	[Description] [nvarchar](max) NULL,
	[RoleType] [int] NULL,
	[ActionDate] [datetime] NULL,
	[ActionIp] [varchar](50) NULL,
	[ActionUser] [varchar](50) NULL,
	[Status] [varchar](5) NULL,
	[TriggerLogUser] [nvarchar](200) NULL,
	[TriggerAction] [varchar](100) NULL,
	[TriggerActionLocalDate] [datetime] NULL,
 CONSTRAINT [PK_PointCategoryaudit] PRIMARY KEY CLUSTERED 
(
	[AuditId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO


