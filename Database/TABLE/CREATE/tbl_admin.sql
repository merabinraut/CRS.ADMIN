CREATE TABLE tbl_admin
(
	Id BIGINT PRIMARY KEY IDENTITY(1,2) NOT NULL,
	RoleId BIGINT NULL,
	Username VARCHAR(200) NULL,
	FullName VARCHAR(512) NULL,
	EmailAddress VARCHAR(512) NULL,
	MobileNumber VARCHAR(15) NULL,
	Password VARBINARY(MAX) NULL,
	Status CHAR(1) NULL,
	ProfileImage VARCHAR(MAX) NULL,
	Session VARCHAR(512) NULL,
	AllowMultipleLogin CHAR(1) NULL,
	FailedLoginAttempt INT NULL,
	IsPasswordForceful CHAR(1) NULL,
	LastPasswordChangedDate DATETIME NULL,
	LastLoginDate DATETIME NULL,
	ActionUser VARCHAR(200) NULL,
	ActionDate DATETIME NULL,
	ActionIP VARCHAR(50) NULL
)