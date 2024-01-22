USE [CRS]
GO

/****** Object:  Table [dbo].[tbl_affiliate_referral_detail]    Script Date: 13/12/2023 23:47:26 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[tbl_affiliate_referral_detail](
	[Sno] [BIGINT] IDENTITY(1,2) NOT NULL,
	[ReferralId] [BIGINT] NULL,
	[ReferId] [BIGINT] NULL,
	[ReferDetailId] [BIGINT] NULL,
	[AffiliateId] [BIGINT] NULL,
	[CustomerId] [BIGINT] NULL,
	[ReservationId] [BIGINT] NULL,
	[PaymentId] [BIGINT] NULL,
	[CommissionAmount] [DECIMAL](18, 2) NULL,
	[Status] [CHAR](1) NULL,
	[Remarks] [NVARCHAR](1000) NULL,
	[CreatedBy] [VARCHAR](200) NULL,
	[CreatedDate] [DATETIME] NULL,
	[CreatedIP] [VARCHAR](50) NULL,
	[CreatedPlatform] [VARCHAR](20) NULL,
	[UpdatedBy] [VARCHAR](200) NULL,
	[UpdatedDate] [DATETIME] NULL,
	[UpdatedIP] [VARCHAR](50) NULL,
	[UpdatedPlatform] [VARCHAR](20) NULL,
PRIMARY KEY CLUSTERED 
(
	[Sno] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

