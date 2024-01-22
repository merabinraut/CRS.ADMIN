USE [CRS]
GO

/****** Object:  StoredProcedure [dbo].[sproc_admin_searchpage_recommendation_management_v2]    Script Date: 05/12/2023 15:28:43 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO






ALTER PROC [dbo].[sproc_admin_searchpage_recommendation_management_v2]
    @Flag VARCHAR(10),
    @RecommendationId VARCHAR(10) = NULL,
    @ClubId VARCHAR(10) = NULL,
    @LocationId VARCHAR(10) = NULL,
    @ActionUser VARCHAR(200) = NULL,
    @ActionIP VARCHAR(50) = NULL,
    @ActionPlatform VARCHAR(20) = NULL,
    @DisplayId VARCHAR(10) = NULL,
    @HostId VARCHAR(10) = NULL
AS
DECLARE @Sno BIGINT,
        @Sno2 BIGINT,
        @StringSQL VARCHAR(MAX),
        @StringSQL2 VARCHAR(MAX),
        @TransactionName VARCHAR(200),
        @ErrorDesc VARCHAR(MAX);
DECLARE @MaxNoOfClub INT = 10,
        @DisplayPageId VARCHAR(10) = 2;
BEGIN TRY
    IF @Flag = 'ccspr' --create club search page recommendation
    BEGIN
        IF EXISTS
        (
            SELECT 'X'
            FROM dbo.tbl_recommendation_detail a WITH (NOLOCK)
                INNER JOIN dbo.tbl_display_page b WITH (NOLOCK)
                    ON b.RecommendationId = a.RecommendationId
                       AND a.ClubId = @ClubId
                       AND a.LocationId = @LocationId
                       AND b.DisplayPageId = @DisplayPageId
                       AND ISNULL(b.Status, '') = 'A'
        )
        BEGIN
            SELECT 1 Code,
                   'Search page recommendation already exists for the given club' Message;
            RETURN;
        END;

        IF
        (
            SELECT COUNT(b.Sno)
            FROM dbo.tbl_recommendation_detail a WITH (NOLOCK)
                INNER JOIN dbo.tbl_display_page b WITH (NOLOCK)
                    ON b.RecommendationId = a.RecommendationId
                       AND a.LocationId = @LocationId
                       AND b.DisplayPageId = @DisplayPageId
                       AND ISNULL(b.Status, '') = 'A'
        ) >= @MaxNoOfClub
        BEGIN
            SELECT 1 Code,
                   'Search page recommendation already consist of maximum number of club' Message;
            RETURN;
        END;

        SELECT @RecommendationId = a.RecommendationId
        FROM dbo.tbl_recommendation_detail a WITH (NOLOCK)
        WHERE a.LocationId = @LocationId
              AND a.ClubId = @ClubId
              AND a.Status = 'A';

        SET @TransactionName = 'Transaction_ccspr';

        BEGIN TRANSACTION @TransactionName;

        IF ISNULL(@RecommendationId, '') = ''
        BEGIN
            INSERT INTO dbo.tbl_recommendation_detail
            (
                LocationId,
                ClubId,
                Status,
                CreatedBy,
                CreatedDate,
                CreatedIP,
                CreatedPlatform
            )
            VALUES
            (   @LocationId,    -- LocationId - bigint
                @ClubId,        -- ClubId - bigint
                'A',            -- Status - char(1)
                @ActionUser,    -- CreatedBy - varchar(200)
                GETDATE(),      -- CreatedDate - datetime
                @ActionIP,      -- CreatedIP - varchar(50)
                @ActionPlatform -- CreatedPlatform - varchar(50)
                );

            SET @RecommendationId = SCOPE_IDENTITY();

            UPDATE dbo.tbl_recommendation_detail
            SET RecommendationId = @RecommendationId
            WHERE Sno = @RecommendationId;
        END;

        INSERT INTO dbo.tbl_display_page
        (
            RecommendationId,
            DisplayPageId,
            Status,
            CreatedBy,
            CreatedDate,
            CreatedIP,
            CreatedPlatform
        )
        VALUES
        (   @RecommendationId, -- RecommendationId - bigint
            @DisplayPageId,    -- DisplayPageId - varchar(10)
            'A',               -- Status - char(1)
            @ActionUser,       -- CreatedBy - varchar(200)
            GETDATE(),         -- CreatedDate - datetime
            @ActionIP,         -- CreatedIP - varchar(50)
            @ActionPlatform    -- CreatedPlatform - varchar(20)
            );

        SET @Sno = SCOPE_IDENTITY();

        IF ISNULL(@HostId, '') != ''
        BEGIN
            IF NOT EXISTS
            (
                SELECT 'X'
                FROM dbo.tbl_host_details a WITH (NOLOCK)
                    INNER JOIN dbo.tbl_club_details b WITH (NOLOCK)
                        ON b.AgentId = a.AgentId
                           AND ISNULL(b.Status, '') = 'A'
                WHERE a.HostId = @HostId
                      AND a.AgentId = @ClubId
                      AND ISNULL(a.Status, '') = 'A'
            )
            BEGIN
                SELECT 1 Code,
                       'Invalid host details' Message;
                ROLLBACK TRANSACTION @TransactionName;
                RETURN;
            END;

            INSERT INTO dbo.tbl_host_recommendation_detail
            (
                DisplayPageId,
                RecommendationId,
                ClubId,
                HostId,
                OrderId,
                Status,
                CreatedBy,
                CreatedIP,
                CreatedPlatform,
                CreatedDate
            )
            SELECT @DisplayPageId,
                   @RecommendationId,
                   a.AgentId,
                   a.HostId,
                   NULL,
                   'A',
                   @ActionUser,
                   @ActionIP,
                   @ActionPlatform,
                   GETDATE()
            FROM dbo.tbl_host_details a WITH (NOLOCK)
            WHERE a.HostId = @HostId
                  AND a.AgentId = @ClubId
                  AND ISNULL(a.Status, '') = 'A';

            SET @Sno2 = SCOPE_IDENTITY();

            UPDATE dbo.tbl_host_recommendation_detail
            SET RecommendationHostId = @Sno2
            WHERE Sno = @Sno2;

        END;

        COMMIT TRANSACTION @TransactionName;

        SELECT 0 Code,
               'Recommendation for search page added successfully for the club' Message;
        RETURN;
    END;

    ELSE IF @Flag = 'gspac' --get search page assigned club
    BEGIN
        SELECT DISTINCT a.RecommendationId,
               c.AgentId AS ClubId,
               c.Logo AS ClubLogo,
               c.ClubName1 AS ClubName,
               'Platinum' AS ClubCategory,
               b.Sno AS DisplayId,
               d.StaticDataLabel AS DisplayPageLabel,
               CASE
                   WHEN ISNULL(b.Status, '') = 'A' THEN
                       'Approved'
                   WHEN ISNULL(b.Status, '') = 'P' THEN
                       'Pending'
                   WHEN ISNULL(b.Status, '') = 'R' THEN
                       'Rejected'
               END AS Status,
               ISNULL(FORMAT(b.CreatedDate, 'MMM dd, yyyy hh:mm:ss'), '-') AS RequestedDate,
               ISNULL(FORMAT(b.UpdatedDate, 'MMM dd, yyyy hh:mm:ss'), '-') AS UpdatedDate,
               ISNULL(f.HostName, '-') AS HostName
        FROM dbo.tbl_recommendation_detail a WITH (NOLOCK)
            INNER JOIN dbo.tbl_display_page b WITH (NOLOCK)
                ON b.RecommendationId = a.RecommendationId
                   AND ISNULL(b.Status, '') = 'A'
            INNER JOIN dbo.tbl_club_details c WITH (NOLOCK)
                ON c.AgentId = a.ClubId
                   AND ISNULL(c.Status, '') = 'A'
            LEFT JOIN dbo.tbl_static_data d WITH (NOLOCK)
                ON d.StaticDataType = 6
                   AND d.StaticDataValue = @DisplayPageId
                   AND ISNULL(d.Status, '') = 'A'
            LEFT JOIN dbo.tbl_host_recommendation_detail e WITH (NOLOCK)
                ON e.RecommendationId = a.RecommendationId
                   AND e.ClubId = c.AgentId
				   AND e.DisplayPageId = @DisplayPageId
                   AND ISNULL(e.Status, '') = 'A'
            LEFT JOIN dbo.tbl_host_details f WITH (NOLOCK)
                ON f.AgentId = c.AgentId
                   AND f.HostId = e.HostId
                   AND ISNULL(f.Status, '') = 'A'
        WHERE a.LocationId = @LocationId
              AND b.DisplayPageId = @DisplayPageId;
        RETURN;
    END;

    ELSE IF @Flag = 'dspr' --delete search page recommendation
    BEGIN
        IF NOT EXISTS
        (
            SELECT 'X'
            FROM dbo.tbl_recommendation_detail a WITH (NOLOCK)
                INNER JOIN dbo.tbl_display_page b WITH (NOLOCK)
                    ON b.RecommendationId = a.RecommendationId
                       AND ISNULL(b.Status, '') = 'A'
                       AND ISNULL(a.Status, '') = 'A'
                INNER JOIN dbo.tbl_club_details c WITH (NOLOCK)
                    ON c.AgentId = a.ClubId
                       AND ISNULL(c.Status, '') = 'A'
            WHERE a.LocationId = @LocationId
                  AND b.DisplayPageId = @DisplayPageId
                  AND c.AgentId = @ClubId
                  AND b.Sno = @DisplayId
                  AND a.RecommendationId = @RecommendationId
        )
        BEGIN
            SELECT 1 Code,
                   'Invalid request' Message;
            RETURN;
        END;

        UPDATE dbo.tbl_display_page
        SET Status = 'D',
            UpdatedBy = @ActionUser,
            UpdatedDate = GETDATE(),
            UpdatedIP = @ActionIP,
            UpdatedPlatform = @ActionPlatform
        FROM dbo.tbl_display_page a WITH (NOLOCK)
            INNER JOIN dbo.tbl_recommendation_detail b WITH (NOLOCK)
                ON b.RecommendationId = a.RecommendationId
                   AND ISNULL(b.Status, '') = 'A'
            INNER JOIN dbo.tbl_club_details c WITH (NOLOCK)
                ON c.AgentId = b.ClubId
                   AND ISNULL(c.Status, '') = 'A'
        WHERE b.LocationId = @LocationId
              AND c.AgentId = @ClubId
              AND a.DisplayPageId = @DisplayPageId
              AND a.Sno = @DisplayId
              AND b.RecommendationId = @RecommendationId
              AND ISNULL(a.Status, '') = 'A';

        UPDATE dbo.tbl_host_recommendation_detail
        SET Status = 'D',
            UpdatedBy = @ActionUser,
            UpdatedDate = GETDATE(),
            UpdatedIP = @ActionIP,
            UpdatedPlatform = @ActionPlatform
        WHERE RecommendationId = @RecommendationId
              AND DisplayPageId = @DisplayPageId
              AND ClubId = @ClubId
              AND ISNULL(Status, '') = 'A';

        SELECT 0 Code,
               'Search page recommendation removed succussfully for the given club' Message;
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
    (@ErrorDesc, 'sproc_admin_searchpage_recommendation_management_v2(Flag: ' + ISNULL(@Flag, '') + ')', 'SQL', 'SQL',
     'sproc_admin_searchpage_recommendation_management_v2', GETDATE());

    SELECT 1 Code,
           'ErrorId:' + CAST(SCOPE_IDENTITY() AS VARCHAR) Message;
    RETURN;
END CATCH;
GO


