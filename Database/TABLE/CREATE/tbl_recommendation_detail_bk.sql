USE [CRS]
GO

/****** Object:  Table [dbo].[tbl_recommendation_detail]    Script Date: 26/11/2023 20:05:51 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[tbl_recommendation_detail](
	[Sno] [BIGINT] IDENTITY(1,2) NOT NULL,
	[RecommendationId] [BIGINT] NULL,
	[LocationId] [BIGINT] NULL,
	[ClubId] [BIGINT] NULL,
	[GroupId] [VARCHAR](10) NULL,
	[OrderId] [VARCHAR](10) NULL,
	[Status] [CHAR](1) NULL,
	[CreatedBy] [VARCHAR](200) NULL,
	[CreatedDate] [DATETIME] NULL,
	[CreatedIP] [VARCHAR](50) NULL,
	[CreatedPlatform] [VARCHAR](50) NULL,
	[UpdatedBy] [VARCHAR](200) NULL,
	[UpdatedDate] [DATETIME] NULL,
	[UpdatedIP] [VARCHAR](50) NULL,
	[UpdatedPlatform] [VARCHAR](50) NULL,
PRIMARY KEY CLUSTERED 
(
	[Sno] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO



