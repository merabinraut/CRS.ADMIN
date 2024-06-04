USE [CRS_V2]
GO

/****** Object:  Table [dbo].[tbl_reservation_plan_detail_audit]    Script Date: 6/4/2024 2:45:45 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[tbl_reservation_plan_detail_audit](
	[AuditId] [bigint] IDENTITY(1,2) PRIMARY KEY NOT NULL,
	[Sno] VARCHAR(10) NULL,
	[PlanDetailId] [bigint] NULL,
	[PlanName] [nvarchar](200) NULL,
	[PlanType] [varchar](10) NULL,
	[PlanTime] [varchar](10) NULL,
	[Price] [decimal](18, 2) NULL,
	[Liquor] [varchar](10) NULL,
	[Nomination] [varchar](10) NULL,
	[Remarks] [nvarchar](500) NULL,
	[ReservationId] [bigint] NULL,
	[PlanId] [bigint] NULL,
	TriggerLogUser	nvarchar(200) NULL,
	TriggerAction	nvarchar(100) NULL,
	TriggerActionLocalDate	datetime NULL,
	TriggerActionUTCDate	datetime NULL
	)

