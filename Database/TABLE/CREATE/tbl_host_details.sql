USE [CRS]
GO

/****** Object:  Table [dbo].[tbl_host_details]    Script Date: 09/10/2023 23:11:09 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[tbl_host_details](
	[Sno] [bigint] IDENTITY(1,2) NOT NULL,
	[AgentId] [bigint] NULL,
	[HostId] [bigint] NULL,
	[HostName] [varchar](512) NULL,
	[Position] [varchar](200) NULL,
	[Rank] [varchar](10) NULL,
	[DOB] [varchar](15) NULL,
	[ConstellationGroup] [varchar](10) NULL,
	[Height] [varchar](5) NULL,
	[BloodType] [varchar](10) NULL,
	[PreviousOccupation] [varchar](10) NULL,
	[LiquorStrength] [varchar](10) NULL,
	[Status] CHAR(1) NULL,
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

ALTER TABLE [dbo].[tbl_host_details] ADD  DEFAULT (getdate()) FOR [ActionDate]
GO

