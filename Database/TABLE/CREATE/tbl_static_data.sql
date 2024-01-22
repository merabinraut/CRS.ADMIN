USE [CRS]
GO

CREATE TABLE [dbo].[tbl_static_data]
(
	Id BIGINT NOT NULL IDENTITY(1,2) PRIMARY KEY,
	StaticDataType BIGINT NULL,
	StaticDataLabel VARCHAR(50) NULL,
	StaticDataValue VARCHAR(200) NULL,
	StaticDataDescription VARCHAR(256) NULL,
	AdditionalValue1 VARCHAR(MAX) NULL,
	AdditionalValue2 VARCHAR(MAX) NULL,
	AdditionalValue3 VARCHAR(MAX) NULL,
	AdditionalValue4 VARCHAR(MAX) NULL,
	Status CHAR(1) NULL,
	ActionUser VARCHAR(200) NULL,
	ActionDate DATETIME NULL
)