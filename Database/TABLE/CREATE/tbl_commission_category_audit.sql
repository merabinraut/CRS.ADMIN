USE [CRS_V2]
GO

/****** Object:  Table [dbo].[tbl_commission_category_audit]    Script Date: 6/3/2024 3:47:24 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[tbl_commission_category_audit](
	[AuditId] [bigint] IDENTITY(1,2) NOT NULL,
	[Sno] VARCHAR(10) NULL,
	[CategoryId] [bigint] NULL,
	[CategoryName] [nvarchar](200) NULL,
	[Description] [nvarchar](512) NULL,
	[Status] [char](1) NULL,
	[ActionUser] [varchar](200) NULL,
	[ActionIP] [varchar](50) NULL,
	[ActionDate] [datetime] NULL,
	[IsDefault] [bit] NULL
	,TriggerLogUser NVARCHAR(200) NULL
		   ,TriggerAction NVARCHAR(100) NULL
		   ,TriggerActionLocalDate DATETIME NULL
		   ,TriggerActionUTCDate DATETIME NULL
)

