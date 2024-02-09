USE [CRS];
GO

/****** Object:  StoredProcedure [dbo].[sproc_club_management]    Script Date: 2/7/2024 5:43:50 PM ******/
SET ANSI_NULLS ON;
GO

SET QUOTED_IDENTIFIER ON;
GO










--[dbo].[sproc_club_management]'cmg',3
ALTER PROC [dbo].[sproc_club_management]
    @Flag VARCHAR(10) = '',
    @AgentId VARCHAR(20) = NULL,
    @ImageID VARCHAR(20) = NULL,
    @UserId VARCHAR(20) = NULL,
    @ClubId VARCHAR(20) = NULL,
    @LoginId VARCHAR(200) = NULL,
    @FirstName NVARCHAR(200) = NULL,
    @MiddleName NVARCHAR(200) = NULL,
    @LastName NVARCHAR(200) = NULL,
    @Email VARCHAR(500) = NULL,
    @MobileNumber VARCHAR(11) = NULL,
    @ClubName1 NVARCHAR(500) = NULL,
    @ClubName2 NVARCHAR(500) = NULL,
    @BusinessType VARCHAR(8) = NULL,
    @GroupName NVARCHAR(512) = NULL,
    @Description NVARCHAR(512) = NULL,
    @LocationURL VARCHAR(MAX) = NULL,
    @Longitude VARCHAR(75) = NULL,
    @Latitude VARCHAR(75) = NULL,
    @Logo VARCHAR(MAX) = NULL,
    @CoverPhoto VARCHAR(MAX) = NULL,
    @BusinessCertificate VARCHAR(MAX) = NULL,
    @Gallery VARCHAR(MAX) = NULL,
    @ActionUser VARCHAR(200) = NULL,
    @ActionIP VARCHAR(50) = NULL,
    @ActionPlatform VARCHAR(20) = NULL,
    @WebsiteLink VARCHAR(MAX) = NULL,
    @TiktokLink VARCHAR(MAX) = NULL,
    @TwitterLink VARCHAR(MAX) = NULL,
    @InstagramLink VARCHAR(MAX) = NULL,
    @ImagePath VARCHAR(MAX) = NULL,
    @ImageTitle NVARCHAR(150) = NULL,
    @ClubSno VARCHAR(10) = NULL,
    @Status CHAR(1) = NULL,
    @LocationId VARCHAR(10) = NULL,
    @CompanyName NVARCHAR(512) = NULL,
    @SearchFilter NVARCHAR(200) = NULL,
    @Skip INT = 0,
    @Take INT = 10
AS
DECLARE @Sno VARCHAR(10),
        @Sno2 VARCHAR(10),
        @Sno3 VARCHAR(10),
        @Sno4 VARCHAR(10),
        @TransactionName VARCHAR(200),
        @ErrorDesc VARCHAR(MAX),
        @RandomPassword VARCHAR(20),
        @RoleId BIGINT,
        @RoleType VARCHAR(10),
        @SmsEmailResponseCode INT = 1;
DECLARE @SQLString NVARCHAR(MAX) = N'',
        @FetchQuery NVARCHAR(MAX),
        @SQLFilterParameter NVARCHAR(MAX) = N'';
BEGIN TRY
    IF ISNULL(@Flag, '') = 'gclist' --get club list
    BEGIN
        SELECT ROW_NUMBER() OVER (ORDER BY a.ClubName1 ASC) AS SNO,
               b.LoginId,
               a.AgentId,
               a.ClubName1 AS ClubNameEng,
               a.ClubName2 AS ClubNameJap,
               a.MobileNumber,
               ISNULL(c.LocationName, '-') AS Location,
               FORMAT(a.ActionDate, N'yyyy年MM月dd日 HH:mm:ss', 'ja-JP') AS CreatedDate,
               FORMAT(a.ActionDate, N'yyyy年MM月dd日 HH:mm:ss', 'ja-JP') AS UpdatedDate,
               --FORMAT(a.ActionDate, 'dd MMM, yyyy hh:mm:ss') AS CreatedDate,
               --FORMAT(a.ActionDate, 'dd MMM, yyyy hh:mm:ss') AS UpdatedDate,
               '1' AS Rank,
               '5' AS Ratings,
               a.Status,
               a.Sno,
               a.CompanyName,
               a.Logo AS ClubLogo,
               ISNULL(e.StaticDataLabel, '-') AS ClubCategory,
               COUNT(a.AgentId) OVER () AS TotalRecords
        FROM dbo.tbl_club_details a WITH (NOLOCK)
            INNER JOIN dbo.tbl_users b WITH (NOLOCK)
                ON b.AgentId = a.AgentId
                   AND b.RoleType = 4
                   AND ISNULL(b.Status, '') IN ( 'A', 'B' )
            LEFT JOIN dbo.tbl_location c WITH (NOLOCK)
                ON c.LocationId = a.LocationId
                   AND ISNULL(c.Status, '') = 'A'
            LEFT JOIN dbo.tbl_tag_detail d WITH (NOLOCK)
                ON d.ClubId = a.AgentId
                   AND ISNULL(d.Tag3Status, '') = 'A'
            LEFT JOIN dbo.tbl_static_data e WITH (NOLOCK)
                ON e.StaticDataType = 17
                   AND e.StaticDataValue = d.Tag3CategoryName
                   AND ISNULL(e.Status, '') = 'A'
        WHERE ISNULL(a.Status, '') IN ( 'A' )
              AND
              (
                  @SearchFilter IS NULL
                  OR
                  (
                      a.ClubName1 LIKE '%' + @SearchFilter + '%'
                      OR a.MobileNumber LIKE '%' + @SearchFilter + '%'
                      OR a.Email LIKE '%' + @SearchFilter + '%'
                  )
              )
        ORDER BY a.ClubName1 ASC OFFSET @Skip ROWS FETCH NEXT @Take ROW ONLY;
        RETURN;
    END;
    ELSE IF ISNULL(@Flag, '') = 'gcd' --get club details
    BEGIN
        IF NOT EXISTS
        (
            SELECT 'X'
            FROM dbo.tbl_club_details a WITH (NOLOCK)
                INNER JOIN dbo.tbl_users b WITH (NOLOCK)
                    ON b.AgentId = a.AgentId
                       AND b.RoleType = 4
                       AND ISNULL(b.Status, '') IN ( 'A', 'B' )
                LEFT JOIN dbo.tbl_website_details c WITH (NOLOCK)
                    ON c.AgentId = b.AgentId
                       AND c.RoleId = 4
            WHERE ISNULL(a.Status, '') IN ( 'A', 'B' )
                  AND a.AgentId = @AgentId
        )
        BEGIN
            SELECT 1 Code,
                   'Invalid details' Message;
            RETURN;
        END;

        SELECT 0 Code,
               'Success' Message,
               a.AgentId,
               a.FirstName,
               a.MiddleName,
               a.LastName,
               a.Email,
               a.MobileNumber,
               a.ClubName1,
               a.ClubName2,
               a.BusinessType,
               a.GroupName,
               a.Description,
               a.LocationURL,
               a.Longitude,
               a.Latitude,
               a.Status,
               a.Logo,
               a.CoverPhoto,
               a.BusinessCertificate,
               a.Gallery,
               a.ActionUser,
               a.ActionIP,
               a.ActionPlatform,
               a.ActionDate,
               b.UserId,
               b.LoginId,
               b.Status,
               c.WebsiteLink,
               c.TiktokLink,
               c.TwitterLink,
               c.InstagramLink,
               a.LocationId,
               a.CompanyName
        FROM dbo.tbl_club_details a WITH (NOLOCK)
            INNER JOIN dbo.tbl_users b WITH (NOLOCK)
                ON b.AgentId = a.AgentId
                   AND b.RoleType = 4
                   AND ISNULL(b.Status, '') IN ( 'A', 'B' )
            LEFT JOIN dbo.tbl_website_details c WITH (NOLOCK)
                ON c.AgentId = b.AgentId
                   AND c.RoleId = 4
        WHERE ISNULL(a.Status, '') IN ( 'A', 'B' )
              AND a.AgentId = @AgentId;

        RETURN;
    END;
    ELSE IF ISNULL(@Flag, '') = 'rc' --register club
    BEGIN
        IF ISNULL(@LoginId, '') = ''
        BEGIN
            SELECT 1 Code,
                   'Duplicate username' Message;
            RETURN;
        END;
        IF EXISTS
        (
            SELECT 'X'
            FROM dbo.tbl_club_details a WITH (NOLOCK)
                INNER JOIN dbo.tbl_users b WITH (NOLOCK)
                    ON b.AgentId = a.AgentId
                       AND ISNULL(b.Status, '') IN ( 'A', 'B' )
            WHERE b.LoginId = @LoginId
                  AND ISNULL(a.Status, '') IN ( 'A', 'B' )
        )
        BEGIN
            SELECT 1 Code,
                   'Duplicate username' Message;
            RETURN;
        END;

        SELECT @TransactionName = 'Flag_rc',
               @RandomPassword = dbo.func_generate_random_no(10);

        BEGIN TRANSACTION @TransactionName;

        INSERT INTO dbo.tbl_club_details
        (
            FirstName,
            MiddleName,
            LastName,
            Email,
            MobileNumber,
            ClubName1,
            ClubName2,
            BusinessType,
            GroupName,
            Description,
            LocationURL,
            Longitude,
            Latitude,
            Status,
            Logo,
            CoverPhoto,
            BusinessCertificate,
            Gallery,
            ActionUser,
            ActionIP,
            ActionPlatform,
            ActionDate,
            LocationId,
            CompanyName,
            CommissionId
        )
        VALUES
        (   @FirstName, @MiddleName, @LastName, @Email, @MobileNumber, @ClubName1, @ClubName2, @BusinessType,
            @GroupName, @Description, @LocationURL, @Longitude, @Latitude, 'A', --Active
            @Logo, @CoverPhoto, @BusinessCertificate, @Gallery, @ActionUser, @ActionIP, @ActionPlatform, GETDATE(),
            @LocationId, @CompanyName, 1);

        SET @Sno = SCOPE_IDENTITY();

        UPDATE dbo.tbl_club_details
        SET AgentId = @Sno
        WHERE Sno = @Sno;

        SELECT TOP 1
               @RoleId = a.Id,
               @RoleType = a.RoleType
        FROM dbo.tbl_roles a WITH (NOLOCK)
        WHERE a.RoleName = 'Club'
              AND ISNULL(a.Status, '') = 'A'
        ORDER BY 1 ASC;

        INSERT INTO dbo.tbl_users
        (
            RoleId,
            AgentId,
            LoginId,
            Password,
            Status,
            IsPrimary,
            ActionUser,
            ActionIP,
            ActionPlatform,
            ActionDate,
            RoleType,
            IsPasswordForceful
        )
        VALUES
        (   @RoleId, @Sno, @LoginId, PWDENCRYPT(@RandomPassword), 'A', --Active
            'Y', @ActionUser, @ActionIP, @ActionPlatform, GETDATE(), @RoleType, 'Y');

        SET @Sno2 = SCOPE_IDENTITY();

        UPDATE dbo.tbl_users
        SET UserId = @Sno2
        WHERE Sno = @Sno2
              AND AgentId = @Sno;

        INSERT INTO dbo.tbl_website_details
        (
            AgentId,
            WebsiteLink,
            TiktokLink,
            TwitterLink,
            InstagramLink,
            ActionUser,
            ActionIP,
            ActionPlatform,
            ActionDate,
            RoleId
        )
        VALUES
        (@Sno, @WebsiteLink, @TiktokLink, @TwitterLink, @InstagramLink, @ActionUser, @ActionIP, @ActionPlatform,
         GETDATE(), @RoleType);

        INSERT INTO dbo.tbl_tag_detail
        (
            ClubId,
            ActionUser,
            ActionIP,
            ActionDate
        )
        VALUES
        (   @Sno,        -- ClubId - bigint
            @ActionUser, -- ActionUser - varchar(200)
            @ActionIP,   -- ActionIP - varchar(50)
            GETDATE()    -- ActionDate - datetime
            );
        SET @Sno3 = SCOPE_IDENTITY();

        UPDATE dbo.tbl_tag_detail
        SET TagId = @Sno3
        WHERE Sno = @Sno3;

        COMMIT TRANSACTION @TransactionName;

        CREATE TABLE #temp_rc
        (
            Code INT
        );

        INSERT INTO #temp_rc
        (
            Code
        )
        EXEC dbo.sproc_club_email_sms_management @Flag = '1',
                                                 @LoginId = @LoginId,
                                                 @Password = @RandomPassword,
                                                 @EmailSendTo = @Email,
                                                 @Username = @ClubName1,
                                                 @AgentId = @Sno,
                                                 @UserId = @Sno2,
                                                 @ActionUser = @ActionUser,
                                                 @ActionIP = @ActionIP,
                                                 @ActionPlatform = @ActionPlatform,
                                                 @ResponseCode = @SmsEmailResponseCode OUTPUT;
        DROP TABLE #temp_rc;

        INSERT INTO dbo.tbl_customer_notification
        (
            ToAgentId,
            NotificationType,
            NotificationSubject,
            NotificationBody,
            NotificationStatus,
            NotificationReadStatus,
            CreatedBy,
            CreatedDate,
            NotificationURL,
            AdditionalDetail1,
            ToAgentType
        )
        VALUES
        (0, 'Club OnBoard Alert', 'Club OnBoard Alert', @ClubName2 + ' (' + @ClubName1 + ')', 'A', 'A', @ActionUser,
         GETDATE(), '#', @Sno, 'Customer');

        SET @Sno4 = SCOPE_IDENTITY();

        UPDATE dbo.tbl_customer_notification
        SET notificationId = @Sno4
        WHERE Sno = @Sno4;

        SELECT 0 Code,
               'Club registred successfully' Message;

        RETURN;
    END;
    ELSE IF ISNULL(@Flag, '') = 'mc' --manage club
    BEGIN
        IF NOT EXISTS
        (
            SELECT 'X'
            FROM dbo.tbl_club_details a WITH (NOLOCK)
                INNER JOIN dbo.tbl_users b WITH (NOLOCK)
                    ON b.AgentId = a.AgentId
                       AND b.RoleType = 4
                       AND ISNULL(b.Status, '') IN ( 'A' )
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

        UPDATE dbo.tbl_club_details
        SET FirstName = ISNULL(@FirstName, FirstName),
            MiddleName = ISNULL(@MiddleName, MiddleName),
            LastName = ISNULL(@LastName, LastName),
            ClubName1 = ISNULL(@ClubName1, ClubName1),
            ClubName2 = ISNULL(@ClubName2, ClubName2),
            BusinessType = ISNULL(@BusinessType, BusinessType),
            GroupName = ISNULL(@GroupName, GroupName),
            Description = ISNULL(@Description, Description),
            LocationURL = ISNULL(@LocationURL, LocationURL),
            Longitude = ISNULL(@Longitude, Longitude),
            Latitude = ISNULL(@Latitude, Latitude),
            Logo = ISNULL(@Logo, Logo),
            CoverPhoto = ISNULL(@CoverPhoto, CoverPhoto),
            BusinessCertificate = ISNULL(@BusinessCertificate, BusinessCertificate),
            Gallery = ISNULL(@Gallery, Gallery),
            ActionUser = @ActionUser,
            ActionIP = @ActionIP,
            ActionPlatform = @ActionPlatform,
            ActionDate = GETDATE(),
            LocationId = ISNULL(@LocationId, LocationId),
            CompanyName = ISNULL(@CompanyName, CompanyName)
        WHERE AgentId = @AgentId
              AND ISNULL(Status, '') IN ( 'A' );

        UPDATE dbo.tbl_website_details
        SET WebsiteLink = ISNULL(@WebsiteLink, WebsiteLink),
            TiktokLink = ISNULL(@TiktokLink, TiktokLink),
            TwitterLink = ISNULL(@TwitterLink, TwitterLink),
            InstagramLink = ISNULL(@InstagramLink, InstagramLink),
            ActionUser = @ActionUser,
            ActionIP = @ActionIP,
            ActionPlatform = @ActionPlatform,
            ActionDate = GETDATE()
        WHERE AgentId = @AgentId
              AND RoleId = 4;

        INSERT INTO dbo.tbl_customer_notification
        (
            ToAgentId,
            NotificationType,
            NotificationSubject,
            NotificationBody,
            NotificationStatus,
            NotificationReadStatus,
            CreatedBy,
            CreatedDate,
            NotificationURL,
            AdditionalDetail1,
            ToAgentType
        )
        VALUES
        (0, 'Club OnBoard Alert', 'Club OnBoard Alert', @ClubName2 + ' (' + @ClubName1 + ')', 'A', 'A', @ActionUser,
         GETDATE(), '#', @AgentId, 'Customer');

        SET @Sno4 = SCOPE_IDENTITY();

        UPDATE dbo.tbl_customer_notification
        SET notificationId = @Sno4
        WHERE Sno = @Sno4;

        SELECT 0 Code,
               'Club details updated successfully' Message;

        COMMIT TRANSACTION @TransactionName;
        RETURN;
    END;
    ELSE IF ISNULL(@Flag, '') = 'ucs' --update club status
    BEGIN
        IF NOT EXISTS
        (
            SELECT 'X'
            FROM dbo.tbl_club_details a WITH (NOLOCK)
            WHERE a.AgentId = @AgentId
        )
        BEGIN
            SELECT 1 Code,
                   'Invalid details' Message;
            RETURN;
        END;

        IF ISNULL(@Status, '') NOT IN ( 'A', 'B', 'D' )
        BEGIN
            SELECT 1 Code,
                   'Invalid status' Message;
            RETURN;
        END;

        SET @TransactionName = 'Flag_ucs';

        BEGIN TRANSACTION @TransactionName;

        IF ISNULL(@Status, '') = 'A'
        BEGIN
            IF NOT EXISTS
            (
                SELECT 'X'
                FROM dbo.tbl_club_details a WITH (NOLOCK)
                    INNER JOIN dbo.tbl_users b WITH (NOLOCK)
                        ON b.AgentId = a.AgentId
                           AND ISNULL(a.Status, '') = 'B'
                           AND b.RoleType = 4
                WHERE a.AgentId = @AgentId
                      AND ISNULL(b.Status, '') = 'B'
            )
            BEGIN
                SELECT 1 Code,
                       'Invalid details' Message;

                ROLLBACK TRANSACTION @TransactionName;
                RETURN;
            END;

            UPDATE dbo.tbl_users
            SET Status = 'A',
                ActionUser = @ActionUser,
                ActionIP = @ActionIP,
                ActionPlatform = @ActionPlatform,
                ActionDate = GETDATE()
            WHERE AgentId = @AgentId
                  AND RoleType = 4
                  AND ISNULL(IsPrimary, '') = 'Y'
                  AND ISNULL(Status, '') IN ( 'B' );

            UPDATE dbo.tbl_club_details
            SET Status = 'A',
                ActionUser = @ActionUser,
                ActionIP = @ActionIP,
                ActionPlatform = @ActionPlatform,
                ActionDate = GETDATE()
            WHERE AgentId = @AgentId;
        END;
        ELSE IF ISNULL(@Status, '') = 'B'
        BEGIN
            IF NOT EXISTS
            (
                SELECT 'X'
                FROM dbo.tbl_club_details a WITH (NOLOCK)
                    INNER JOIN dbo.tbl_users b WITH (NOLOCK)
                        ON b.AgentId = a.AgentId
                           AND ISNULL(a.Status, '') = 'A'
                           AND b.RoleType = 4
                WHERE a.AgentId = @AgentId
                      AND ISNULL(b.Status, '') = 'A'
            )
            BEGIN
                SELECT 1 Code,
                       'Invalid details' Message;

                ROLLBACK TRANSACTION @TransactionName;
                RETURN;
            END;

            UPDATE dbo.tbl_users
            SET Status = 'B',
                ActionUser = @ActionUser,
                ActionIP = @ActionIP,
                ActionPlatform = @ActionPlatform,
                ActionDate = GETDATE()
            WHERE AgentId = @AgentId
                  AND RoleType = 4
                  AND ISNULL(Status, '') = 'A';

            UPDATE dbo.tbl_club_details
            SET Status = 'B',
                ActionUser = @ActionUser,
                ActionIP = @ActionIP,
                ActionPlatform = @ActionPlatform,
                ActionDate = GETDATE()
            WHERE AgentId = @AgentId;
        END;
        ELSE IF ISNULL(@Status, '') = 'D'
        BEGIN
            IF NOT EXISTS
            (
                SELECT 'X'
                FROM dbo.tbl_club_details a WITH (NOLOCK)
                    INNER JOIN dbo.tbl_users b WITH (NOLOCK)
                        ON b.AgentId = a.AgentId
                           AND ISNULL(a.Status, '') = 'A'
                           AND b.RoleType = 4
                WHERE a.AgentId = @AgentId
                      AND ISNULL(b.Status, '') = 'A'
            )
            BEGIN
                SELECT 1 Code,
                       'Invalid details' Message;

                ROLLBACK TRANSACTION @TransactionName;
                RETURN;
            END;

            UPDATE dbo.tbl_users
            SET Status = 'D',
                ActionUser = @ActionUser,
                ActionIP = @ActionIP,
                ActionPlatform = @ActionPlatform,
                ActionDate = GETDATE()
            WHERE AgentId = @AgentId
                  AND RoleType = 4
                  AND ISNULL(Status, '') = 'A';

            UPDATE dbo.tbl_club_details
            SET Status = 'D',
                ActionUser = @ActionUser,
                ActionIP = @ActionIP,
                ActionPlatform = @ActionPlatform,
                ActionDate = GETDATE()
            WHERE AgentId = @AgentId;

            SELECT 0 Code,
                   'Club deleted successfully' Message;

            COMMIT TRANSACTION @TransactionName;
            RETURN;
        END;
        SELECT 0 Code,
               'User status updated successfully' Message;

        COMMIT TRANSACTION @TransactionName;
        RETURN;

    END;
    ELSE IF ISNULL(@Flag, '') = 'rcup' --reset club user password
    BEGIN
        IF NOT EXISTS
        (
            SELECT 'X'
            FROM dbo.tbl_club_details a WITH (NOLOCK)
                INNER JOIN dbo.tbl_users b WITH (NOLOCK)
                    ON b.AgentId = a.AgentId
                       AND RoleType = 4
                       AND ISNULL(a.Status, '') IN ( 'A' )
            WHERE a.AgentId = @AgentId
                  --AND b.UserId = @UserId				
                  AND ISNULL(b.Status, '') IN ( 'A' )
        )
        BEGIN
            SELECT 1 Code,
                   'Invalid user detail' Message;
            RETURN;
        END;

        SET @RandomPassword = dbo.func_generate_random_no(10);

        UPDATE dbo.tbl_users
        SET Password = PWDENCRYPT(@RandomPassword),
            ActionUser = @ActionUser,
            ActionIP = @ActionIP,
            ActionPlatform = @ActionPlatform,
            ActionDate = GETDATE()
        WHERE AgentId = @AgentId
              AND RoleType = 4;
        --AND UserId = @UserId;

        SELECT 0 Code,
               'Club user password reset successfully' Message;

        RETURN;
    END;
    ELSE IF ISNULL(@Flag, '') = 'aci' --Add club image
    BEGIN
        IF NOT EXISTS
        (
            SELECT 'X'
            FROM dbo.tbl_club_details a WITH (NOLOCK)
                INNER JOIN dbo.tbl_users b WITH (NOLOCK)
                    ON b.AgentId = a.AgentId
                       AND b.RoleType = 4
                       AND ISNULL(a.Status, '') IN ( 'A' )
            WHERE a.AgentId = @AgentId
                  AND ISNULL(b.Status, '') IN ( 'A' )
        )
        BEGIN
            SELECT 1 Code,
                   'Invalid user detail' Message;
            RETURN;
        END;

        IF @ImageTitle IS NULL
        BEGIN
            SELECT 11 CODE,
                   'Image title is required' MESSAGE;
            RETURN;
        END;

        IF @ImagePath IS NULL
        BEGIN
            SELECT 10 CODE,
                   'Image path is required' MESSAGE;
            RETURN;
        END;

        INSERT INTO dbo.tbl_gallery
        (
            RoleId,
            AgentId,
            ImageTitle,
            ImagePath,
            [Status],
            CreatedBy,
            CreatedDate
        )
        VALUES
        (4, @AgentId, @ImageTitle, @ImagePath, 'A', @ActionUser, GETDATE());

        --UPDATE dbo.tbl_gallery
        --SET RoleId = '4'
        --WHERE AgentId = @AgentId;
        SELECT 0 Code,
               'Club Image Inserted Successfully' Message;
        RETURN;
    END;
    --ELSE IF ISNULL(@Flag, '') = 'cmg' --Club Management Gallery
    --BEGIN
    --    SELECT DISTINCT ga.AgentId,
    --           ga.ImagePath,
    --           ga.ImageTitle,
    --           ga.Sno,
    --           ga.CreatedDate,
    --           ga.UpdatedDate,
    --           ga.Status
    --    FROM dbo.tbl_roles tr
    --        INNER JOIN dbo.tbl_users tu
    --            ON tu.RoleType = tr.RoleType
    --        INNER JOIN dbo.tbl_gallery ga WITH (NOLOCK)
    --            ON ga.RoleId = tu.RoleType
    --    WHERE ga.Status = 'A'
    --          AND ga.RoleId IN ( '4', '5' )
    --          AND ga.AgentId = 3;

    --    RETURN;
    --END;
    ELSE IF ISNULL(@Flag, '') = 'cmg' --Club Management Gallery
    BEGIN
        IF ISNULL(@SearchFilter, '') <> ''
        BEGIN
            SET @SQLFilterParameter = N' AND ga.ImageTitle LIKE N''%' + @SearchFilter + N'%''';
        END;
        IF ISNULL(@AgentId, '') <> ''
        BEGIN
            BEGIN
                SET @SQLFilterParameter += N' AND ga.AgentId=' + @AgentId;
            END;
        END;
        SET @FetchQuery
            = N' OFFSET ' + CAST(ISNULL(@Skip, '0') AS VARCHAR) + N' ROWS FETCH NEXT '
              + CAST(ISNULL(@Take, '10') AS VARCHAR) + N' ROW ONLY';

        SET @SQLString
            = N'
        SELECT ROW_NUMBER() OVER (ORDER BY ga.CreatedDate DESC) AS SNO,
               b.LoginId,
               ga.AgentId,
               ga.ImagePath,
               ga.ImageTitle,
               ga.Sno,
               FORMAT(ga.CreatedDate, N''yyyy年MM月dd日 HH:mm:ss'', ''ja-JP'') as CreatedDate,
			   FORMAT(ga.UpdatedDate, N''yyyy年MM月dd日 HH:mm:ss'', ''ja-JP'') as UpdatedDate,
               ga.Status,
			   COUNT(ga.AgentId) OVER() AS TotalRecords
        FROM dbo.tbl_gallery ga WITH (NOLOCK)
            INNER JOIN dbo.tbl_users b WITH (NOLOCK)
                ON b.AgentId = ga.AgentId
                   AND ISNULL(b.Status, '''') IN ( ''A'' )
                   AND b.RoleType = 4
                   AND ga.RoleId = 4
        WHERE ISNULL(ga.Status, '''') IN ( ''A'' ) ' + @SQLFilterParameter + N' ORDER BY ga.CreatedDate DESC '
              + @FetchQuery;
        PRINT (@SQLString);
        EXEC (@SQLString);
        RETURN;
    END;
    ELSE IF ISNULL(@Flag, '') = 'dgi' --delete gallery image
    BEGIN
        --IF NOT EXISTS
        --      (
        --          SELECT 'X'
        --          FROM tbl_club_details a WITH (NOLOCK)
        --              INNER JOIN tbl_users b WITH (NOLOCK)
        --                  ON b.AgentId = a.AgentId
        --                     AND ISNULL(a.Status, '') IN ( 'A' )
        --          WHERE a.AgentId = @AgentId
        --                AND ISNULL(b.Status, '') IN ( 'A' )
        --      )
        --      BEGIN
        --          SELECT 1 Code,
        --                 'Invalid user detail' Message;
        --          RETURN;
        --      END;

        DELETE FROM dbo.tbl_gallery
        WHERE Sno = @ClubSno
              AND AgentId = @AgentId
              AND RoleId = 4;
        --UPDATE tbl_gallery
        --SET [Status] = 'D',
        --	UpdatedBy = @ActionUser,
        --	UpdatedIP = @ActionIP,
        --	UpdatedDate = GETDATE()
        --WHERE Sno = @ImageID
        --		AND AgentId = @AgentId 

        SELECT 0 Code,
               'Club Gallery Image deleted successfully' Message;
        RETURN;
    END;
    ELSE IF ISNULL(@Flag, '') = 'gci' --get club image
    BEGIN
        IF NOT EXISTS
        (
            SELECT 'X'
            FROM dbo.tbl_club_details a WITH (NOLOCK)
                INNER JOIN dbo.tbl_users b WITH (NOLOCK)
                    ON b.AgentId = a.AgentId
                       AND ISNULL(a.Status, '') IN ( 'A' )
                       AND b.RoleType = 4
            WHERE a.AgentId = @AgentId
                  AND ISNULL(b.Status, '') IN ( 'A' )
        )
        BEGIN
            SELECT 1 Code,
                   'Invalid user detail' Message;
            RETURN;
        END;

        IF @ClubSno IS NULL
        BEGIN
            SELECT 101 code,
                   'Club sno is required' MESSAGE;
            RETURN;
        END;

        SELECT ImageTitle,
               ImagePath,
               Sno,
               AgentId
        FROM dbo.tbl_gallery
        WHERE AgentId = @AgentId
              AND Sno = @ClubSno
              AND RoleId = 4;
        RETURN;
    END;
    ELSE IF ISNULL(@Flag, '') = 'uci' --Update club image
    BEGIN
        IF NOT EXISTS
        (
            SELECT 'X'
            FROM dbo.tbl_club_details a WITH (NOLOCK)
                INNER JOIN dbo.tbl_users b WITH (NOLOCK)
                    ON b.AgentId = a.AgentId
                       AND ISNULL(a.Status, '') IN ( 'A' )
                       AND b.RoleType = 4
            WHERE a.AgentId = @AgentId
                  AND ISNULL(b.Status, '') IN ( 'A' )
        )
        BEGIN
            SELECT 1 Code,
                   'Invalid user detail' Message;
            RETURN;
        END;

        IF @ImageID IS NULL
        BEGIN
            SELECT 1 CODE,
                   'Image is required' MESSAGE;
            RETURN;
        END;

        IF @ImagePath IS NULL
        BEGIN
            SELECT 1 CODE,
                   'Invalid Detail' MESSAGE;
            RETURN;
        END;

        UPDATE dbo.tbl_gallery
        SET ImagePath = @ImagePath,
            ImageTitle = ISNULL(@ImageTitle, 'title was null'),
            UpdatedBy = @ActionUser,
            UpdatedIP = ISNULL(@ActionIP, '::1'),
            UpdatedDate = GETDATE()
        WHERE Sno = @ImageID
              AND AgentId = @AgentId
              AND RoleId = 4;

        SELECT 0 Code,
               'Club Image updated successfully' Message;
        RETURN;
    END;

    ELSE
    BEGIN
        SELECT 1 Code,
               'Invalid function' Message;
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
    (@ErrorDesc, 'sproc_club_management(Flag: ' + ISNULL(@Flag, '') + ')', 'SQL', 'SQL', 'sproc_club_management',
     GETDATE());

    SELECT 1 Code,
           'ErrorId:' + CAST(SCOPE_IDENTITY() AS VARCHAR) Message;
    RETURN;
END CATCH;
GO


