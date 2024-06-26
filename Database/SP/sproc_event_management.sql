USE [CRS]
GO
/****** Object:  StoredProcedure [dbo].[sproc_event_management]    Script Date: 2/16/2024 3:25:15 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--[dbo].[sproc_event_management]'gel','3'
ALTER PROC [dbo].[sproc_event_management]
    @Flag VARCHAR(10) = '',
    @AgentId VARCHAR(20) = NULL,
    @ImageID VARCHAR(20) = NULL,
    @UserId VARCHAR(20) = NULL,
    @EventId VARCHAR(20) = NULL,
    @EventTitle NVARCHAR(200) = NULL,
    @Description NVARCHAR(500) = NULL,
    @Gallery VARCHAR(MAX) = NULL,
    @ActionUser VARCHAR(200) = NULL,
    @ActionDate VARCHAR(200) = NULL,
    @ActionIP VARCHAR(50) = NULL,
    @ActionPlatform VARCHAR(20) = NULL,
    @ImagePath VARCHAR(MAX) = NULL,
    @ImageTitle VARCHAR(150) = NULL,
    @Status CHAR(1) = NULL,
    @EventDate VARCHAR(10) = NULL,
    @SearchFilter NVARCHAR(100) = '',
    @Take INT = 10,
    @Skip INT = 0,
	@EventType NVARCHAR(500) = NULL
AS
DECLARE @Sno VARCHAR(10),
        @Sno2 VARCHAR(10),
        @Sno3 VARCHAR(10),
        @TransactionName VARCHAR(200),
        @ErrorDesc VARCHAR(MAX),
        @RandomPassword VARCHAR(20),
        @FetchQuery NVARCHAR(MAX),
        @SQLString NVARCHAR(MAX) = N'',
        @SQLFilterString NVARCHAR(MAX) = N'';
BEGIN TRY

    IF ISNULL(@Flag, '') = 'gel' --get event list
    BEGIN
        IF ISNULL(@AgentId, '') <> ''
        BEGIN
            BEGIN
                SET @SQLFilterString += N' AND em.AgentId=' + @AgentId;
            END;
        END;
        IF ISNULL(@SearchFilter, '') <> ''
        BEGIN
            SET @SQLFilterString += N' AND (em.EventTitle) LIKE N''%' + @SearchFilter + N'%''     ';
			 SET @SQLFilterString += N' OR (em.Description) LIKE N''%' + @SearchFilter + N'%''     ';
        END;
        SET @FetchQuery
            = N' OFFSET ' + CAST(ISNULL(@Skip, '0') AS VARCHAR) + N' ROWS FETCH NEXT '
              + CAST(ISNULL(@Take, '10') AS VARCHAR) + N' ROW ONLY';

        SET @SQLString
            = N' SELECT ROW_NUMBER() OVER (ORDER BY em.EventDate DESC) AS SNO,
									   c.LoginId,
									   em.Status,
									   em.Sno,
									   em.AgentId,
									   em.EventId,
									   em.EventTitle,
									   em.Description,
									   em.ActionUser,
									   em.ActionIP,
									   em.ActionPlatform,
									   FORMAT(em.ActionDate,N''yyyy年MM月dd日 HH:mm:ss'',''ja-JP'') AS ActionDate,
									   em.UpdatedDate,
									   CONCAT(FORMAT(CONVERT(DATETIME,  em.EventDate), N''yyyy年MM月''), RIGHT(''0'' + CAST(DAY(CONVERT(DATETIME,  em.EventDate)) AS VARCHAR(2)), 2) + N''日'') AS   EventDate,
									   em.EventType,
									   em.Image,
									   case when em.eventType=''2'' then ''Schedule''  ELSE ''Notice'' end EventTypeName,
									   COUNT(em.AgentId) OVER() AS TotalRecords
								FROM dbo.tbl_event_management em WITH (NOLOCK)
									INNER JOIN dbo.tbl_club_details b WITH (NOLOCK)
										ON b.AgentId = em.AgentId
										   AND ISNULL(b.Status, '''') = ''A''
									INNER JOIN dbo.tbl_users c WITH (NOLOCK)
										ON c.AgentId = em.AgentId
										   AND c.RoleType = 4
										   AND ISNULL(c.Status, '''') = ''A''
								WHERE ISNULL(em.Status, '''') = ''A''
								 ' + @SQLFilterString;
        SET @SQLString += N' ORDER BY em.EventDate DESC ' + @FetchQuery;
        EXEC (@SQLString);
        PRINT (@SQLString);
        RETURN;
    END;
    ELSE IF ISNULL(@Flag, '') = 'ged' --get event details
    BEGIN
        IF NOT EXISTS
        (
            SELECT 'X'
            FROM tbl_event_management a WITH (NOLOCK)
                INNER JOIN tbl_users b WITH (NOLOCK)
                    ON b.AgentId = a.AgentId
                       AND ISNULL(b.Status, '') NOT IN ( 'D', '' )
            WHERE a.AgentId = @AgentId
                  AND ISNULL(a.Status, '') NOT IN ( 'D', '' )
        )
        BEGIN
            SELECT 1 Code,
                   'Invalid details' Message;
            RETURN;
        END;

        SELECT 0 Code,
               'Success' Message,
               em.AgentId,
               b.UserId,
               b.LoginId,
               em.EventId,
               em.Status,
               em.EventTitle,
               em.Description,
               CONVERT(CHAR(10), em.EventDate, 126) AS [Date]
			   ,EM.EventType,EM.Image,
			    case when EM.eventType='2' then 'Schedule'  ELSE 'Notice' end EventTypeName
        FROM tbl_event_management em WITH (NOLOCK)
            INNER JOIN tbl_users b WITH (NOLOCK)
                ON b.AgentId = em.AgentId
                   AND ISNULL(b.Status, '') NOT IN ( 'D', '' )
        WHERE em.AgentId = @AgentId
              AND em.EventId = @EventId
			  AND b.RoleType=4;
        RETURN;
    END;
    ELSE IF ISNULL(@Flag, '') = 'rc' --register event
    BEGIN
        IF NOT EXISTS
        (
            SELECT 'X'
            FROM dbo.tbl_club_details a WITH (NOLOCK)
                INNER JOIN dbo.tbl_users b WITH (NOLOCK)
                    ON b.AgentId = a.AgentId
                       AND ISNULL(b.Status, '') IN ( 'A' )
                       AND b.RoleType = 4
            WHERE a.AgentId = @AgentId
                  AND ISNULL(a.Status, '') IN ( 'A' )
        )
        BEGIN
            SELECT 1 Code,
                   'Invalid request' Message;
            RETURN;
        END;

        INSERT INTO dbo.tbl_event_management
        (
            AgentId,
            EventTitle,
            [Description],
            [Status],
            ActionUser,
            ActionIP,
            ActionPlatform,
            ActionDate,
            EventDate,
			[EventType], 
			[Image]
        )
        VALUES
        (@AgentId, @EventTitle, @Description, 'A', @ActionUser, @ActionIP, @ActionPlatform, GETDATE(), @EventDate,@EventType,ISNULL( @ImagePath ,''));

        SET @Sno = SCOPE_IDENTITY();

        UPDATE dbo.tbl_event_management
        SET EventId = @Sno
        WHERE Sno = @Sno;

        SELECT 0 Code,
               'Event added successfully' Message;

        RETURN;
    END;
    ELSE IF ISNULL(@Flag, '') = 'me' --manage event / Edit
    BEGIN
        IF NOT EXISTS
        (
            SELECT 'X'
            FROM dbo.tbl_event_management a WITH (NOLOCK)
                INNER JOIN dbo.tbl_club_details b WITH (NOLOCK)
                    ON b.AgentId = a.AgentId
                       AND ISNULL(b.Status, '') = 'A'
                INNER JOIN dbo.tbl_users c WITH (NOLOCK)
                    ON c.AgentId = a.AgentId
                       AND c.RoleType = 4
                       AND ISNULL(c.Status, '') IN ( 'A' )
            WHERE a.AgentId = @AgentId
                  AND ISNULL(a.Status, '') IN ( 'A' )
        )
        BEGIN
            SELECT 1 Code,
                   'Invalid details' Message;
            RETURN;
        END;

        SET @TransactionName = 'Flag_mc';

        BEGIN TRANSACTION @TransactionName;


		IF 	@EventType='2'
		begin
        UPDATE dbo.tbl_event_management
        SET EventTitle = ISNULL(@EventTitle, EventTitle),
            EventDate = ISNULL(@EventDate, EventDate),
            [Description] = ISNULL(@Description, [Description]),
            ActionDate = ISNULL(@ActionDate, ActionDate),
            ActionUser = @ActionUser,
            ActionIP = ISNULL(@ActionIP, ActionIP),
            ActionPlatform = ISNULL(@ActionPlatform, ActionPlatform),
			EventType=ISNULL(@EventType, EventType),
			Image=ISNULL(@ImagePath, Image),
            UpdatedDate = GETDATE(),
            [Status] = ISNULL(@Status, [Status])
        WHERE AgentId = @AgentId
              AND EventId = @EventId
              AND ISNULL(Status, '') NOT IN ( 'D', '' );
END 
ELSE IF 	@EventType='1'
BEGIN
 UPDATE dbo.tbl_event_management
        SET EventTitle = @EventTitle,
            EventDate = ISNULL(@EventDate, EventDate),
            [Description] = ISNULL(@Description, [Description]),
            ActionDate = ISNULL(@ActionDate, ActionDate),
            ActionUser = @ActionUser,
            ActionIP = ISNULL(@ActionIP, ActionIP),
            ActionPlatform = ISNULL(@ActionPlatform, ActionPlatform),
			EventType=ISNULL(@EventType, EventType),
			Image=@ImagePath,
            UpdatedDate = GETDATE(),
            [Status] = ISNULL(@Status, [Status])
        WHERE AgentId = @AgentId
              AND EventId = @EventId
              AND ISNULL(Status, '') NOT IN ( 'D', '' );
end
        SELECT 0 Code,
               'Event updated successfully' Message;

        COMMIT TRANSACTION @TransactionName;
        RETURN;
    END;
    ELSE IF ISNULL(@Flag, '') = 'del' --delete event list (DISABLED)
    BEGIN
        IF NOT EXISTS
        (
            SELECT 'X'
            FROM dbo.tbl_event_management a WITH (NOLOCK)
                INNER JOIN dbo.tbl_club_details b WITH (NOLOCK)
                    ON b.AgentId = a.AgentId
                       AND ISNULL(b.Status, '') = 'A'
                INNER JOIN dbo.tbl_users c WITH (NOLOCK)
                    ON c.AgentId = a.AgentId
                       AND c.RoleType = 4
                       AND ISNULL(c.Status, '') IN ( 'A' )
            WHERE a.AgentId = @AgentId
                  AND ISNULL(a.Status, '') IN ( 'A' )
        )
        BEGIN
            SELECT 1 Code,
                   'Invalid request' Message;
            RETURN;
        END;

        UPDATE dbo.tbl_event_management
        SET [Status] = 'D',
            UpdatedDate = GETDATE()
        WHERE EventId = @EventId
              AND AgentId = @AgentId;

        SELECT 0 Code,
               'Event deleted successfully' Message;
        RETURN;
    END;
	--ELSE IF ISNULL(@Flag,'')='I' --insert event	
	--BEGIN
	--PRINT 1
	--IF ISNULL(@AgentId, '') = ''
 --       BEGIN
 --           SELECT 1 Code,
 --                  'Required validation failed' Message;
 --           RETURN;
 --       END;

	
	--INSERT INTO tbl_event_management ( [AgentId],[EventTitle], [Description], [Status], [ActionUser], [ActionIP], [ActionPlatform], [ActionDate], [UpdatedDate], [EventDate], [EventType], [Image])
 --   VALUES
 --   (  @AgentId,ISNULL( @EventTitle,''), @Description, @Status, @ActionUser,@ActionIP, @ActionPlatform, GETDATE(), NULL, @EventDate, @EventType,ISNULL( @ImagePath ,''))
	
	-- SET @Sno = SCOPE_IDENTITY();

 --       UPDATE dbo.tbl_event_management
 --       SET EventId = @Sno
 --       WHERE Sno = @Sno;

	--	  SELECT 0 Code,
 --              'Event created successfully' Message;

 --       RETURN;
	--END
 --   ELSE
 --   BEGIN
 --       SELECT 1 Code,
 --              'Invalid function' Message;
 --       RETURN;
 --   END;

	

END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0
        ROLLBACK TRANSACTION @TransactionName;

    SET @ErrorDesc = 'SQL error found: (' + ERROR_MESSAGE() + ')' + ' at ' + CAST(ERROR_LINE() AS VARCHAR);

    INSERT INTO tbl_error_log
    (
        ErrorDesc,
        ErrorScript,
        QueryString,
        ErrorCategory,
        ErrorSource,
        ActionDate
    )
    VALUES
    (@ErrorDesc, 'sproc_event_management(Flag: ' + ISNULL(@Flag, '') + ')', 'SQL', 'SQL', 'sproc_event_management',
     GETDATE());

    SELECT 1 Code,
           'ErrorId:' + CAST(SCOPE_IDENTITY() AS VARCHAR) Message;
    RETURN;
END CATCH;
