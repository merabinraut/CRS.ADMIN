USE [CRS_V2]
GO

/****** Object:  Table [dbo].[tbl_club_plan_hold]    Script Date: 4/23/2024 3:47:27 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[tbl_club_plan_hold](
	[holdId] [bigint] IDENTITY(1,1) NOT NULL,
	[ClubholdId] [bigint] NOT NULL,
	[ClubId] [bigint] NULL,
	[ClubPlanType] [varchar](10) NOT NULL,
	[ClubPlanTypeId] [varchar](10) NOT NULL,
	[Description] [varchar](50) NULL,
	[PlanListId] [bigint] NOT NULL,
	[CreatedDate] [datetime] NULL,
	[CreatedBy] [nvarchar](200) NULL,
	[CreatedPlatform] [varchar](20) NULL,
	[UpdatedBy] [nvarchar](200) NULL,
	[UpdatedDate] [nchar](10) NULL,
	[UpdatedIP] [varchar](50) NULL,
	[UpdatedPlatform] [varchar](20) NULL,
	[holdType] [varchar](5) NULL,
	[holdStatus] [varchar](5) NULL,
 CONSTRAINT [PK_tbl_club_plan_hold] PRIMARY KEY CLUSTERED 
(
	[holdId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO


