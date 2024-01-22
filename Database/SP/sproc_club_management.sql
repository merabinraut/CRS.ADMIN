USE [CRS]
GO

/****** Object:  StoredProcedure [dbo].[sproc_club_management]    Script Date: 23/11/2023 21:02:38 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO










ALTER PROC [dbo].[sproc_club_management]
    @Flag VARCHAR(10),
    @AgentId VARCHAR(20) = NULL,
    @ImageID VARCHAR(20) = NULL,
    @UserId VARCHAR(20) = NULL,
    @ClubId VARCHAR(20) = NULL,
    @LoginId VARCHAR(200) = NULL,
    @FirstName VARCHAR(200) = NULL,
    @MiddleName VARCHAR(200) = NULL,
    @LastName VARCHAR(200) = NULL,
    @Email VARCHAR(500) = NULL,
    @MobileNumber VARCHAR(11) = NULL,
    @ClubName1 VARCHAR(500) = NULL,
    @ClubName2 NVARCHAR(500) = NULL,
    @BusinessType VARCHAR(8) = NULL,
    @GroupName VARCHAR(500) = NULL,
    @Description VARCHAR(500) = NULL,
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
    @ImageTitle VARCHAR(150) = NULL,
    @ClubSno VARCHAR(10) = NULL,
    @Status CHAR(1) = NULL,
    @LocationId VARCHAR(10) = NULL,
    @CompanyName NVARCHAR(512) = NULL
AS
DECLARE @Sno VARCHAR(10),
        @Sno2 VARCHAR(10),
        @Sno3 VARCHAR(10),
        @TransactionName VARCHAR(200),
        @ErrorDesc VARCHAR(MAX),
        @RandomPassword VARCHAR(20),
        @RoleId BIGINT;
BEGIN TRY
    IF ISNULL(@Flag, '') = 'gclist' --get club list
    BEGIN
        SELECT b.LoginId,
               a.AgentId,
               a.ClubName1 AS ClubNameEng,
               a.ClubName2 AS ClubNameJap,
               a.MobileNumber,
               'Default Location' AS Location,
               a.ActionDate AS CreatedDate,
               --CONVERT(VARCHAR, a.ActionDate, 100) AS CreatedDate,
               a.ActionDate AS UpdatedDate,
               --CONVERT(VARCHAR, a.ActionDate, 100) AS UpdatedDate,
               '1' AS Rank,
               '5' AS Ratings,
               a.Status,
               a.Sno,
               a.CompanyName,
			   a.Logo AS ClubLogo
        FROM tbl_club_details a WITH (NOLOCK)
            INNER JOIN tbl_users b WITH (NOLOCK)
                ON b.AgentId = a.AgentId
                   AND b.RoleId = 5
                   AND ISNULL(b.Status, '') NOT IN ( 'D', '' )
        WHERE ISNULL(a.Status, '') NOT IN ( 'D', '' );
        RETURN;
    END;
    ELSE IF ISNULL(@Flag, '') = 'gcd' --get club details
    BEGIN
        IF NOT EXISTS
        (
            SELECT 'X'
            FROM tbl_club_details a WITH (NOLOCK)
                INNER JOIN tbl_users b WITH (NOLOCK)
                    ON b.AgentId = a.AgentId
                       AND b.RoleId = 5
                       AND ISNULL(b.Status, '') NOT IN ( 'D', '' )
                LEFT JOIN tbl_website_details c WITH (NOLOCK)
                    ON c.AgentId = b.AgentId
            WHERE ISNULL(a.Status, '') NOT IN ( 'D', '' )
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
        FROM tbl_club_details a WITH (NOLOCK)
            INNER JOIN tbl_users b WITH (NOLOCK)
                ON b.AgentId = a.AgentId
                   AND b.RoleId = 5
                   AND ISNULL(b.Status, '') NOT IN ( 'D', '' )
            LEFT JOIN tbl_website_details c WITH (NOLOCK)
                ON c.AgentId = b.AgentId
                   AND b.RoleId = 5
        WHERE ISNULL(a.Status, '') NOT IN ( 'D', '' )
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
            FROM tbl_club_details a WITH (NOLOCK)
                INNER JOIN tbl_users b WITH (NOLOCK)
                    ON b.AgentId = a.AgentId
                       AND ISNULL(b.Status, '') NOT IN ( 'D', '' )
            WHERE b.LoginId = @LoginId
                  AND ISNULL(a.Status, '') NOT IN ( 'D', '' )
        )
        BEGIN
            SELECT 1 Code,
                   'Duplicate username' Message;
            RETURN;
        END;

        SELECT @TransactionName = 'Flag_rc',
               @RandomPassword = dbo.func_generate_random_no(8);

        BEGIN TRANSACTION @TransactionName;

        INSERT INTO tbl_club_details
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
            CompanyName
        )
        VALUES
        (   @FirstName, @MiddleName, @LastName, @Email, @MobileNumber, @ClubName1, @ClubName2, @BusinessType,
            @GroupName, @Description, @LocationURL, @Longitude, @Latitude, 'A', --Active
            @Logo, @CoverPhoto, @BusinessCertificate, @Gallery, @ActionUser, @ActionIP, @ActionPlatform, GETDATE(),
            @LocationId, @CompanyName);

        SET @Sno = SCOPE_IDENTITY();

        UPDATE tbl_club_details
        SET AgentId = @Sno
        WHERE Sno = @Sno;

        SELECT @RoleId = a.Id
        FROM dbo.tbl_roles a WITH (NOLOCK)
        WHERE a.RoleName = 'Club'
              AND ISNULL(a.Status, '') = 'A';

        INSERT INTO tbl_users
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
            ActionDate
        )
        VALUES
        (   @RoleId, @Sno, @LoginId, PWDENCRYPT(@RandomPassword), 'A', --Active
            'Y', @ActionUser, @ActionIP, @ActionPlatform, GETDATE());

        SET @Sno2 = SCOPE_IDENTITY();

        UPDATE tbl_users
        SET UserId = @Sno2
        WHERE Sno = @Sno2
              AND AgentId = @Sno;

        INSERT INTO tbl_website_details
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
         GETDATE(), 5);

        SELECT 0 Code,
               'Club registred successfully' Message;

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

        UPDATE tbl_tag_detail
        SET TagId = @Sno3
        WHERE Sno = @Sno3;

        COMMIT TRANSACTION @TransactionName;
        RETURN;
    END;
    ELSE IF ISNULL(@Flag, '') = 'mc' --manage club
    BEGIN
        IF NOT EXISTS
        (
            SELECT 'X'
            FROM tbl_club_details a WITH (NOLOCK)
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

        SET @TransactionName = 'Flag_mc';

        BEGIN TRANSACTION @TransactionName;

        UPDATE tbl_club_details
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
              AND ISNULL(Status, '') NOT IN ( 'D', '' );

        IF EXISTS
        (
            SELECT 'X'
            FROM dbo.tbl_website_details a WITH (NOLOCK)
            WHERE a.AgentId = @AgentId
                  AND a.RoleId = 5
        )
        BEGIN
            UPDATE tbl_website_details
            SET WebsiteLink = ISNULL(@WebsiteLink, WebsiteLink),
                TiktokLink = ISNULL(@TiktokLink, TiktokLink),
                TwitterLink = ISNULL(@TwitterLink, TwitterLink),
                InstagramLink = ISNULL(@InstagramLink, InstagramLink),
                ActionUser = @ActionUser,
                ActionIP = @ActionIP,
                ActionPlatform = @ActionPlatform,
                ActionDate = GETDATE()
            WHERE AgentId = @AgentId
                  AND RoleId = 5;
        END;
        ELSE
        BEGIN
            INSERT INTO tbl_website_details
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
            (@AgentId, @WebsiteLink, @TiktokLink, @TwitterLink, @InstagramLink, @ActionUser, @ActionIP,
             @ActionPlatform, GETDATE(), 5);
        END;

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
            FROM tbl_club_details a WITH (NOLOCK)
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
                FROM tbl_club_details a WITH (NOLOCK)
                    INNER JOIN tbl_users b WITH (NOLOCK)
                        ON b.AgentId = a.AgentId
                           AND ISNULL(a.Status, '') = 'I'
                WHERE a.AgentId = @AgentId
                      AND ISNULL(b.Status, '') = 'I'
            )
            BEGIN
                SELECT 1 Code,
                       'Invalid details' Message;

                ROLLBACK TRANSACTION @TransactionName;
                RETURN;
            END;

            UPDATE tbl_users
            SET Status = 'A',
                ActionUser = @ActionUser,
                ActionIP = @ActionIP,
                ActionPlatform = @ActionPlatform,
                ActionDate = GETDATE()
            WHERE AgentId = @AgentId
                  AND ISNULL(IsPrimary, '') = 'Y'
                  AND ISNULL(Status, '') NOT IN ( 'D', 'S' );
        END;
        ELSE
        BEGIN
            IF NOT EXISTS
            (
                SELECT 'X'
                FROM tbl_club_details a WITH (NOLOCK)
                    INNER JOIN tbl_users b WITH (NOLOCK)
                        ON b.AgentId = a.AgentId
                           AND ISNULL(a.Status, '') = 'A'
                WHERE a.AgentId = @AgentId
                      AND ISNULL(b.Status, '') = 'A'
            )
            BEGIN
                SELECT 1 Code,
                       'Invalid details' Message;

                ROLLBACK TRANSACTION @TransactionName;
                RETURN;
            END;

            UPDATE tbl_users
            SET Status = 'D',
                ActionUser = @ActionUser,
                ActionIP = @ActionIP,
                ActionPlatform = @ActionPlatform,
                ActionDate = GETDATE()
            WHERE AgentId = @AgentId;
        END;

        UPDATE tbl_club_details
        SET Status = 'D',
            ActionUser = @ActionUser,
            ActionIP = @ActionIP,
            ActionPlatform = @ActionPlatform,
            ActionDate = GETDATE()
        WHERE AgentId = @AgentId;

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
            FROM tbl_club_details a WITH (NOLOCK)
                INNER JOIN tbl_users b WITH (NOLOCK)
                    ON b.AgentId = a.AgentId
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

        SET @RandomPassword = dbo.func_generate_random_no(8);

        UPDATE tbl_users
        SET Password = PWDENCRYPT(@RandomPassword),
            ActionUser = @ActionUser,
            ActionIP = @ActionIP,
            ActionPlatform = @ActionPlatform,
            ActionDate = GETDATE()
        WHERE AgentId = @AgentId;
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
            FROM tbl_club_details a WITH (NOLOCK)
                INNER JOIN tbl_users b WITH (NOLOCK)
                    ON b.AgentId = a.AgentId
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
        (NULL, @AgentId, @ImageTitle, @ImagePath, 'A', @ActionUser, GETDATE());

        SELECT 0 Code,
               'Club Image Inserted Successfully' Message;
        RETURN;
    END;
    ELSE IF ISNULL(@Flag, '') = 'cmg' --Club Management Gallery
    BEGIN
        SELECT b.LoginId,
               ga.AgentId,
               ga.ImagePath,
               ga.ImageTitle,
               ga.Sno,
               ga.CreatedDate,
               ga.UpdatedDate
        FROM tbl_gallery ga WITH (NOLOCK)
            INNER JOIN tbl_users b WITH (NOLOCK)
                ON b.AgentId = ga.AgentId
                   AND ISNULL(b.Status, '') NOT IN ( 'D', '' )
        WHERE ISNULL(ga.Status, '') NOT IN ( 'D', '' );

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

        DELETE FROM tbl_gallery
        WHERE Sno = @ClubSno
              AND AgentId = @AgentId;
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
            FROM tbl_club_details a WITH (NOLOCK)
                INNER JOIN tbl_users b WITH (NOLOCK)
                    ON b.AgentId = a.AgentId
                       AND ISNULL(a.Status, '') IN ( 'A' )
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
        FROM tbl_gallery
        WHERE AgentId = @AgentId
              AND Sno = @ClubSno;
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
    (@ErrorDesc, 'sproc_club_management(Flag: ' + ISNULL(@Flag, '') + ')', 'SQL', 'SQL', 'sproc_club_management',
     GETDATE());

    SELECT 1 Code,
           'ErrorId:' + CAST(SCOPE_IDENTITY() AS VARCHAR) Message;
    RETURN;
END CATCH;
GO


