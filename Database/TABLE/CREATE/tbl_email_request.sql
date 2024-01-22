CREATE TABLE dbo.tbl_email_request
(
	Sno BIGINT IDENTITY(1,2) PRIMARY KEY,
	EmailSubject NVARCHAR(600) NULL,
	EmailText NVARCHAR(MAX) NULL,
	EmailFileAttached VARCHAR(200) NULL,
	EmailNotesAttached IMAGE NULL,
	EmailSendBy VARCHAR(256) NULL,
	EmailSendTo VARCHAR(5000) NULL,
	EmailSendToCC VARCHAR(5000) NULL,
	EmailSendToBCC VARCHAR(5000) NULL,
	EmailSendStatus CHAR(1) NULL,
	IsImportant CHAR(1) NULL,
	Status CHAR(1) NULL,
	CreatedBy NVARCHAR(200) NULL,
	CreatedDate DATETIME NULL,
	CreatedIP VARCHAR(50) NULL,
	CretaedPlatform VARCHAR(20) NULL,
	UpdatedBy NVARCHAR(200) NULL,
	UpdatedDate DATETIME NULL,
	UpdatedIP VARCHAR(50) NULL,
	UpdatedPlatform VARCHAR(20) NULL
)