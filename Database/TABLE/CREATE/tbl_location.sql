USE [CRS]
GO

/****** Object:  Table [dbo].[tbl_location]    Script Date: 10/18/2023 10:39:03 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[tbl_location]') AND type in (N'U'))
DROP TABLE [dbo].[tbl_location]
GO

/****** Object:  Table [dbo].[tbl_location]    Script Date: 10/18/2023 10:39:03 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[tbl_location](
	[Sno] [bigint] IDENTITY(1,2) primary key NOT NULL,
	[LocationId] [bigint] NULL,
	[LocationName] [varchar](200) NULL,
	[LocationImage] [varchar](200) NULL,
	[LocationURL] [varchar](500) NULL,
	[Status] [char](1) NULL,
	[Latitude] [varchar](200),
	[Longitude] [varchar](200),
	[ActionUser] [varchar](100) NULL,
	[ActionDate] [varchar](100) NULL,
	[ActionIp] [varchar](100) NULL
	)