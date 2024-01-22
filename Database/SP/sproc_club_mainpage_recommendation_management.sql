USE [CRS];
GO

/****** Object:  StoredProcedure [dbo].[sproc_club_mainpage_recommendation_management]    Script Date: 08/12/2023 09:41:18 ******/
SET ANSI_NULLS ON;
GO

SET QUOTED_IDENTIFIER ON;
GO

ALTER PROC [dbo].[sproc_club_mainpage_recommendation_management]
    @Flag VARCHAR(10),
    @ClubId VARCHAR(10) = NULL,
    @RecommendationHoldId VARCHAR(10) = NULL,
    @ActionUser VARCHAR(200) = NULL,
    @ActionIP VARCHAR(50) = NULL,
    @ActionPlatform VARCHAR(20) = NULL,
    @OrderId VARCHAR(10) = NULL,
    @HostRecommendationCSValue VARCHAR(MAX) = NULL,
    @DisplayId VARCHAR(10) = NULL,
    @HostId VARCHAR(10) = NULL,
    @RecommendationHostId VARCHAR(10) = NULL
AS
DECLARE @Sno BIGINT,
        @Sno2 BIGINT,
        @StringSQL VARCHAR(MAX),
        @StringSQL2 VARCHAR(MAX),
        @TransactionName VARCHAR(200),
        @ErrorDesc VARCHAR(MAX);
DECLARE @DisplayPageId VARCHAR(10) = 3;
BEGIN TRY
    IF @Flag = 'ghpac' --get main page assigned club
    BEGIN
        SELECT a.RecommendationHoldId,
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
               g.StaticDataLabel AS DisplayOrder
        FROM dbo.tbl_recommendation_detail_hold a WITH (NOLOCK)
            INNER JOIN dbo.tbl_display_mainpage_hold b WITH (NOLOCK)
                ON b.RecommendationHoldId = a.RecommendationHoldId
                   AND ISNULL(b.Status, '') = 'P'
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
                   AND e.OrderId IS NOT NULL
                   AND ISNULL(e.Status, '') = 'A'
            LEFT JOIN dbo.tbl_host_details f WITH (NOLOCK)
                ON f.AgentId = c.AgentId
                   AND f.HostId = e.HostId
                   AND ISNULL(f.Status, '') = 'A'
            LEFT JOIN dbo.tbl_static_data g WITH (NOLOCK)
                ON g.StaticDataType = 5
                   AND g.StaticDataValue = b.OrderId
                   AND ISNULL(g.Status, '') = 'A'
        WHERE a.ClubId = @ClubId
              AND b.DisplayPageId = @DisplayPageId;
        RETURN;
    END;

    ELSE IF @Flag = 'ccmpr' --create club main page recommendation request
    BEGIN
        --IF EXISTS
        --(
        --    SELECT 'X'
        --    FROM dbo.tbl_recommendation_detail_hold a WITH (NOLOCK)
        --        INNER JOIN dbo.tbl_display_mainpage_hold b WITH (NOLOCK)
        --            ON b.RecommendationHoldId = a.RecommendationHoldId
        --               AND a.ClubId = @ClubId
        --               AND a.LocationId = @LocationId
        --               AND b.DisplayPageId = @DisplayPageId
        --               AND ISNULL(b.Status, '') = 'A'
        --               AND ISNULL(a.Status, '') <> 'D'
        --)
        --BEGIN
        --    SELECT 1 Code,
        --           'Main page recommendation already exists for the given club' Message;
        --    RETURN;
        --END;

        IF NOT EXISTS
        (
            SELECT 'X'
            FROM dbo.tbl_club_details a WITH (NOLOCK)
            WHERE a.AgentId = @ClubId
                  AND ISNULL(a.Status, '') = 'A'
        )
        BEGIN
            SELECT 1 Code,
                   'Invalid club' Message;
            RETURN;
        END;

        SELECT @RecommendationHoldId = a.RecommendationHoldId
        FROM dbo.tbl_recommendation_detail_hold a WITH (NOLOCK)
        WHERE a.ClubId = @ClubId
              AND a.Status <> 'D';

        SET @TransactionName = 'Transaction_ccmpr';

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
                   'P',            -- Status - char(1)
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

        INSERT INTO dbo.tbl_display_mainpage_hold
        (
            RecommendationHoldId,
            OrderId,
            DisplayPageId,
            Status,
            CreatedBy,
            CreatedDate,
            CreatedIP,
            CreatedPlatform
        )
        VALUES
        (   @RecommendationHoldId, -- RecommendationHoldId - bigint
            @OrderId,              -- OrderId varchar(10)
            @DisplayPageId,        -- DisplayPageId - varchar(10)
            'P',                   -- Status - char(1)
            @ActionUser,           -- CreatedBy - varchar(200)
            GETDATE(),             -- CreatedDate - datetime
            @ActionIP,             -- CreatedIP - varchar(50)
            @ActionPlatform        -- CreatedPlatform - varchar(20)
            );

        SET @Sno = SCOPE_IDENTITY();

        IF ISNULL(@HostRecommendationCSValue, '') != ''
        BEGIN
            CREATE TABLE #temp_table1
            (
                Value VARCHAR(MAX) NULL
            );

            INSERT INTO #temp_table1
            (
                Value
            )
            SELECT *
            FROM STRING_SPLIT(ISNULL(@HostRecommendationCSValue, ''), ',');


            CREATE TABLE #temp_table2
            (
                HostId VARCHAR(10),
                OrderId VARCHAR(10)
            );

            INSERT INTO #temp_table2
            (
                HostId,
                OrderId
            )
            SELECT SUBSTRING(Value, 1, CHARINDEX(':', Value) - 1),
                   SUBSTRING(Value, CHARINDEX(':', Value) + 1, LEN(Value) - CHARINDEX(':', Value))
            FROM #temp_table1;

            IF NOT EXISTS
            (
                SELECT 'X'
                FROM dbo.tbl_club_details a WITH (NOLOCK)
                    INNER JOIN dbo.tbl_host_details b WITH (NOLOCK)
                        ON b.AgentId = a.AgentId
                           AND ISNULL(a.Status, '') = 'A'
                           AND ISNULL(b.Status, '') = 'A'
                WHERE a.AgentId = @ClubId
                      AND b.HostId NOT IN
                          (
                              SELECT t1.HostId FROM #temp_table2 t1 WITH (NOLOCK)
                          )
            )
            BEGIN
                SELECT 1 Code,
                       'Invalid host' Message;

                ROLLBACK TRANSACTION @TransactionName;
                RETURN;
            END;
            SET @StringSQL2 = 'INSERT INTO dbo.tbl_host_recommendation_detail_hold';
            SET @StringSQL2 += '(
							DisplayPageId
		                   ,RecommendationHoldId
						   ,ClubId
						   ,HostId
						   ,OrderId
						   ,Status
						   ,CreatedBy						   
						   ,CreatedIP
						   ,CreatedPlatform
						   ,CreatedDate)';
            SET @StringSQL2 += ' SELECT ' + CAST(@DisplayPageId AS VARCHAR) + ','
                               + CAST(@RecommendationHoldId AS VARCHAR) + ',' + CAST(@ClubId AS VARCHAR)
                               + ',a.HostId,a.OrderId,''P'',''' + ISNULL(@ActionUser, '') + ''',''';
            SET @StringSQL2 += ISNULL(@ActionIP, '') + ''',''' + ISNULL(@ActionPlatform, '')
                               + ''',GETDATE() FROM #temp_table2 a with (NOLOCK)';
            PRINT (@StringSQL2);
            EXEC (@StringSQL2);

            DROP TABLE #temp_table1;
            DROP TABLE #temp_table2;
        END;

        COMMIT TRANSACTION @TransactionName;

        SELECT 0 Code,
               'Recommendation for main page requested successfully for the club' Message;
        RETURN;

    END;

    ELSE IF @Flag = 'gmprd' --get main page recommendation request detail for update
    BEGIN
        IF NOT EXISTS
        (
            SELECT 'X'
            FROM dbo.tbl_recommendation_detail_hold a WITH (NOLOCK)
                INNER JOIN dbo.tbl_display_mainpage_hold b WITH (NOLOCK)
                    ON b.RecommendationHoldId = a.RecommendationHoldId
                       AND ISNULL(a.Status, '') <> 'D'
                       AND ISNULL(b.Status, '') = 'P'
            WHERE a.ClubId = @ClubId
                  AND a.RecommendationHoldId = @RecommendationHoldId
                  AND b.Sno = @DisplayId
        )
        BEGIN
            SELECT 1 Code,
                   'Invalid request' Message;
            RETURN;
        END;

        SELECT 0 Code,
               'Success' Message,
               a.LocationId,
               a.ClubId,
               b.OrderId AS DisplayOrderId,
               b.RecommendationHoldId,
               b.Sno AS DisplayId
        FROM dbo.tbl_recommendation_detail_hold a WITH (NOLOCK)
            INNER JOIN dbo.tbl_display_mainpage_hold b WITH (NOLOCK)
                ON b.RecommendationHoldId = a.RecommendationHoldId
                   AND ISNULL(a.Status, '') <> 'D'
                   AND ISNULL(b.Status, '') = 'P'
        WHERE a.ClubId = @ClubId
              AND a.RecommendationHoldId = @RecommendationHoldId
              AND b.Sno = @DisplayId;

        RETURN;
    END;

    ELSE IF @Flag = 'gmphrd' --get main page host recommendation request detail for update
    BEGIN
        SELECT a.RecommendationHoldId,
               b.Sno AS RecommendationHostHoldId,
               b.HostId,
               b.OrderId AS HostDisplayOrderId
        FROM dbo.tbl_recommendation_detail_hold a WITH (NOLOCK)
            INNER JOIN dbo.tbl_host_recommendation_detail_hold b WITH (NOLOCK)
                ON b.RecommendationHoldId = a.RecommendationHoldId
                   AND ISNULL(a.Status, '') <> 'D'
                   AND ISNULL(b.Status, '') = 'P'
            INNER JOIN dbo.tbl_display_mainpage_hold c WITH (NOLOCK)
                ON c.RecommendationHoldId = a.RecommendationHoldId
                   AND ISNULL(c.Status, '') = 'P'
        WHERE a.ClubId = @ClubId
              AND c.Sno = @DisplayId
              AND a.RecommendationHoldId = @RecommendationHoldId
              AND b.DisplayPageId = @DisplayPageId;
        RETURN;
    END;

    ELSE IF @Flag = 'ucmpr' --update club main page recommendation
    BEGIN
        IF NOT EXISTS
        (
            SELECT 'X'
            FROM dbo.tbl_recommendation_detail_hold a WITH (NOLOCK)
                INNER JOIN dbo.tbl_display_mainpage_hold b WITH (NOLOCK)
                    ON b.RecommendationHoldId = a.RecommendationHoldId
                       AND b.DisplayPageId = @DisplayPageId
                       AND ISNULL(b.Status, '') = 'P'
                       AND ISNULL(a.Status, '') <> 'D'
            WHERE b.Sno = @DisplayId
                  AND a.ClubId = @ClubId
                  AND a.RecommendationHoldId = @RecommendationHoldId
        )
        BEGIN
            SELECT 1 Code,
                   'Invalid request' Message;
            RETURN;
        END;

        IF NOT EXISTS
        (
            SELECT 'X'
            FROM dbo.tbl_club_details a WITH (NOLOCK)
            WHERE a.AgentId = @ClubId
                  AND ISNULL(a.Status, '') = 'A'
        )
        BEGIN
            SELECT 1 Code,
                   'Invalid club' Message;
            RETURN;
        END;

        SET @TransactionName = 'Transaction_ccmpr';

        BEGIN TRANSACTION @TransactionName;

        UPDATE dbo.tbl_display_mainpage_hold
        SET OrderId = ISNULL(@OrderId, a.OrderId),
            UpdatedBy = @ActionUser,
            UpdatedDate = GETDATE(),
            UpdatedIP = @ActionIP,
            UpdatedPlatform = @ActionPlatform
        FROM dbo.tbl_display_mainpage_hold a WITH (NOLOCK)
            INNER JOIN dbo.tbl_recommendation_detail_hold b WITH (NOLOCK)
                ON b.RecommendationHoldId = a.RecommendationHoldId
                   AND ISNULL(b.Status, '') <> 'D'
            INNER JOIN dbo.tbl_club_details c WITH (NOLOCK)
                ON c.AgentId = b.ClubId
                   AND ISNULL(c.Status, '') = 'A'
        WHERE c.AgentId = @ClubId
              AND a.DisplayPageId = @DisplayPageId
              AND a.Sno = @DisplayId;

        UPDATE dbo.tbl_host_recommendation_detail_hold
        SET Status = 'D',
            UpdatedBy = @ActionUser,
            UpdatedDate = GETDATE(),
            UpdatedIP = @ActionIP,
            UpdatedPlatform = @ActionPlatform
        WHERE RecommendationHoldId = @RecommendationHoldId
              AND DisplayPageId = @DisplayPageId
              AND ISNULL(Status, '') = 'P';

        IF ISNULL(@HostRecommendationCSValue, '') != ''
        BEGIN
            CREATE TABLE #temp_table1_ucmpr
            (
                Value VARCHAR(MAX) NULL
            );

            INSERT INTO #temp_table1_ucmpr
            (
                Value
            )
            SELECT *
            FROM STRING_SPLIT(ISNULL(@HostRecommendationCSValue, ''), ',');


            CREATE TABLE #temp_table2_ucmpr
            (
                HostId VARCHAR(10),
                OrderId VARCHAR(10)
            );

            INSERT INTO #temp_table2_ucmpr
            (
                HostId,
                OrderId
            )
            SELECT SUBSTRING(Value, 1, CHARINDEX(':', Value) - 1),
                   SUBSTRING(Value, CHARINDEX(':', Value) + 1, LEN(Value) - CHARINDEX(':', Value))
            FROM #temp_table1_ucmpr;

            IF NOT EXISTS
            (
                SELECT 'X'
                FROM dbo.tbl_club_details a WITH (NOLOCK)
                    INNER JOIN dbo.tbl_host_details b WITH (NOLOCK)
                        ON b.AgentId = a.AgentId
                           AND ISNULL(a.Status, '') = 'A'
                           AND ISNULL(b.Status, '') = 'A'
                WHERE a.AgentId = @ClubId
                      AND b.HostId NOT IN
                          (
                              SELECT t1.HostId FROM #temp_table2_ucmpr t1 WITH (NOLOCK)
                          )
            )
            BEGIN
                SELECT 1 Code,
                       'Invalid host' Message;

                ROLLBACK TRANSACTION @TransactionName;
                RETURN;
            END;

            SET @StringSQL2 = 'INSERT INTO dbo.tbl_host_recommendation_detail_hold';
            SET @StringSQL2 += '(
							DisplayPageId
		                   ,RecommendationHoldId
						   ,ClubId
						   ,HostId
						   ,OrderId
						   ,Status
						   ,CreatedBy						   
						   ,CreatedIP
						   ,CreatedPlatform
						   ,CreatedDate)';
            SET @StringSQL2 += ' SELECT ' + CAST(@DisplayPageId AS VARCHAR) + ','
                               + CAST(@RecommendationHoldId AS VARCHAR) + ',' + CAST(@ClubId AS VARCHAR)
                               + ',a.HostId,a.OrderId,''P'',''' + ISNULL(@ActionUser, '') + ''',''';
            SET @StringSQL2 += ISNULL(@ActionIP, '') + ''',''' + ISNULL(@ActionPlatform, '')
                               + ''',GETDATE() FROM #temp_table2_ucmpr a with (NOLOCK)';
            PRINT (@StringSQL2);
            EXEC (@StringSQL2);

            DROP TABLE #temp_table1_ucmpr;
            DROP TABLE #temp_table2_ucmpr;
        END;

        COMMIT TRANSACTION @TransactionName;

        SELECT 0 Code,
               'Recommendation request for main page updated successfully for the club' Message;
        RETURN;
    END;

    ELSE IF @Flag = 'gmprh' --get main page recommended hosts
    BEGIN
        SELECT a.RecommendationHoldId,
               d.HostId,
               c.Sno AS RecommendationHostHoldId,
               a.ClubId,
               d.HostName,
               e.StaticDataLabel AS DisplayOrder,
               ISNULL(
               (
                   SELECT TOP 1
                          a2.ImagePath
                   FROM dbo.tbl_gallery a2 WITH (NOLOCK)
                   WHERE a2.AgentId = d.HostId
                         AND a2.RoleId = 5
                         AND ISNULL(a2.Status, '') = 'A'
                   ORDER BY a2.Sno DESC
               ),
               ''
                     ) AS HostImage,
               ISNULL(FORMAT(c.CreatedDate, 'MMM dd, yyyy hh:mm:ss'), '-') AS CreatedOn,
               ISNULL(FORMAT(c.UpdatedDate, 'MMM dd, yyyy hh:mm:ss'), '-') AS UpdatedOn,
               CASE
                   WHEN ISNULL(c.Status, '') = 'A' THEN
                       'Approved'
                   WHEN ISNULL(c.Status, '') = 'P' THEN
                       'Pending'
                   WHEN ISNULL(c.Status, '') = 'R' THEN
                       'Rejected'
                   ELSE
                       '-'
               END AS Status
        FROM dbo.tbl_recommendation_detail_hold a WITH (NOLOCK)
            INNER JOIN dbo.tbl_display_mainpage_hold b WITH (NOLOCK)
                ON b.RecommendationHoldId = a.RecommendationHoldId
                   AND b.DisplayPageId = @DisplayPageId
                   AND ISNULL(b.Status, '') <> 'D'
                   AND ISNULL(a.Status, '') <> 'D'
            INNER JOIN dbo.tbl_host_recommendation_detail_hold c WITH (NOLOCK)
                ON c.RecommendationHoldId = a.RecommendationHoldId
                   AND c.DisplayPageId = @DisplayPageId
                   AND c.ClubId = a.ClubId
                   AND ISNULL(c.Status, '') <> 'D'
            INNER JOIN dbo.tbl_host_details d WITH (NOLOCK)
                ON d.HostId = c.HostId
                   AND ISNULL(d.Status, '') = 'A'
            LEFT JOIN dbo.tbl_static_data e WITH (NOLOCK)
                ON e.StaticDataType = 5
                   AND e.StaticDataValue = b.OrderId
                   AND ISNULL(e.Status, '') = 'A'
        WHERE a.RecommendationHoldId = @RecommendationHoldId
              AND b.Sno = @DisplayId
              AND a.ClubId = @ClubId;
        RETURN;
    END;

    ELSE IF @Flag = 'rmprh' --remove main page recommendate host
    BEGIN
        IF NOT EXISTS
        (
            SELECT 'X'
            FROM dbo.tbl_recommendation_detail_hold a WITH (NOLOCK)
                INNER JOIN dbo.tbl_host_recommendation_detail_hold b WITH (NOLOCK)
                    ON b.RecommendationHoldId = a.RecommendationHoldId
                       AND b.DisplayPageId = @DisplayPageId
                       AND b.ClubId = a.ClubId
                       AND ISNULL(a.Status, '') <> 'D'
            WHERE a.RecommendationHoldId = @RecommendationHoldId
                  AND b.HostId = @HostId
                  AND b.Sno = @RecommendationHostId
                  AND a.ClubId = @ClubId
                  AND ISNULL(b.Status, '') = 'P'
        )
        BEGIN
            SELECT 1 Code,
                   'Invalid request' Message;
            RETURN;
        END;

        UPDATE dbo.tbl_host_recommendation_detail_hold
        SET Status = 'D',
            UpdatedBy = @ActionUser,
            UpdatedDate = GETDATE(),
            UpdatedPlatform = @ActionPlatform,
            UpdatedIP = @ActionIP
        FROM dbo.tbl_host_recommendation_detail_hold a WITH (NOLOCK)
            INNER JOIN dbo.tbl_recommendation_detail_hold b WITH (NOLOCK)
                ON b.RecommendationHoldId = a.RecommendationHoldId
                   AND a.DisplayPageId = @DisplayPageId
                   AND b.ClubId = a.ClubId
                   AND ISNULL(b.Status, '') <> 'D'
        WHERE b.RecommendationHoldId = @RecommendationHoldId
              AND a.HostId = @HostId
              AND a.Sno = @RecommendationHostId
              AND a.ClubId = @ClubId
              AND ISNULL(a.Status, '') = 'P';

        SELECT 0 Code,
               'Host removed successfully from recommendation request' Message;
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
    (@ErrorDesc, 'sproc_club_mainpage_recommendation_management(Flag: ' + ISNULL(@Flag, '') + ')', 'SQL', 'SQL',
     'sproc_club_mainpage_recommendation_management', GETDATE());

    SELECT 1 Code,
           'ErrorId:' + CAST(SCOPE_IDENTITY() AS VARCHAR) Message;
    RETURN;
END CATCH;
GO


