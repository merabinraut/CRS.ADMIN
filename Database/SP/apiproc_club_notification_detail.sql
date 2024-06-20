USE [CRS.UAT_V2]
GO

/****** Object:  StoredProcedure [dbo].[apiproc_club_notification_detail]    Script Date: 6/7/2024 12:47:43 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

ALTER PROC [dbo].[apiproc_club_notification_detail] @Flag VARCHAR(10)
	,@ClubId VARCHAR(10) = NULL
	,@ActionUser NVARCHAR(100) = NULL
	,@NotificationId VARCHAR(10) = NULL
	,@Skip INT = 0
	,@Take INT = 10
AS
BEGIN
	IF @Flag = 1 --get notification list
	BEGIN
		SELECT *
		FROM (
			SELECT a.notificationId AS NotificationId
				,FORMAT(CAST(a.createdDate AS DATETIME), 'yyyy.M.d HH:mm') AS NotificationDate
				,ISNULL(a.NotificationReadStatus, 'P') NotificationReadStatus
				,a.NotificationBody AS NotificationContent
				,CASE 
					WHEN ISNULL(a.NotificationType, '8') IN ('8')
						THEN ''
					ELSE a.NotificationType
					END AS NotificationType
				,COUNT(a.notificationId) OVER () AS RowsTotal
			FROM dbo.tbl_club_notification a WITH (NOLOCK)
			INNER JOIN dbo.tbl_club_details b WITH (NOLOCK) ON b.AgentId = a.ToAgentId
			WHERE ISNULL(a.NotificationStatus, '') = 'A'
				AND b.AgentId = @ClubId
			GROUP BY a.notificationId
				,a.createdDate
				,a.NotificationReadStatus
				,a.NotificationBody
				,a.NotificationType
			
			UNION ALL
			
			SELECT a.notificationId AS NotificationId
				,FORMAT(CAST(a.createdDate AS DATETIME), 'yyyy.M.d HH:mm') AS NotificationDate
				,ISNULL(a.NotificationReadStatus, 'P') NotificationReadStatus
				,a.NotificationBody AS NotificationContent
				,CASE 
					WHEN ISNULL(a.NotificationType, '8') IN ('8')
						THEN ''
					ELSE a.NotificationType
					END AS NotificationType
				,COUNT(a.notificationId) OVER () AS RowsTotal
			FROM dbo.tbl_club_notification a WITH (NOLOCK)
			WHERE ISNULL(a.NotificationStatus, '') = 'A'
				AND a.ToAgentId = 0
				AND FORMAT(a.CreatedDate, 'yyyy-MM-dd') >= (
					SELECT TOP 1 ISNULL(FORMAT(MIN(b2.ActionDate), 'yyyy-MM-dd'), FORMAT(a2.ActionDate, 'yyyy-MM-dd'))
					FROM dbo.tbl_club_details a2 WITH (NOLOCK)
					LEFT JOIN dbo.tbl_club_details_audit b2 WITH (NOLOCK) ON b2.AgentId = a2.AgentId
						AND ISNULL(b2.STATUS, '') = 'A'
					WHERE a2.AgentId = @ClubId
					GROUP BY b2.ActionDate
						,a2.ActionDate
					)
			GROUP BY a.notificationId
				,a.createdDate
				,a.NotificationReadStatus
				,a.NotificationBody
				,a.NotificationType
			) t
		ORDER BY t.NotificationDate DESC OFFSET @Skip ROWS

		FETCH NEXT @Take ROWS ONLY;

		RETURN;
			--SELECT a.notificationId AS NotificationId,
			--       FORMAT(CAST(a.createdDate AS DATETIME), 'yyyy.M.d HH:mm') AS NotificationDate,
			--       ISNULL(a.NotificationReadStatus, 'P') NotificationReadStatus,
			--       a.NotificationBody AS NotificationContent,
			--       CASE
			--           WHEN ISNULL(a.NotificationType, '8') IN ( '8' ) THEN
			--               ''
			--           ELSE
			--               a.NotificationType
			--       END AS NotificationType,
			--       COUNT(a.notificationId) OVER() AS RowsTotal
			--FROM dbo.tbl_club_notification a WITH (NOLOCK)
			--    INNER JOIN dbo.tbl_club_details b WITH (NOLOCK)
			--        ON b.AgentId = a.ToAgentId
			--WHERE ISNULL(a.NotificationStatus, '') = 'A'
			--      AND b.AgentId = @ClubId
			--GROUP BY a.notificationId,
			--         a.createdDate,
			--         a.NotificationReadStatus,
			--         a.NotificationBody,
			--         a.NotificationType
			--ORDER BY a.createdDate DESC OFFSET @Skip ROWS FETCH NEXT @Take ROWS ONLY;
			--RETURN;
	END;
	ELSE IF @Flag = 2 --get unread notification count
	BEGIN
		IF NOT EXISTS (
				SELECT 1
				FROM dbo.tbl_club_notification a WITH (NOLOCK)
				INNER JOIN dbo.tbl_club_details b WITH (NOLOCK) ON b.AgentId = a.ToAgentId
				WHERE a.notificationId = @NotificationId
					AND b.AgentId = @ClubId
					AND ISNULL(a.NotificationStatus, '') = 'A'
					AND ISNULL(a.NotificationReadStatus, 'P') = 'P'
				)
		BEGIN
			SELECT 1 Code
				,'Invalid_Request' Message;

			RETURN;
		END;

		UPDATE dbo.tbl_club_notification
		SET NotificationReadStatus = 'A'
			,UpdatedBy = @ActionUser
			,UpdatedDate = GETDATE()
		WHERE notificationId = @NotificationId
			AND ToAgentId = @ClubId
			AND NotificationStatus = 'A'
			AND NotificationReadStatus = 'P';

		SELECT 0 Code
			,'Notification_Updated_Successfully' Message;

		RETURN;
	END;
END;
GO


