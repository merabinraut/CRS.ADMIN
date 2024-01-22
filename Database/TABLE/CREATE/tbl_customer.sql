USE [CRS]
GO

/****** Object:  Table [dbo].[tbl_customer]    Script Date: 09/10/2023 00:52:53 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[tbl_customer](
	[Sno] [bigint] IDENTITY(1,2) NOT NULL,
	[AgentId] [bigint] NULL,
	[NickName] [nvarchar](200) NULL,
	[FirstName] [nvarchar](100) NULL,
	[LastName] [nvarchar](100) NULL,
	[MobileNumber] [varchar](15) NULL,
	[DOB] [varchar](20) NULL,
	[EmailAddress] [varchar](512) NULL,
	[Gender] [varchar](10) NULL,
	[PreferredLocation] [varchar](10) NULL,
	[PostalCode] [varchar](100) NULL,
	[Prefecture] [varchar](10) NULL,
	[City] [varchar](100) NULL,
	[Street] [varchar](100) NULL,
	[ResidenceNumber] [varchar](100) NULL,
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

