CREATE TABLE [dbo].[tbl_plans](
    [Sno] [bigint] IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [PlanId] [bigint] NOT NULL,
    [PlanName] [varchar](200) NULL,
    [PlanType] [varchar](10) NULL,
    [PlanTime] [varchar](10) NULL,
    [Price] [decimal](18, 2) NULL,
    [Liquor] [varchar](10) NULL,
    [Nomination] [varchar](10) NULL,
    [Remarks] [varchar](500) NULL,
    [PlanStatus] [char](1) NULL,
    [ActionUser] [varchar](200) NULL,
    [ActionIp] [varchar](50) NULL,
    [ActionPlatform] [varchar](20) NULL,
    [ActionDate] [datetime] NULL
);
