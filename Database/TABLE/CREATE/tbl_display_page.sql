USE [CRS]
GO

/****** Object:  Table [dbo].[tbl_display_page]    Script Date: 18/10/2023 12:46:06 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[tbl_display_page](
	[Sno] [BIGINT] IDENTITY(1,2) NOT NULL,
	[RecommendationId] [BIGINT] NULL,
	[DisplayPageId] VARCHAR(10) NULL,
	[Status] [CHAR](1) NULL,
	[CreatedBy] [VARCHAR](200) NULL,
	[CreatedDate] [DATETIME] NULL,
	[CreatedIP] [VARCHAR](50) NULL,
	[CreatedPlatform] [VARCHAR](20) NULL,
	[UpdatedBy] [VARCHAR](200) NULL,
	[UpdatedDate] [DATETIME] NULL,
	[UpdatedIP] [VARCHAR](50) NULL,
	[UpdatedPlatform] [VARCHAR](20) NULL,
PRIMARY KEY CLUSTERED 
(
	[Sno] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

