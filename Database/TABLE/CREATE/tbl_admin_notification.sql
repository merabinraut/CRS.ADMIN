CREATE TABLE tbl_admin_notification
(
	Sno BIGINT IDENTITY(1,2) PRIMARY KEY,
	NotificationTo BIGINT NULL,
	NotificationType VARCHAR(200) NULL,
	NotificationSubject VARCHAR(512) NULL,
	NotificationBody VARCHAR(512) NULL,
	NotificationImageURL VARCHAR(512) NULL,
	NotificationStatus CHAR(1) NULL,
	NotificationReadStatus CHAR(1) NULL,
	CreatedBy VARCHAR(200) NULL,
	CreatedDate DATETIME NULL,
	UpdatedBy VARCHAR(200) NULL,
	UpdatedDate DATETIME NULL
)