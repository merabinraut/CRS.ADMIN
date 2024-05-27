USE [CRS_V2]
GO

/****** Object:  Table [dbo].[tbl_Club_Representative_hold]    Script Date: 4/23/2024 3:47:54 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[tbl_Club_Representative_hold](
	[holdId] [bigint] IDENTITY(1,1) NOT NULL,
	[ClubholdId] [bigint] NOT NULL,
	[ClubId] [bigint] NULL,
	[ContactName] [varchar](100) NULL,
	[MobileNumber] [varchar](20) NULL,
	[EmailAddress] [varchar](255) NULL,
	[ActionIP] [varchar](50) NULL,
	[ActionUser] [varchar](200) NULL,
	[ActionPlatform] [varchar](20) NULL,
	[Status] [char](1) NULL,
	[ActionDate] [datetime] NULL,
	[holdType] [varchar](5) NULL,
	[holdStatus] [varchar](5) NULL,
	[RepresentativeId] [bigint] NULL,
 CONSTRAINT [PK_tbl_Club_Representative_hold] PRIMARY KEY CLUSTERED 
(
	[holdId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO


