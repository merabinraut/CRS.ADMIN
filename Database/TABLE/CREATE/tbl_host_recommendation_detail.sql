USE [CRS]
GO

/****** Object:  Table [dbo].[tbl_host_recommendation_detail]    Script Date: 03/12/2023 17:07:58 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[tbl_host_recommendation_detail](
	[Sno] [BIGINT] IDENTITY(1,2) NOT NULL,
	[RecommendationHostId] [BIGINT] NULL,
	[RecommendationId] [BIGINT] NULL,
	[ClubId] [BIGINT] NULL,
	[DisplayPageId] [VARCHAR](10) NULL,
	[HostId] [BIGINT] NULL,
	[OrderId] [VARCHAR](10) NULL,
	[Status] [CHAR](1) NULL,
	[CreatedBy] [VARCHAR](200) NULL,
	[CreatedDate] [DATETIME] NULL,
	[CreatedIP] [VARCHAR](50) NULL,
	[CreatedPlatform] [VARCHAR](20) NULL,
	[UpdatedBy] [VARCHAR](20) NULL,
	[UpdatedDate] [DATETIME] NULL,
	[UpdatedIP] [VARCHAR](50) NULL,
	[UpdatedPlatform] [VARCHAR](20) NULL,
PRIMARY KEY CLUSTERED 
(
	[Sno] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[tbl_host_recommendation_detail] ADD  DEFAULT (GETDATE()) FOR [CreatedDate]
GO

