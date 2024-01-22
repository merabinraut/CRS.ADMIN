USE [CRS];
GO

/****** Object:  Table [dbo].[tbl_review_and_rating_qna]    Script Date: 27/11/2023 12:40:26 ******/
SET ANSI_NULLS ON;
GO

SET QUOTED_IDENTIFIER ON;
GO

CREATE TABLE [dbo].[tbl_review_and_rating_qna]
(
    [Sno] [BIGINT] IDENTITY(1, 2) NOT NULL,
    [QnaId] [BIGINT] NULL,
    ReviewId BIGINT NULL,
    [DichotomousQuestionId] [VARCHAR](10) NULL,
    [DichotomousAnswerId] [VARCHAR](10) NULL,
    [ActionUser] [VARCHAR](200) NULL,
    [ActionDate] [DATETIME] NULL,
    [ActionIP] [VARCHAR](20) NULL,
    [ActionPlatform] [VARCHAR](20) NULL,
    PRIMARY KEY CLUSTERED ([Sno] ASC)
    WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON,
          ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF
         ) ON [PRIMARY]
) ON [PRIMARY];
GO

