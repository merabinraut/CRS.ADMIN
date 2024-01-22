CREATE TABLE tbl_commission_category
(
	Sno BIGINT IDENTITY(1,2) PRIMARY KEY,
	CategoryId BIGINT NULL,
	CategoryName VARCHAR(200) NULL,
	[Description] VARCHAR(512) NULL,
	[Status] CHAR(1) NULL,
	ActionUser VARCHAR(200) NULL,
	ActionIP VARCHAR(50) NULL,
	ActionDate DATETIME NULL
)