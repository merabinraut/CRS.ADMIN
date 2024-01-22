CREATE TABLE dbo.tbl_recommendation_detail
(
	Sno BIGINT PRIMARY KEY IDENTITY(1,2),
	RecommendationId BIGINT NULL,
	LocationId BIGINT NULL,
	ClubId BIGINT NULL,
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
