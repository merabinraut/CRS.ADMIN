USE [CRS]
GO

/****** Object:  Table [dbo].[tbl_bookmark]    Script Date: 23/11/2023 19:41:37 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[tbl_bookmark](
	[Sno] [bigint] IDENTITY(1,2) PRIMARY KEY,
	[BookmarkId] [bigint] NULL,
	[CustomerId] [bigint] NULL,
	[ClubId] [bigint] NULL,
	[HostId] [BIGINT] NULL,
	[AgentType] [varchar](10) NULL,
	[Status] [char](1) NULL,
	[CreatedBy] [varchar](200) NULL,
	[CreatedDate] [datetime] NULL,
	[CreatedPlatform] [varchar](20) NULL,
	[CreatedIP] [varchar](20) NULL,
	[UpdatedBy] [varchar](200) NULL,
	[UpdatedDate] [datetime] NULL,
	[UpdatedPlatform] [varchar](20) NULL,
	[UpdatedIP] [varchar](20) NULL,
)
