use [CRS_V2]
go

/****** Object:  Table [dbo].[tbl_Agent_balance_audit]    Script Date: 6/3/2024 3:32:15 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[tbl_Agent_balance_audit](
	[AuditId] [bigint] IDENTITY(1,2) NOT NULL,
	[Id] VARCHAR(10) NULL,
	[AgentId] [bigint] NULL,
	[Amount] [decimal](16, 2) NULL,
	[creditLimit] [decimal](16, 2) NULL,
	[CreditPointUse] [decimal](16, 2) NULL,
	[Status] [char](1) NULL,
	[RoleType] [bigint] NULL,
	[ActionUser] [bigint] NULL,
	[ActionDate] [datetime] NULL
	 ,TriggerLogUser NVARCHAR(200) NULL
		   ,TriggerAction NVARCHAR(100) NULL
		   ,TriggerActionLocalDate DATETIME NULL
		   ,TriggerActionUTCDate DATETIME NULL
)

