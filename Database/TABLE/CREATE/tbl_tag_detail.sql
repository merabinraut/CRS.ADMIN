use CRS

CREATE TABLE tbl_tag_detail
(
	Sno BIGINT PRIMARY KEY IDENTITY(1,2),
	DetailId BIGINT NULL,
	AgentId BIGINT NULL,
	Tag1Location VARCHAR(512) NULL,
	Tag1Status CHAR(1) NULL,
	Tag2RankName VARCHAR(100) NULL,
	Tag2RankDescription VARCHAR(512) NULL,
	Tag2Status CHAR(1) NULL,
	Tag3CategoryName VARCHAR(100) NULL,
	Tag3CategoryDescription VARCHAR(512) NULL,
	Tag3Status CHAR(1) NULL,
	Tag4ExcellencyName VARCHAR(100) NULL,
	Tag4ExcellencyDescription VARCHAR(512) NULL,
	Tag4Status CHAR(1) NULL,
	Tag5StoreName VARCHAR(100) NULL,
	Tag5SoteDescription VARCHAR(512) NULL,
	Tag5Status CHAR(1) NULL,
	ActionUser VARCHAR(200) NULL,
	ActionIP VARCHAR(50) NULL,
	ActionDate DATETIME NULL
)