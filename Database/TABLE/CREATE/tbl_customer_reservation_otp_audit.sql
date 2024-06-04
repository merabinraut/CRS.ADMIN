USE [CRS_V2]
GO

/****** Object:  Table [dbo].[tbl_customer_reservation_otp_audit]    Script Date: 6/3/2024 5:27:08 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[tbl_customer_reservation_otp_audit](
	[AuditId] [bigint] IDENTITY(1,2) NOT NULL,
	[Sno] VARCHAR(10) NULL,
	[ReservationId] [bigint] NULL,
	[SMSId] [bigint] NULL,
	[CustomerId] [bigint] NULL,
	[MobileNumber] [varchar](15) NULL,
	[FullName] [nvarchar](256) NULL,
	[OTPCode] [varchar](6) NULL,
	[Status] [char](1) NULL,
	[CreatedBy] [varchar](200) NULL,
	[CreatedDate] [datetime] NULL,
	[CreatedIP] [varchar](50) NULL,
	[CreatedPlatform] [varchar](20) NULL,
	[UpdatedBy] [varchar](200) NULL,
	[UpdatedDate] [datetime] NULL,
	[UpdatedIP] [varchar](50) NULL,
	[UpdatedPlatform] [varchar](20) NULL
	,TriggerLogUser NVARCHAR(200) NULL
    ,TriggerAction NVARCHAR(100) NULL
    ,TriggerActionLocalDate DATETIME NULL
    ,TriggerActionUTCDate DATETIME NULL
)

