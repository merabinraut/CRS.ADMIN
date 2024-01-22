

CREATE TABLE tbl_display_mainpage
(
	Sno BIGINT PRIMARY KEY IDENTITY(1,2),
	RecommendationId BIGINT NULL,
	GroupId BIGINT NULL,
	OrderId BIGINT NULL,
	DisplayPageId VARCHAR(10) NULL,
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
