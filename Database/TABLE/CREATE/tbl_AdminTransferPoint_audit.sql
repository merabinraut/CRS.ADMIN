CREATE TABLE [dbo].[tbl_AdminTransferPoint_audit](
	AuditId BIGINT IDENTITY(1,2) PRIMARY KEY,
	[Id] [VARCHAR](10) NULL,
	[TransactionId] [nvarchar](100) NULL,
	[AgentId] [bigint] NULL,
	[Amount] [decimal](16, 2) NULL,
	[Remark] [nvarchar](1000) NULL,
	[TrancationType] [int] NULL,
	[Image] [varchar](max) NULL,
	[Status] [char](1) NULL,
	[ActionDate] [datetime] NULL,
	[ActionUser] [nvarchar](200) NULL,
	TriggerLogUser NVARCHAR(200) NULL,
	TriggerAction NVARCHAR(100) NULL,
	TriggerActionLocalDate DATETIME NULL,
	TriggerActionUTCDate DATETIME NULL
)
