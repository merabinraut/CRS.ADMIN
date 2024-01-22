USE [CRS]
GO

CREATE TABLE [dbo].[tbl_error_log]
(
	Id BIGINT IDENTITY(1,2) NOT NULL
   ,ErrorDesc VARCHAR(MAX) NULL
   ,ErrorScript VARCHAR(512) NULL
   ,QueryString VARCHAR(512) NULL
   ,ErrorCategory VARCHAR(512) NULL
   ,ErrorSource VARCHAR(512) NULL
   ,ActionDate DATETIME NULL
)
