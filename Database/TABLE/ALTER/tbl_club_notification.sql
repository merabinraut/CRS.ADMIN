use crs_v2
ALTER TABLE tbl_club_notification
ALTER COLUMN NotificationSubject NVARCHAR(512)
GO
ALTER TABLE tbl_club_notification
ALTER COLUMN NotificationBody NVARCHAR(512)
GO
ALTER TABLE tbl_club_notification
ALTER COLUMN CreatedBy NVARCHAR(200)
GO
ALTER TABLE tbl_club_notification
ALTER COLUMN UpdatedBy NVARCHAR(200)
GO
