USE [CRS]
GO

/****** Object:  Table [dbo].[tbl_display_mainpage]    Script Date: 07/12/2023 10:29:50 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[tbl_display_mainpage_hold](
	[Sno] [bigint] IDENTITY(1,2) PRIMARY KEY NOT NULL,
	[RecommendationHoldId] [bigint] NULL,
	[OrderId] [bigint] NULL,
	[DisplayPageId] [varchar](10) NULL,
	[Status] [char](1) NULL,
	[CreatedBy] [varchar](200) NULL,
	[CreatedDate] [datetime] NULL,
	[CreatedIP] [varchar](50) NULL,
	[CreatedPlatform] [varchar](20) NULL,
	[UpdatedBy] [varchar](200) NULL,
	[UpdatedDate] [datetime] NULL,
	[UpdatedIP] [varchar](50) NULL,
	[UpdatedPlatform] [varchar](20) NULL
)

