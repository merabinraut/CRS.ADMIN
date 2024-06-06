USE [CRS_V2]
GO

/****** Object:  Table [dbo].[tbl_reservation_host_detail_audit]    Script Date: 6/4/2024 2:38:32 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[tbl_reservation_host_detail_audit](
	[AuditId] [bigint] IDENTITY(1,2) PRIMARY KEY NOT NULL,
	[Sno] VARCHAR(10) NULL,
	[HostDetailId] [bigint] NULL,
	[ReservationId] [bigint] NULL,
	[HostId] [bigint] NULL,
	[CreatedDate] [datetime] NOT NULL,
	[CreatedUser] [varchar](200) NULL,
	[CreatedIP] [varchar](50) NULL,
	[CreatedPlatform] [varchar](20) NULL,
	[UpdatedDate] [datetime] NOT NULL,
	[UpdatedUser] [varchar](200) NULL,
	[UpdatedIP] [varchar](50) NULL,
	[UpdatedPlatform] [varchar](20) NULL,
	TriggerLogUser	nvarchar(200) NULL,
	TriggerAction	nvarchar(100) NULL,
	TriggerActionLocalDate	datetime NULL,
	TriggerActionUTCDate	datetime NULL
)


