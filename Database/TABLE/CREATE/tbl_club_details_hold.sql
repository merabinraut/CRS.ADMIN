USE [CRS_V2]
GO

/****** Object:  Table [dbo].[tbl_club_details_hold]    Script Date: 4/23/2024 3:46:58 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[tbl_club_details_hold](
	[holdId] [bigint] IDENTITY(1,2) NOT NULL,
	[AgentId] [bigint] NULL,
	[FirstName] [nvarchar](256) NULL,
	[MiddleName] [nvarchar](256) NULL,
	[LastName] [nvarchar](256) NULL,
	[Email] [varchar](512) NULL,
	[MobileNumber] [varchar](15) NULL,
	[ClubName1] [nvarchar](512) NULL,
	[ClubName2] [nvarchar](max) NULL,
	[BusinessType] [varchar](10) NULL,
	[GroupName] [nvarchar](512) NULL,
	[Description] [nvarchar](512) NULL,
	[LocationURL] [varchar](800) NULL,
	[Longitude] [varchar](100) NULL,
	[Latitude] [varchar](100) NULL,
	[Status] [char](1) NULL,
	[Logo] [varchar](max) NULL,
	[CoverPhoto] [varchar](max) NULL,
	[BusinessCertificate] [varchar](max) NULL,
	[Gallery] [varchar](max) NULL,
	[CommissionId] [bigint] NULL,
	[ActionUser] [varchar](200) NULL,
	[ActionIP] [varchar](50) NULL,
	[ActionPlatform] [varchar](20) NULL,
	[ActionDate] [datetime] NULL,
	[LocationId] [bigint] NULL,
	[ClubOpeningTime] [varchar](5) NULL,
	[ClubClosingTime] [varchar](5) NULL,
	[InputRole] [varchar](256) NULL,
	[LastOrderTime] [varchar](5) NULL,
	[LastEntrySyokai] [varchar](5) NULL,
	[Holiday] [varchar](5) NULL,
	[InputPlan] [varchar](10) NULL,
	[InputPlanPrice] [varchar](10) NULL,
	[Tax] [varchar](10) NULL,
	[InputZip] [varchar](256) NULL,
	[InputPrefecture] [varchar](256) NULL,
	[InputCity] [nvarchar](200) NULL,
	[InputStreet] [nvarchar](200) NULL,
	[InputHouseNo] [nvarchar](50) NULL,
	[CompanyName] [nvarchar](512) NULL,
	[CommissionCategoryId] [bigint] NULL,
	[RegularPrice] [nvarchar](500) NULL,
	[NominationFee] [nvarchar](500) NULL,
	[AccompanyingFee] [nvarchar](500) NULL,
	[OnSiteNominationFee] [nvarchar](500) NULL,
	[VariousDrinksFee] [nvarchar](500) NULL,
	[LandLineNumber] [varchar](20) NULL,
	[ClubAddress] [nvarchar](200) NULL,
	[GroupNamekatakana] [varchar](100) NULL,
	[BusinessLicensePhoto] [varchar](1000) NULL,
	[LicenseIssuedDate] [datetime] NULL,
	[BusinessLicenseNumber] [nvarchar](100) NULL,
	[CompanyAddress] [nvarchar](500) NULL,
	[KYCDocument] [nvarchar](1000) NULL,
	[CEOName] [nvarchar](100) NULL,
	[ClosingDate] [varchar](20) NULL,
	[LandLineCode] [varchar](10) NULL,
	[MaxReservationPerDay] [int] NULL,
	[updatedUser] [varchar](200) NULL,
	[updatedIP] [varchar](50) NULL,
	[updatedPlatform] [varchar](20) NULL,
	[updatedDate] [datetime] NULL,
	[holdType] [varchar](5) NULL,
	[holdStatus] [varchar](5) NULL,
	[LoginId] [nvarchar](100) NULL,
 CONSTRAINT [PK_tbl_club_details_hold] PRIMARY KEY CLUSTERED 
(
	[holdId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO


