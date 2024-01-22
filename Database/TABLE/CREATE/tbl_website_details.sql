USE [CRS]
GO

/****** Object:  Table [dbo].[tbl_website_details]    Script Date: 09/10/2023 23:35:15 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[tbl_website_details](
	[Sno] [bigint] IDENTITY(1,2) NOT NULL,
	[AgentId] [bigint] NULL,
	[HostId] [BigINt] NULL,
	[WebsiteLink] [varchar](max) NULL,
	[TiktokLink] [varchar](max) NULL,
	[TwitterLink] [varchar](max) NULL,
	[InstagramLink] [varchar](max) NULL,
	[ActionUser] [varchar](200) NULL,
	[ActionIP] [varchar](20) NULL,
	[ActionPlatform] [varchar](20) NULL,
	[ActionDate] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[Sno] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

