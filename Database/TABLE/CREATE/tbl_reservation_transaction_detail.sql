USE [CRS.UAT_V2]
GO
 
/****** Object:  Table [dbo].[tbl_reservation_transaction_detail]    Script Date: 5/28/2024 11:10:34 AM ******/
SET ANSI_NULLS ON
GO
 
SET QUOTED_IDENTIFIER ON
GO
 
CREATE TABLE [dbo].[tbl_reservation_transaction_detail](
	[Sno] [bigint] IDENTITY(1,2) NOT NULL,
	[ReservationTxnId] [bigint] NULL,
	[ReservationId] [bigint] NULL,
	[ReservationType] [varchar](10) NULL,
	[PlanAmount] [decimal](18, 2) NULL,
	[TotalPlanAmount] [decimal](18, 2) NULL,
	[TotalClubPlanAmount] [decimal](18, 2) NULL,
	[CommissionId] [VARCHAR](10) NULL,
	[AdminPlanCommissionAmount] DECIMAL(18,2) NULL,
	[TotalAdminPlanCommissionAmount] DECIMAL(18,2) NULL,
	[AdminCommissionAmount] [decimal](18, 2) NULL,
	[TotalAdminCommissionAmount] [decimal](18, 2) NULL,
	[TotalAdminPayableAmount] [decimal](18, 2) NULL,
	[Remarks] [nvarchar](512) NULL,
	[AdminPaymentStatus] [char](1) NULL,
	[ActionUser] [nvarchar](200) NULL,
	[ActionDate] [datetime] NULL,
	[ActionIP] [varchar](50) NULL,
	[ActionPlatform] [Nvarchar](20) NULL,
PRIMARY KEY CLUSTERED 
(
	[Sno] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
 
