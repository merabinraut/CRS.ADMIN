USE [CRS]
GO

CREATE TABLE [dbo].[tbl_static_data_type]
(
	Id BIGINT NOT NULL IDENTITY(1,2) PRIMARY KEY,
	StaticDataType BIGINT NULL,
	StaticDataLabel VARCHAR(50) NULL,
	StaticDataName VARCHAR(200) NULL,
	StaticDataDescription VARCHAR(256) NULL,
	ActionUser VARCHAR(200) NULL,
	ActionDate DATETIME NULL
)