USE [CRS_V2]
GO

/****** Object:  Table [dbo].[tbl_reservation_detail_audit]    Script Date: 6/4/2024 12:57:21 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[tbl_reservation_detail_audit](
	[AuditId] [bigint] IDENTITY(1,2) PRIMARY KEY NOT NULL,
	[Sno] VARCHAR(10) NULL,
	[ReservationId] [bigint] NULL,
	[InvoiceId] [varchar](100) NULL,
	[ClubId] [bigint] NOT NULL,
	[CustomerId] [bigint] NOT NULL,
	[ReservedDate] [datetime] NOT NULL,
	[VisitDate] [varchar](10) NULL,
	[VisitTime] [varchar](5) NULL,
	[NoOfPeople] [int] NULL,
	[PlanDetailId] [bigint] NULL,
	[HostDetailId] [bigint] NULL,
	[PaymentType] [varchar](10) NULL,
	[TransactionStatus] [char](1) NULL,
	[LocationVerificationStatus] [char](1) NULL,
	[OTPVerificationStatus] [char](1) NULL,
	[IsManual] [char](1) NULL,
	[ManualRemarkId] [varchar](3) NULL,
	[ActionDate] [datetime] NOT NULL,
	[ActionUser] [varchar](200) NULL,
	[ActionIP] [varchar](50) NULL,
	[ActionPlatform] [varchar](20) NULL,
	TriggerLogUser	nvarchar(200) NULL,
TriggerAction	nvarchar(100) NULL,
TriggerActionLocalDate	datetime NULL,
TriggerActionUTCDate	datetime NULL
)

