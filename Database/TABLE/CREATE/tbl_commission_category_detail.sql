USE [CRS]
GO

/****** Object:  Table [dbo].[tbl_commission_category_detail]    Script Date: 10/10/2023 22:16:33 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[tbl_commission_category_detail](
	[Sno] [bigint] IDENTITY(1,2) NOT NULL,
	[CategoryDetailId] [bigint] NULL,
	[CategoryId] [bigint] NULL,
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
	[ActionPlatform] [varchar](20) NULL,
	[ActionDate] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[Sno] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

