USE [CRS_V2]
GO

/****** Object:  Table [dbo].[tbl_website_details_hold]    Script Date: 4/23/2024 3:48:28 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[tbl_website_details_hold](
	[holdId] [bigint] IDENTITY(1,2) NOT NULL,
	[clubholdId] [bigint] NOT NULL,
	[AgentId] [bigint] NULL,
	[HostId] [bigint] NULL,
	[WebsiteLink] [varchar](max) NULL,
	[TiktokLink] [varchar](max) NULL,
	[TwitterLink] [varchar](max) NULL,
	[InstagramLink] [varchar](max) NULL,
	[ActionUser] [varchar](200) NULL,
	[ActionIP] [varchar](50) NULL,
	[ActionPlatform] [varchar](20) NULL,
	[ActionDate] [datetime] NULL,
	[RoleId] [bigint] NULL,
	[FacebookLink] [varchar](max) NULL,
	[Line] [varchar](max) NULL,
	[holdType] [varchar](5) NULL,
	[holdStatus] [varchar](5) NULL,
 CONSTRAINT [PK_tbl_website_details_hold] PRIMARY KEY CLUSTERED 
(
	[holdId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO


