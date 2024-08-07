USE [CRS_V2]
GO
/****** Object:  StoredProcedure [dbo].[sproc_host_management]    Script Date: 5/13/2024 12:02:22 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO




ALTER PROC [dbo].[sproc_host_management]
    @Flag VARCHAR(10),
    @AgentId VARCHAR(20) = NULL,
    @HostId VARCHAR(20) = NULL,
    @HostName VARCHAR(256) = NULL,
    @HostNameJapanese NVARCHAR(200) = NULL,
    @Position NVARCHAR(200) = NULL,
    @Rank INT = 0,
    @DOB VARCHAR(15) = NULL,
    @ConstellationGroup VARCHAR(10) = NULL,
    @Height VARCHAR(5) = NULL,
    @BloodType VARCHAR(10) = NULL,
    @PreviousOccupation VARCHAR(10) = NULL,
    @LiquorStrength VARCHAR(10) = NULL,
    @ActionUser VARCHAR(200) = NULL,
    @ActionIP VARCHAR(50) = NULL,
    @ActionPlatform VARCHAR(20) = NULL,
    @Status CHAR(1) = NULL,
    @ImagePath VARCHAR(MAX) = NULL,
	@IconImagePath VARCHAR(MAX) = NULL,
    @TiktokLink VARCHAR(MAX) = NULL,
    @TwitterLink VARCHAR(MAX) = NULL,
    @InstagramLink VARCHAR(MAX) = NULL,
    @Line VARCHAR(MAX) = NULL,
    @Address NVARCHAR(200) = NULL,
    @Skip INT = 0,
    @Take INT = 10,
    @SearchFilter NVARCHAR(200) = NULL,
    @HostIntroduction NVARCHAR(512) = NULL
AS
DECLARE @Sno VARCHAR(10),
        @Sno2 VARCHAR(10),
        @TransactionName VARCHAR(200),
        @ErrorDesc VARCHAR(MAX),
        @RandomPassword VARCHAR(20);
DECLARE @SQLString NVARCHAR(MAX) = N'',
        @SQLFilterParameter NVARCHAR(MAX) = N'';
BEGIN TRY
    IF ISNULL(@Flag, '') = 'rh' --register host
    BEGIN
        IF NOT EXISTS
        (
            SELECT 'X'
            FROM dbo.tbl_club_details a WITH (NOLOCK)
            WHERE a.AgentId = @AgentId
                  AND ISNULL(a.Status, '') = 'A'
        )
        BEGIN
            SELECT 1 Code,
                   'Invalid club details' Message;
            RETURN;
        END;

        IF @Rank != 0
        BEGIN
            IF @Rank <= 0
            BEGIN
                SELECT 1 Code,
                       'Invalid rank' Message;
                RETURN;
            END;

            IF EXISTS
            (
                SELECT 'X'
                FROM dbo.tbl_host_details a WITH (NOLOCK)
                WHERE a.AgentId = @AgentId
                      AND a.Rank = @Rank
                      AND ISNULL(a.Status, '') IN ( 'A' ) --, 'B' )
            )
            BEGIN
                SELECT 1 Code,
                       'Invalid rank' Message;
                RETURN;
            END;
        END;

        SET @TransactionName = 'Flag_rh';
        BEGIN TRANSACTION @TransactionName;

        INSERT INTO dbo.tbl_host_details
        (
            AgentId,
            HostName,
            Position,
            [Rank],
            DOB,
            ConstellationGroup,
            Height,
            BloodType,
            PreviousOccupation,
            LiquorStrength,
            Status,
            ActionUser,
            ActionIP,
            ActionPlatform,
            ActionDate,
            ImagePath,
			Icon,
            Address,
            HostNameJapanese,
            HostIntroduction
        )
        VALUES
        (@AgentId, @HostName, @Position, @Rank, @DOB, @ConstellationGroup, @Height, @BloodType, @PreviousOccupation,
         @LiquorStrength, 'A', @ActionUser, @ActionIP, @ActionPlatform, GETDATE(), @ImagePath,@IconImagePath, @Address,
         @HostNameJapanese, @HostIntroduction);

        SELECT @Sno = SCOPE_IDENTITY();

        UPDATE dbo.tbl_host_details
        SET HostId = @Sno
        WHERE Sno = @Sno;

        INSERT INTO dbo.tbl_website_details
        (
            AgentId,
            HostId,
            TiktokLink,
            TwitterLink,
            InstagramLink,
            ActionUser,
            ActionIP,
            ActionPlatform,
            ActionDate,
            RoleId,
            Line
        )
        VALUES
        (@AgentId, @Sno, @TiktokLink, @TwitterLink, @InstagramLink, @ActionUser, @ActionIP, @ActionPlatform, GETDATE(),
         5  , @Line);

        SELECT 0 Code,
               'Host registred successfully' Message,
			   @Sno AS Extra1;

        COMMIT TRANSACTION @TransactionName;
        RETURN;
    END;
    ELSE IF ISNULL(@Flag, '') = 'uh' --update host
    BEGIN
        IF NOT EXISTS
        (
            SELECT 'X'
            FROM dbo.tbl_host_details a WITH (NOLOCK)
            WHERE a.HostId = @HostId
                  AND a.AgentId = @AgentId
                  AND ISNULL(a.Status, '') = 'A'
        )
        BEGIN
            SELECT 1 Code,
                   'Invalid host details' Message;
            RETURN;
        END;

        IF ISNULL(@Rank, 0) <> 0
        BEGIN
            IF @Rank <= 0
            BEGIN
                SELECT 1 Code,
                       'Invalid rank' Message;
                RETURN;
            END;

            IF EXISTS
            (
                SELECT 'X'
                FROM dbo.tbl_host_details a WITH (NOLOCK)
                WHERE a.AgentId = @AgentId
                      AND a.HostId <> @HostId
                      AND a.Rank = @Rank
                      AND ISNULL(a.Status, '') IN ( 'A' ) --, 'B' )
            )
            BEGIN
                SELECT 1 Code,
                       'Invalid rank' Message;
                RETURN;
            END;
        END;

        SET @TransactionName = 'Flag_uh';

        BEGIN TRANSACTION @TransactionName;

        UPDATE dbo.tbl_host_details
        SET HostName = ISNULL(@HostName, HostName),
            HostNameJapanese = ISNULL(@HostNameJapanese, HostNameJapanese),
            Position = ISNULL(@Position, Position),
            [Rank] = ISNULL(@Rank, [Rank]),
            DOB = ISNULL(@DOB, DOB),
            ConstellationGroup = ISNULL(@ConstellationGroup, ConstellationGroup),
            Height = ISNULL(@Height, Height),
            BloodType = ISNULL(@BloodType, BloodType),
            PreviousOccupation = ISNULL(@PreviousOccupation, PreviousOccupation),
            LiquorStrength = ISNULL(@LiquorStrength, LiquorStrength),
            ImagePath = ISNULL(@ImagePath, ImagePath),
			Icon = ISNULL(@IconImagePath, Icon),
            ActionUser = @ActionUser,
            ActionIP = @ActionIP,
            ActionPlatform = @ActionPlatform,
            ActionDate = GETDATE(),
            Address = ISNULL(@Address, Address),
            HostIntroduction = ISNULL(@HostIntroduction, HostIntroduction)
        WHERE AgentId = @AgentId
              AND HostId = @HostId;


        UPDATE dbo.tbl_website_details
        SET TiktokLink = ISNULL(@TiktokLink, TiktokLink),
            TwitterLink = ISNULL(@TwitterLink, TwitterLink),
            InstagramLink = ISNULL(@InstagramLink, InstagramLink),
            Line = ISNULL(@Line, Line),
            ActionUser = @ActionUser,
            ActionIP = @ActionIP,
            ActionPlatform = @ActionPlatform,
            ActionDate = GETDATE()
        WHERE AgentId = @AgentId
              AND HostId = @HostId
              AND RoleId = 5;

        SELECT 0 Code,
               'Host updated successfully' Message;

        COMMIT TRANSACTION @TransactionName;
        RETURN;
    END;
    ELSE IF ISNULL(@Flag, '') = 'ghl' --get host list
    BEGIN
        SELECT ROW_NUMBER() OVER (ORDER BY CAST(a.Rank AS INT) ASC) AS SNO,
               a.AgentId,
               a.HostId,
			   a.Height Height,
			   a.Address Address,		   
               CASE WHEN  a.HostNameJapanese IS NOT  NULL THEN   a.HostNameJapanese + '( ' +a.HostName +' )' ELSE  a.HostName END  as HostName,
               a.Position,
               a.[Rank],
               a.DOB AS Age,
               a.Status,
               sd.StaticDataLabel AS ConstellationGroup,
               -- a.ImagePath,
               a.ActionUser,
               a.ActionIP,
               a.ActionPlatform,
               FORMAT(a.ActionDate, 'dd MMM, yyyy hh:mm:ss') CreatedDate,
               FORMAT(a.ActionDate, 'dd MMM, yyyy hh:mm:ss') UpdatedDate,
               b.ClubName1 AS ClubName,
               1 AS Ratings,
               100 AS TotalVisitors,
               --(
               --    SELECT TOP (1)
               --           c.ImagePath
               --    FROM dbo.tbl_gallery c WITH (NOLOCK)
               --    WHERE c.AgentId = a.HostId
               --          AND c.RoleId = 5
               --          AND ISNULL(c.Status, '') = 'A'
               --    ORDER BY c.Sno DESC
               --) AS HostImage,
               a.ImagePath AS HostImage,
               (
                   SELECT TOP (1)
                          c.ImagePath
                   FROM dbo.tbl_gallery c WITH (NOLOCK)
                   WHERE c.AgentId = a.HostId
                         AND c.RoleId = 5
                         AND ISNULL(c.Status, '') = 'A'
                   ORDER BY c.Sno DESC
               ) AS ImagePath,
               COUNT(a.AgentId) OVER () AS TotalRecords,
               a.HostNameJapanese,
               a.HostIntroduction
        FROM dbo.tbl_host_details a WITH (NOLOCK)
            INNER JOIN dbo.tbl_club_details b WITH (NOLOCK)
                ON b.AgentId = a.AgentId
                   AND ISNULL(b.Status, '') = 'A'
            LEFT JOIN dbo.tbl_static_data sd
                ON sd.StaticDataType = 13
                   AND sd.StaticDataValue = a.ConstellationGroup
                   AND sd.Status = 'A'
        WHERE ISNULL(a.[Status], '') IN ( 'A','B' )
              AND
              (
                  @AgentId IS NULL
                  OR b.AgentId = @AgentId
              )
              AND
              (
                  @SearchFilter IS NULL
                  OR (a.HostName LIKE '%' + @SearchFilter + '%')
              )
        ORDER BY CAST(a.Rank AS INT) ASC OFFSET @Skip ROWS FETCH NEXT @Take ROW ONLY;

        RETURN;
    END;
    ELSE IF ISNULL(@Flag, '') = 'ghd' --get host details
    BEGIN
        SELECT a.AgentId,
               a.HostId,
               a.HostName,
               a.Position,
               a.Rank,
               FORMAT(CONVERT(DATE, a.DOB), 'yyyy') AS Year,
               FORMAT(CONVERT(DATE, a.DOB), 'MM') AS Month,
               FORMAT(CONVERT(DATE, a.DOB), 'dd') AS Day,
               a.DOB,
               a.ConstellationGroup,
               a.Height,
               a.BloodType,
               a.PreviousOccupation,
               a.LiquorStrength,
               a.Status,
               b.WebsiteLink,
               b.InstagramLink,
               b.TiktokLink,
               b.TwitterLink,
               b.Line,
               a.ImagePath,
			   a.Icon IconImagePath,
               a.Address,
               a.HostNameJapanese,
               a.HostIntroduction
        FROM dbo.tbl_host_details a WITH (NOLOCK)
            INNER JOIN dbo.tbl_website_details b WITH (NOLOCK)
                ON b.AgentId = a.AgentId
                   AND b.HostId = @HostId
                   AND b.RoleId = 5
        WHERE a.AgentId = @AgentId
              AND a.HostId = @HostId
              AND ISNULL(a.[Status], '') IN ( 'A' );
    END;
    ELSE IF ISNULL(@Flag, '') = 'uhs' --update host status
    BEGIN
        IF NOT EXISTS
        (
            SELECT 'X'
            FROM dbo.tbl_host_details a WITH (NOLOCK)
            WHERE a.AgentId = @AgentId
                  AND a.HostId = @HostId
                  AND ISNULL(a.[Status], '') IN ( 'A', 'B' )
        )
        BEGIN
            SELECT 1 Code,
                   'Invalid host details' Message;
            RETURN;
        END;

        IF ISNULL(@Status, '') = 'A'
        BEGIN
            IF NOT EXISTS
            (
                SELECT 'X'
                FROM dbo.tbl_host_details a WITH (NOLOCK)
                WHERE a.AgentId = @AgentId
                      AND a.HostId = @HostId
                      AND ISNULL(a.[Status], '') = 'B'
            )
            BEGIN
                SELECT 1 Code,
                       'Invalid host status' Message;
                RETURN;
            END;

            UPDATE dbo.tbl_host_details
            SET Status = 'A',
                ActionUser = @ActionUser,
                ActionIP = @ActionIP,
                ActionPlatform = @ActionPlatform,
                ActionDate = GETDATE()
            WHERE AgentId = @AgentId
                  AND HostId = @HostId
                  AND ISNULL([Status], '') = 'B';
            SELECT 0 Code,
                   'Host status updated successfully' Message;
            RETURN;
        END;
        ELSE IF ISNULL(@Status, '') = 'B'
        BEGIN
            IF NOT EXISTS
            (
                SELECT 'X'
                FROM dbo.tbl_host_details a WITH (NOLOCK)
                WHERE a.AgentId = @AgentId
                      AND a.HostId = @HostId
                      AND ISNULL(a.[Status], '') = 'A'
            )
            BEGIN
                SELECT 1 Code,
                       'Invalid host status' Message;
                RETURN;
            END;

            UPDATE dbo.tbl_host_details
            SET Status = 'B',
                ActionUser = @ActionUser,
                ActionIP = @ActionIP,
                ActionPlatform = @ActionPlatform,
                ActionDate = GETDATE()
            WHERE AgentId = @AgentId
                  AND HostId = @HostId
                  AND ISNULL([Status], '') = 'A';

            SELECT 0 Code,
                   'Host deleted successfully' Message;
            RETURN;
        END;
        ELSE
        BEGIN
            SELECT 1 Code,
                   'Invalid status' Message;
            RETURN;
        END;
    END;

    ELSE IF ISNULL(@Flag, '') = 'dhi' --Disable Host
    BEGIN
        IF NOT EXISTS
        (
            SELECT 'X'
            FROM dbo.tbl_club_details a WITH (NOLOCK)
                INNER JOIN dbo.tbl_users b WITH (NOLOCK)
                    ON b.AgentId = a.AgentId
                       AND ISNULL(a.Status, '') IN ( 'A' )
            WHERE a.AgentId = @AgentId
                  AND ISNULL(b.Status, '') IN ( 'A' )
                  AND b.RoleType = 4
        )
        BEGIN
            SELECT 1 Code,
                   'Invalid user detail' Message;
            RETURN;
        END;

        UPDATE dbo.tbl_host_details
        SET [Status] = 'D',
            ActionDate = GETDATE(),
            ActionUser = @ActionUser
        WHERE HostId = @HostId
              AND AgentId = @AgentId;

        SELECT 0 Code,
               'Host deleted successfully' Message;
        RETURN;
    END;
    ELSE IF ISNULL(@Flag, '') = 'ghbi' -- Get Host By ID
    BEGIN
        IF NOT EXISTS
        (
            SELECT 'X'
            FROM dbo.tbl_club_details a WITH (NOLOCK)
                INNER JOIN dbo.tbl_users b WITH (NOLOCK)
                    ON b.AgentId = a.AgentId
                       AND ISNULL(a.Status, '') IN ( 'A' )
            WHERE a.AgentId = @AgentId
                  AND ISNULL(b.Status, '') IN ( 'A' )
                  AND b.RoleType = 4
        )
        BEGIN
            SELECT 1 Code,
                   'Invalid user detail' Message;
            RETURN;
        END;

        IF @HostId IS NULL
        BEGIN
            SELECT 101 code,
                   'Host Id is required' MESSAGE;
            RETURN;
        END;

        IF @AgentId IS NULL
        BEGIN
            SELECT 101 code,
                   'Agent Id is required' MESSAGE;
            RETURN;
        END;

        SELECT hd.Sno,
               hd.AgentId,
               hd.HostId,
               hd.HostName,
               hd.Position,
               wd.InstagramLink,
               wd.TiktokLink,
               wd.TwitterLink,
               wd.Line,
               [hd].[Rank],
               FORMAT(CONVERT(DATE, hd.DOB), 'yyyy') AS Year,
               FORMAT(CONVERT(DATE, hd.DOB), 'MM') AS Month,
               FORMAT(CONVERT(DATE, hd.DOB), 'dd') AS Day,
               hd.DOB,
               hd.ConstellationGroup,
               hd.Height,
               hd.BloodType,
               hd.PreviousOccupation,
               hd.LiquorStrength,
               [hd].[Status],
               hd.ActionDate,
               hd.Address,
               hd.ImagePath,
               hd.HostNameJapanese,
               hd.HostIntroduction
        FROM dbo.tbl_host_details hd WITH (NOLOCK)
            INNER JOIN dbo.tbl_website_details wd WITH (NOLOCK)
                ON hd.HostId = wd.HostId
                   AND wd.RoleId = 5
        WHERE wd.AgentId = @AgentId
              AND wd.HostId = @HostId;
        RETURN;
    END;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0
        ROLLBACK TRANSACTION @TransactionName;

    SET @ErrorDesc = 'SQL error found: (' + ERROR_MESSAGE() + ')' + ' at ' + CAST(ERROR_LINE() AS VARCHAR);

    INSERT INTO dbo.tbl_error_log
    (
        ErrorDesc,
        ErrorScript,
        QueryString,
        ErrorCategory,
        ErrorSource,
        ActionDate
    )
    VALUES
    (@ErrorDesc, 'sproc_host_management(Flag: ' + ISNULL(@Flag, '') + ')', 'SQL', 'SQL', 'sproc_host_management',
     GETDATE());

    SELECT 1 Code,
           'ErrorId:' + CAST(SCOPE_IDENTITY() AS VARCHAR) Message;
    RETURN;
END CATCH;
