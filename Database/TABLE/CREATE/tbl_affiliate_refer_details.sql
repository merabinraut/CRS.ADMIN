CREATE TABLE dbo.tbl_affiliate_refer_details
(
	Sno BIGINT IDENTITY(1,2) PRIMARY KEY,
	ReferId BIGINT NULL,
	AffiliateId BIGINT NULL,
	SnsId VARCHAR(10) NULL,
	TotalClickCount INT NULL,
	STATUS CHAR(1) NULL,
	CreatedBy VARCHAR(200) NULL,
	CreatedDate DATETIME NULL,
	CreatedIP VARCHAR(50) NULL,
	CreatedPlatform VARCHAR(20) NULL,
	UpdatedBy VARCHAR(200) NULL,
	UpdatedDate DATETIME NULL,
	UpdatedIP VARCHAR(50) NULL,
	UpdatedPlatform VARCHAR(20) NULL
)

