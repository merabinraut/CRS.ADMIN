CREATE TABLE dbo.tbl_host_identity_details
(
	Sno BIGINT IDENTITY(1,2) NOT NULL PRIMARY KEY,
	ClubId BIGINT NULL,
	HostId BIGINT NULL,
	IdentityType VARCHAR(10) NULL,
	IdentityValue VARCHAR(10) NULL,
	IdentityDDLType VARCHAR(10) NULL,
	IdentityDescription NVARCHAR(MAX) NULL,
	CreatedBy NVARCHAR(200) NULL,
	CreatedDate DATETIME NULL,
	CreatedIP VARCHAR(50) NULL,
	CreatedPlatform VARCHAR(20) NULL,
	UpdatedBy NVARCHAR(200) NULL,
	UpdatedDate DATETIME NULL,
	UpdatedIP VARCHAR(50) NULL,
	UpdatedPlatform VARCHAR(20) NULL
)
