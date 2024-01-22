USE [CRS]
GO

/****** Object:  Table [dbo].[tbl_users]    Script Date: 06/10/2023 06:51:04 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[tbl_users](
	[Sno] [bigint] IDENTITY(1,2) NOT NULL,
	[AgentId] [bigint] NULL,
	[UserId] [bigint] NULL,
	[LoginId] [varchar](200) NULL,
	[Password] [varbinary](MAX) NULL,
	[Status] [char](1) NULL,
	[IsPrimary] [char](1) NULL,
	[ActionUser] [varchar](200) NULL,
	[ActionIP] [varchar](50) NULL,
	[ActionPlatform] [varchar](20) NULL,
	[ActionDate] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[Sno] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

