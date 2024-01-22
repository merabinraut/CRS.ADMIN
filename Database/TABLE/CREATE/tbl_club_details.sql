USE [CRS]
GO

/****** Object:  Table [dbo].[tbl_club_details]    Script Date: 10/10/2023 22:26:50 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[tbl_club_details](
	[Sno] [bigint] IDENTITY(1,2) NOT NULL,
	[AgentId] [bigint] NULL,
	[FirstName] [varchar](256) NULL,
	[MiddleName] [varchar](256) NULL,
	[LastName] [varchar](256) NULL,
	[Email] [varchar](512) NULL,
	[MobileNumber] [varchar](15) NULL,
	[ClubName1] [varchar](512) NULL,
	[ClubName2] [varchar](512) NULL,
	[BusinessType] [varchar](10) NULL,
	[GroupName] [varchar](512) NULL,
	[Description] [nvarchar](512) NULL,
	[LocationURL] [varchar](800) NULL,
	[Longitude] [varchar](100) NULL,
	[Latitude] [varchar](100) NULL,
	[Status] [char](1) NULL,
	[Logo] [varchar](max) NULL,
	[CoverPhoto] [varchar](max) NULL,
	[BusinessCertificate] [varchar](max) NULL,
	[Gallery] [varchar](max) NULL,
	[CommissionId] BIGINT NULL,
	[ActionUser] [varchar](200) NULL,
	[ActionIP] [varchar](50) NULL,
	[ActionPlatform] [varchar](20) NULL,
	[ActionDate] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[Sno] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

