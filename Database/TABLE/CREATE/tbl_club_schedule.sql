USE [CRS]
GO

/****** Object:  Table [dbo].[tbl_club_schedule]    Script Date: 08/12/2023 15:17:17 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[tbl_club_schedule](
	[Sno] [BIGINT] IDENTITY(1,2) NOT NULL,
	[ScheduleId] [BIGINT] NULL,
	[ClubId] [BIGINT] NULL,
	[DateValue] [VARCHAR](10) NULL,
	[ClubSchedule] [VARCHAR](10) NULL,
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

