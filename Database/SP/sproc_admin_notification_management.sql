USE [CRS]
GO

/****** Object:  StoredProcedure [dbo].[sproc_admin_notification_management]    Script Date: 20/11/2023 23:07:45 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



ALTER PROC [dbo].[sproc_admin_notification_management]
    @Flag VARCHAR(10),
    @NotificationTo VARCHAR(10) = NULL,
    @NotificationType VARCHAR(200) = NULL,
    @NotificationSubject VARCHAR(200) = NULL,
    @NotificationBody VARCHAR(MAX) = NULL,
    @NotificationImageURL VARCHAR(512) = NULL,
    @NotificationStatus CHAR(1) = NULL,
    @ActionUser VARCHAR(200) = NULL,
    @AdminId VARCHAR(10) = NULL,
    @NotificationId VARCHAR(10) = NULL
AS
BEGIN
    IF ISNULL(@Flag, '') = 'i'
    BEGIN
        IF NOT EXISTS
        (
            SELECT 'X'
            FROM dbo.tbl_admin a WITH (NOLOCK)
            WHERE a.Id = @AdminId
                  AND ISNULL(a.[Status], '') NOT IN ( 'D', 'S' )
        )
        BEGIN
            SELECT 1 Code,
                   'Invalid admin details' Message;
            RETURN;
        END;

        INSERT INTO dbo.tbl_admin_notification
        (
            NotificationTo,
            NotificationType,
            NotificationSubject,
            NotificationBody,
            NotificationImageURL,
            NotificationStatus,
            NotificationReadStatus,
            CreatedBy,
            CreatedDate
        )
        VALUES
        (   @NotificationTo,       -- NotificationTo - bigint
            @NotificationType,     -- NotificationType - varchar(200)
            @NotificationSubject,  -- NotificationSubject - varchar(512)
            @NotificationBody,     -- NotificationBody - varchar(512)
            @NotificationImageURL, -- NotificationImageURL - varchar(512)
            @NotificationStatus,   -- NotificationStatus - char(1)
            'N',                   -- NotificationReadStatus - char(1)
            @ActionUser,           -- CreatedBy - varchar(200)
            GETDATE()              -- CreatedDate - datetime
            );

        SELECT 0 Code,
               'Notification pushed successfully' Message;
        RETURN;
    END;
    ELSE IF ISNULL(@Flag, '') = 's'
    BEGIN
        WITH CTE
        AS (SELECT b.Sno,
                   b.NotificationTo,
                   b.NotificationType,
                   b.NotificationSubject,
                   b.NotificationBody,
                   b.NotificationImageURL,
                   b.NotificationReadStatus,
                   b.CreatedDate,
				   b.CreatedDate AS Time
            FROM dbo.tbl_admin a WITH (NOLOCK)
                INNER JOIN dbo.tbl_admin_notification b WITH (NOLOCK)
                    ON b.NotificationTo = a.Id
                       AND ISNULL(a.Status, '') = 'A'
                       AND ISNULL(b.NotificationStatus, '') = 'A'
            WHERE a.Id = @AdminId)
        SELECT TOP 5
               *,
               (
                   SELECT COUNT(ct.Sno)
                   FROM CTE ct WITH (NOLOCK)
                   WHERE ISNULL(ct.NotificationReadStatus, '') = 'N'
               ) AS NotificationCount
        FROM CTE a WITH (NOLOCK)
        ORDER BY a.CreatedDate ASC;
        RETURN;
    END;
    ELSE IF ISNULL(@Flag, '') = 'gan' --get all notification
    BEGIN
        IF EXISTS
        (
            SELECT 'X'
            FROM dbo.tbl_admin a WITH (NOLOCK)
            WHERE a.Id = @AdminId
                  AND ISNULL(a.[Status], '') = 'Y'
        )
        BEGIN
            UPDATE dbo.tbl_admin_notification
            SET NotificationReadStatus = 'Y',
                UpdatedBy = @ActionUser,
                UpdatedDate = GETDATE()
            WHERE Sno = ISNULL(@NotificationId, Sno)
                  AND ISNULL(NotificationReadStatus, '') = 'N'
                  AND ISNULL(NotificationStatus, '') = 'A';


            SELECT b.Sno,
                   b.NotificationTo,
                   b.NotificationType,
                   b.NotificationSubject,
                   b.NotificationBody,
                   b.NotificationImageURL,
                   b.NotificationStatus,
                   b.NotificationReadStatus,
                   b.CreatedBy,
                   b.CreatedDate,
                   b.UpdatedBy,
                   b.UpdatedDate,
				   b.CreatedDate AS Time
            FROM dbo.tbl_admin a WITH (NOLOCK)
                INNER JOIN dbo.tbl_admin_notification b WITH (NOLOCK)
                    ON b.NotificationTo = a.Id
                       AND ISNULL(a.[Status], '') = 'A'
                       AND ISNULL(b.NotificationStatus, '') = 'A'
            WHERE a.Id = @AdminId
                  AND b.Sno = ISNULL(@NotificationId, b.Sno);
        END;
        RETURN;
    END;
END;
GO


