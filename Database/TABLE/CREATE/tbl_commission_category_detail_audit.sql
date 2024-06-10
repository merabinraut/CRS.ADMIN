USE [CRS_V2]
GO

/****** Object:  Table [dbo].[tbl_commission_category_detail_audit]    Script Date: 6/3/2024 3:52:52 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[tbl_commission_category_detail_audit](
	[AuditId] [bigint] IDENTITY(1,2) PRIMARY KEY NOT NULL,
	[Sno] VARCHAR(10) NULL,
	[CategoryDetailId] [bigint] NULL,
	[CategoryId] [bigint] NULL,
	[AdminCommissionTypeId] [bigint] NULL,
	[FromAmount] [decimal](18, 2) NULL,
	[ToAmount] [decimal](18, 2) NULL,
	[CommissionType] [varchar](10) NULL,
	[CommissionValue] [decimal](18, 2) NULL,
	[CommissionPercentageType] [char](1) NULL,
	[MinCommissionValue] [decimal](18, 2) NULL,
	[MaxCommissionValue] [decimal](18, 2) NULL,
	[Status] [char](1) NULL,
	[ActionUser] [varchar](200) NULL,
	[ActionIP] [varchar](50) NULL,
	[ActionDate] [datetime] NULL
	   ,TriggerLogUser NVARCHAR(200) NULL
		   ,TriggerAction NVARCHAR(100) NULL
		   ,TriggerActionLocalDate DATETIME NULL
		   ,TriggerActionUTCDate DATETIME NULL
)

