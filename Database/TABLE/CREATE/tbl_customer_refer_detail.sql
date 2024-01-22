CREATE TABLE dbo.tbl_customer_refer_detail
(
	Sno BIGINT IDENTITY(1,2) PRIMARY KEY,
	ReferDetailId BIGINT NULL,
	ReferId BIGINT NULL,
	AffiliateId BIGINT NULL,
	CustomerId BIGINT NULL,
	Status CHAR(1) NULL,
	CreatedBy VARCHAR(200) NULL,
	CreatedDate DATETIME NULL,
	CreatedIP VARCHAR(50) NULL,
	CreatedPlatform VARCHAR(20) NULL,
	UpdatedBy VARCHAR(200) NULL,
	UpdatedDate DATETIME NULL,
	UpdatedIP VARCHAR(50) NULL,
	UpdatedPlatform VARCHAR(20) NULL
)

CREATE TABLE dbo.tbl_affiliate_referral_detail
(
	Sno BIGINT IDENTITY(1,2) PRIMARY KEY,
	ReferId BIGINT NULL,
	ReferDetailId BIGINT NULL,
	AffiliateId BIGINT NULL,
	CustomerId BIGINT NULL,
	ReservationId BIGINT NULL,
	PaymentId BIGINT NULL,
	CommissionAmount DECIMAL(18,2) NULL,
	Status CHAR(1) NULL,
	Remarks NVARCHAR(1000) NULL,
	CreatedBy VARCHAR(200) NULL,
	CreatedDate DATETIME NULL,
	CreatedIP VARCHAR(50) NULL,
	CreatedPlatform VARCHAR(20) NULL,
	UpdatedBy VARCHAR(200) NULL,
	UpdatedDate DATETIME NULL,
	UpdatedIP VARCHAR(50) NULL,
	UpdatedPlatform VARCHAR(20) NULL
)