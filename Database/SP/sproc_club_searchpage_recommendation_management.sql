ALTER PROC [dbo].[sproc_club_searchpage_recommendation_management]
    @Flag VARCHAR(10),
    @ClubId VARCHAR(10) = NULL,
    @RecommendationHoldId VARCHAR(10) = NULL,
    @ActionUser VARCHAR(200) = NULL,
    @ActionIP VARCHAR(50) = NULL,
    @ActionPlatform VARCHAR(20) = NULL,
    @HostId VARCHAR(10) = NULL
AS
DECLARE @Sno BIGINT,
        @Sno2 BIGINT,
        @StringSQL VARCHAR(MAX),
        @StringSQL2 VARCHAR(MAX),
        @TransactionName VARCHAR(200),
        @ErrorDesc VARCHAR(MAX);
DECLARE @DisplayPageId VARCHAR(10) = 2;
BEGIN TRY
    IF @Flag = 'gspac' --get search page assigned club
    BEGIN
        SELECT DISTINCT
               a.RecommendationHoldId,
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
        FROM dbo.tbl_recommendation_detail_hold a WITH (NOLOCK)
            INNER JOIN dbo.tbl_display_page_hold b WITH (NOLOCK)
                ON b.RecommendationHoldId = a.RecommendationHoldId
                   AND ISNULL(b.Status, '') <> 'D'
            INNER JOIN dbo.tbl_club_details c WITH (NOLOCK)
                ON c.AgentId = a.ClubId
                   AND ISNULL(c.Status, '') = 'A'
            LEFT JOIN dbo.tbl_static_data d WITH (NOLOCK)
                ON d.StaticDataType = 6
                   AND d.StaticDataValue = @DisplayPageId
                   AND ISNULL(d.Status, '') = 'A'
            LEFT JOIN dbo.tbl_host_recommendation_detail_hold e WITH (NOLOCK)
                ON e.RecommendationHoldId = a.RecommendationHoldId
                   AND e.ClubId = c.AgentId
                   AND e.DisplayPageId = @DisplayPageId
                   AND ISNULL(e.Status, '') = 'A'
            LEFT JOIN dbo.tbl_host_details f WITH (NOLOCK)
                ON f.AgentId = c.AgentId
                   AND f.HostId = e.HostId
                   AND ISNULL(f.Status, '') = 'A'
        WHERE a.ClubId = @ClubId
              AND b.DisplayPageId = @DisplayPageId;
        RETURN;
    END;

    ELSE IF @Flag = 'ccspr' --create club search page recommendation
    BEGIN
        IF NOT EXISTS
        (
            SELECT 'X'
            FROM dbo.tbl_club_details a WITH (NOLOCK)
            WHERE a.AgentId = @ClubId
                  AND ISNULL(a.Status, '') = 'A'
        )
        BEGIN
            SELECT 1 Code,
                   'Invalid request' Message;
            RETURN;
        END;
        SELECT @RecommendationHoldId = a.RecommendationHoldId
        FROM dbo.tbl_recommendation_detail_hold a WITH (NOLOCK)
        WHERE a.ClubId = @ClubId
              AND a.Status <> 'D';

        SET @TransactionName = 'Transaction_ccspr';

        BEGIN TRANSACTION @TransactionName;

        IF ISNULL(@RecommendationHoldId, '') = ''
        BEGIN
            INSERT INTO dbo.tbl_recommendation_detail_hold
            (
                LocationId,
                ClubId,
                Status,
                CreatedBy,
                CreatedDate,
                CreatedIP,
                CreatedPlatform
            )
            SELECT a.LocationId,   -- LocationId - bigint
                   a.AgentId,      -- ClubId - bigint
                   'A',            -- Status - char(1)
                   @ActionUser,    -- CreatedBy - varchar(200)
                   GETDATE(),      -- CreatedDate - datetime
                   @ActionIP,      -- CreatedIP - varchar(50)
                   @ActionPlatform -- CreatedPlatform - varchar(50)
            FROM dbo.tbl_club_details a WITH (NOLOCK)
            WHERE a.AgentId = @ClubId
                  AND ISNULL(a.Status, '') = 'A';

            SET @RecommendationHoldId = SCOPE_IDENTITY();

            UPDATE dbo.tbl_recommendation_detail_hold
            SET RecommendationHoldId = @RecommendationHoldId
            WHERE Sno = @RecommendationHoldId;
        END;

        INSERT INTO dbo.tbl_display_page_hold
        (
            RecommendationHoldId,
            DisplayPageId,
            Status,
            CreatedBy,
            CreatedDate,
            CreatedIP,
            CreatedPlatform
        )
        VALUES
        (   @RecommendationHoldId, -- RecommendationHoldId - bigint
            @DisplayPageId,        -- DisplayPageId - varchar(10)
            'P',                   -- Status - char(1)
            @ActionUser,           -- CreatedBy - varchar(200)
            GETDATE(),             -- CreatedDate - datetime
            @ActionIP,             -- CreatedIP - varchar(50)
            @ActionPlatform        -- CreatedPlatform - varchar(20)
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

            INSERT INTO dbo.tbl_host_recommendation_detail_hold
            (
                DisplayPageId,
                RecommendationHoldId,
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
                   @RecommendationHoldId,
                   a.AgentId,
                   a.HostId,
                   NULL,
                   'P',
                   @ActionUser,
                   @ActionIP,
                   @ActionPlatform,
                   GETDATE()
            FROM dbo.tbl_host_details a WITH (NOLOCK)
            WHERE a.HostId = @HostId
                  AND a.AgentId = @ClubId
                  AND ISNULL(a.Status, '') = 'A';

            SET @Sno2 = SCOPE_IDENTITY();

        END;

        COMMIT TRANSACTION @TransactionName;

        SELECT 0 Code,
               'Recommendation for search page requested successfully for the club' Message;
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
    (@ErrorDesc, 'sproc_club_searchpage_recommendation_management(Flag: ' + ISNULL(@Flag, '') + ')', 'SQL', 'SQL',
     'sproc_club_searchpage_recommendation_management', GETDATE());

    SELECT 1 Code,
           'ErrorId:' + CAST(SCOPE_IDENTITY() AS VARCHAR) Message;
    RETURN;
END CATCH;
GO