USE [CRS];
GO

/****** Object:  StoredProcedure [dbo].[sproc_admin_mainpage_recommendation_management_v2]    Script Date: 06/12/2023 13:23:49 ******/
SET ANSI_NULLS ON;
GO

SET QUOTED_IDENTIFIER ON;
GO





ALTER PROC [dbo].[sproc_admin_mainpage_recommendation_management_v2]
    @Flag VARCHAR(10),
    @RecommendationId VARCHAR(10) = NULL,
    @ClubId VARCHAR(10) = NULL,
    @LocationId VARCHAR(10) = NULL,
    @ActionUser VARCHAR(200) = NULL,
    @ActionIP VARCHAR(50) = NULL,
    @ActionPlatform VARCHAR(20) = NULL,
    @DisplayId VARCHAR(10) = NULL,
    @GroupId VARCHAR(10) = NULL,
    @OrderId VARCHAR(10) = NULL,
    @HostRecommendationCSValue VARCHAR(MAX) = NULL,
    @RecommendationHostId VARCHAR(10) = NULL,
    @HostId VARCHAR(10) = NULL
AS
DECLARE @Sno BIGINT,
        @Sno2 BIGINT,
        @StringSQL VARCHAR(MAX),
        @StringSQL2 VARCHAR(MAX),
        @TransactionName VARCHAR(200),
        @ErrorDesc VARCHAR(MAX);
DECLARE @MaxNoOfClub INT = 10,
        @DisplayPageId VARCHAR(10) = 3;
BEGIN TRY
    IF @Flag = 'ccmpr' --create club main page recommendation
    BEGIN
        IF EXISTS
        (
            SELECT 'X'
            FROM dbo.tbl_recommendation_detail a WITH (NOLOCK)
                INNER JOIN dbo.tbl_display_mainpage b WITH (NOLOCK)
                    ON b.RecommendationId = a.RecommendationId
                       AND a.ClubId = @ClubId
                       AND a.LocationId = @LocationId
                       AND b.DisplayPageId = @DisplayPageId
                       AND ISNULL(b.Status, '') = 'A'
                       AND b.GroupId = @GroupId
                       AND ISNULL(a.Status, '') = 'A'
        )
        BEGIN
            SELECT 1 Code,
                   'Main page recommendation already exists for the given club' Message;
            RETURN;
        END;

        IF
        (
            SELECT COUNT(b.Sno)
            FROM dbo.tbl_recommendation_detail a WITH (NOLOCK)
                INNER JOIN dbo.tbl_display_mainpage b WITH (NOLOCK)
                    ON b.RecommendationId = a.RecommendationId
                       AND a.LocationId = @LocationId
                       AND b.DisplayPageId = @DisplayPageId
                       AND ISNULL(b.Status, '') = 'A'
                       AND b.GroupId = @GroupId
        ) >= @MaxNoOfClub
        BEGIN
            SELECT 1 Code,
                   'Main page recommendation already consist of maximum number of club' Message;
            RETURN;
        END;

        IF NOT EXISTS
        (
            SELECT 'X'
            FROM dbo.tbl_club_details a WITH (NOLOCK)
            WHERE a.AgentId = @ClubId
                  AND a.LocationId = @LocationId
                  AND ISNULL(a.Status, '') = 'A'
        )
        BEGIN
            SELECT 1 Code,
                   'Invalid club' Message;
            RETURN;
        END;

        SELECT @RecommendationId = a.RecommendationId
        FROM dbo.tbl_recommendation_detail a WITH (NOLOCK)
        WHERE a.LocationId = @LocationId
              AND a.ClubId = @ClubId
              AND a.Status = 'A';

        SET @TransactionName = 'Transaction_ccmpr';

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

        INSERT INTO dbo.tbl_display_mainpage
        (
            RecommendationId,
            GroupId,
            OrderId,
            DisplayPageId,
            Status,
            CreatedBy,
            CreatedDate,
            CreatedIP,
            CreatedPlatform
        )
        VALUES
        (   @RecommendationId, -- RecommendationId - bigint
            @GroupId,          -- GroupId varchar(10)
            @OrderId,          -- OrderId varchar(10)
            @DisplayPageId,    -- DisplayPageId - varchar(10)
            'A',               -- Status - char(1)
            @ActionUser,       -- CreatedBy - varchar(200)
            GETDATE(),         -- CreatedDate - datetime
            @ActionIP,         -- CreatedIP - varchar(50)
            @ActionPlatform    -- CreatedPlatform - varchar(20)
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
            SET @StringSQL2 = 'INSERT INTO dbo.tbl_host_recommendation_detail';
            SET @StringSQL2 += '(
							DisplayPageId
		                   ,RecommendationId
						   ,ClubId
						   ,HostId
						   ,OrderId
						   ,Status
						   ,CreatedBy						   
						   ,CreatedIP
						   ,CreatedPlatform
						   ,CreatedDate)';
            SET @StringSQL2 += ' SELECT ' + CAST(@DisplayPageId AS VARCHAR) + ',' + CAST(@Sno AS VARCHAR) + ','
                               + CAST(@ClubId AS VARCHAR) + ',a.HostId,a.OrderId,''A'',''' + ISNULL(@ActionUser, '')
                               + ''',''';
            SET @StringSQL2 += ISNULL(@ActionIP, '') + ''',''' + ISNULL(@ActionPlatform, '')
                               + ''',GETDATE() FROM #temp_table2 a with (NOLOCK)';
            PRINT (@StringSQL2);
            EXEC (@StringSQL2);

            DROP TABLE #temp_table1;
            DROP TABLE #temp_table2;

            UPDATE tbl_host_recommendation_detail
            SET RecommendationHostId = Sno
            WHERE RecommendationId = @Sno
                  AND ClubId = @ClubId;
        END;

        COMMIT TRANSACTION @TransactionName;

        SELECT 0 Code,
               'Recommendation for main page added successfully for the club' Message;
        RETURN;

    END;

    ELSE IF @Flag = 'gmprd' --get main page recommendation detail for update
    BEGIN
        IF NOT EXISTS
        (
            SELECT 'X'
            FROM dbo.tbl_recommendation_detail a WITH (NOLOCK)
                INNER JOIN dbo.tbl_display_mainpage b WITH (NOLOCK)
                    ON b.RecommendationId = a.RecommendationId
                       AND ISNULL(a.Status, '') = 'A'
                       AND ISNULL(b.Status, '') = 'A'
            WHERE a.ClubId = @ClubId
                  AND a.RecommendationId = @RecommendationId
                  AND b.GroupId = @GroupId
                  AND a.LocationId = @LocationId
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
               b.GroupId,
               a.ClubId,
               b.OrderId AS DisplayOrderId,
               b.RecommendationId,
               b.Sno AS DisplayId
        FROM dbo.tbl_recommendation_detail a WITH (NOLOCK)
            INNER JOIN dbo.tbl_display_mainpage b WITH (NOLOCK)
                ON b.RecommendationId = a.RecommendationId
                   AND ISNULL(a.Status, '') = 'A'
                   AND ISNULL(b.Status, '') = 'A'
        WHERE a.ClubId = @ClubId
              AND a.RecommendationId = @RecommendationId
              AND b.GroupId = @GroupId
              AND a.LocationId = @LocationId
              AND b.Sno = @DisplayId;
        RETURN;
    END;

    ELSE IF @Flag = 'gmphrd' --get main page host recommendation detail for update
    BEGIN
        SELECT a.RecommendationId,
               b.RecommendationHostId,
               b.HostId,
               b.OrderId AS HostDisplayOrderId
        FROM dbo.tbl_recommendation_detail a WITH (NOLOCK)
            INNER JOIN dbo.tbl_host_recommendation_detail b WITH (NOLOCK)
                ON b.RecommendationId = a.RecommendationId
                   AND ISNULL(a.Status, '') = 'A'
                   AND ISNULL(b.Status, '') = 'A'
            INNER JOIN dbo.tbl_display_mainpage c WITH (NOLOCK)
                ON c.RecommendationId = a.RecommendationId
                   AND ISNULL(c.Status, '') = 'A'
                   AND c.GroupId = @GroupId
        WHERE a.ClubId = @ClubId
              AND a.LocationId = @LocationId
              AND a.RecommendationId = @RecommendationId
              AND b.DisplayPageId = @DisplayPageId;
        RETURN;
    END;

    ELSE IF @Flag = 'ucmpr' --update club main page recommendation
    BEGIN
        IF NOT EXISTS
        (
            SELECT 'X'
            FROM dbo.tbl_recommendation_detail a WITH (NOLOCK)
                INNER JOIN dbo.tbl_display_mainpage b WITH (NOLOCK)
                    ON b.RecommendationId = a.RecommendationId
                       AND a.ClubId = @ClubId
                       AND a.LocationId = @LocationId
                       AND b.DisplayPageId = @DisplayPageId
                       AND ISNULL(b.Status, '') = 'A'
                       AND b.GroupId = @GroupId
                       AND ISNULL(a.Status, '') = 'A'
            WHERE b.Sno = @DisplayId
                  AND a.RecommendationId = @RecommendationId
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
                  AND a.LocationId = @LocationId
                  AND ISNULL(a.Status, '') = 'A'
        )
        BEGIN
            SELECT 1 Code,
                   'Invalid club' Message;
            RETURN;
        END;

        SET @TransactionName = 'Transaction_ccmpr';

        BEGIN TRANSACTION @TransactionName;

        UPDATE dbo.tbl_display_mainpage
        SET OrderId = ISNULL(@OrderId, a.OrderId),
            UpdatedBy = @ActionUser,
            UpdatedDate = GETDATE(),
            UpdatedIP = @ActionIP,
            UpdatedPlatform = @ActionPlatform
        FROM dbo.tbl_display_mainpage a WITH (NOLOCK)
            INNER JOIN dbo.tbl_recommendation_detail b WITH (NOLOCK)
                ON b.RecommendationId = a.RecommendationId
                   AND ISNULL(b.Status, '') = 'A'
            INNER JOIN dbo.tbl_club_details c WITH (NOLOCK)
                ON c.AgentId = b.ClubId
                   AND ISNULL(c.Status, '') = 'A'
        WHERE b.LocationId = @LocationId
              AND c.AgentId = @ClubId
              AND a.DisplayPageId = @DisplayPageId
              AND a.GroupId = @GroupId
              AND a.Sno = @DisplayId;

        UPDATE dbo.tbl_host_recommendation_detail
        SET Status = 'D',
            UpdatedBy = @ActionUser,
            UpdatedDate = GETDATE(),
            UpdatedIP = @ActionIP,
            UpdatedPlatform = @ActionPlatform
        WHERE RecommendationId = @RecommendationId
              AND DisplayPageId = @DisplayPageId
              AND ISNULL(Status, '') = 'A';

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

            SET @StringSQL2 = 'INSERT INTO dbo.tbl_host_recommendation_detail';
            SET @StringSQL2 += '(
							DisplayPageId
		                   ,RecommendationId
						   ,ClubId
						   ,HostId
						   ,OrderId
						   ,Status
						   ,CreatedBy						   
						   ,CreatedIP
						   ,CreatedPlatform
						   ,CreatedDate)';
            SET @StringSQL2 += ' SELECT ' + CAST(@DisplayPageId AS VARCHAR) + ',' + CAST(@RecommendationId AS VARCHAR)
                               + ',' + CAST(@ClubId AS VARCHAR) + ',a.HostId,a.OrderId,''A'','''
                               + ISNULL(@ActionUser, '') + ''',''';
            SET @StringSQL2 += ISNULL(@ActionIP, '') + ''',''' + ISNULL(@ActionPlatform, '')
                               + ''',GETDATE() FROM #temp_table2_ucmpr a with (NOLOCK)';
            PRINT (@StringSQL2);
            EXEC (@StringSQL2);

            DROP TABLE #temp_table1_ucmpr;
            DROP TABLE #temp_table2_ucmpr;

            UPDATE tbl_host_recommendation_detail
            SET RecommendationHostId = Sno
            WHERE RecommendationId = @RecommendationId
                  AND ClubId = @ClubId
                  AND DisplayPageId = @DisplayPageId
                  AND ISNULL(Status, '') = 'A';
        END;

        COMMIT TRANSACTION @TransactionName;

        SELECT 0 Code,
               'Recommendation for main page updated successfully for the club' Message;
        RETURN;
    END;

    ELSE IF @Flag = 'dmpr' --delete main page recommendation
    BEGIN
        IF NOT EXISTS
        (
            SELECT 'X'
            FROM dbo.tbl_recommendation_detail a WITH (NOLOCK)
                INNER JOIN dbo.tbl_display_mainpage b WITH (NOLOCK)
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
                  AND b.GroupId = @GroupId
        )
        BEGIN
            SELECT 1 Code,
                   'Invalid request' Message;
            RETURN;
        END;

        UPDATE dbo.tbl_display_mainpage
        SET Status = 'D',
            UpdatedBy = @ActionUser,
            UpdatedDate = GETDATE(),
            UpdatedIP = @ActionIP,
            UpdatedPlatform = @ActionPlatform
        FROM dbo.tbl_display_mainpage a WITH (NOLOCK)
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
              AND a.GroupId = @GroupId
              AND ISNULL(a.Status, '') = 'A';


        UPDATE dbo.tbl_host_recommendation_detail
        SET Status = 'D',
            UpdatedBy = @ActionUser,
            UpdatedDate = GETDATE(),
            UpdatedIP = @ActionIP,
            UpdatedPlatform = @ActionPlatform
        FROM dbo.tbl_host_recommendation_detail a WITH (NOLOCK)
            INNER JOIN dbo.tbl_recommendation_detail b WITH (NOLOCK)
                ON b.RecommendationId = a.RecommendationId
        WHERE b.RecommendationId = @RecommendationId
              AND b.LocationId = @LocationId
              AND a.DisplayPageId = @DisplayPageId
              AND b.ClubId = @ClubId
              AND ISNULL(a.Status, '') = 'A'
              AND ISNULL(b.Status, '') = 'A';
    END;

    ELSE IF @Flag = 'ghpac' --get home page assigned club
    BEGIN
        SELECT a.RecommendationId,
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
        FROM dbo.tbl_recommendation_detail a WITH (NOLOCK)
            INNER JOIN dbo.tbl_display_mainpage b WITH (NOLOCK)
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
        WHERE a.LocationId = @LocationId
              AND b.DisplayPageId = @DisplayPageId
              AND b.GroupId = @GroupId;
        RETURN;
    END;

    ELSE IF @Flag = 'rmprh' --remove main page recommendate host
    BEGIN
        IF NOT EXISTS
        (
            SELECT 'X'
            FROM dbo.tbl_recommendation_detail a WITH (NOLOCK)
                INNER JOIN dbo.tbl_host_recommendation_detail b WITH (NOLOCK)
                    ON b.RecommendationId = a.RecommendationId
                       AND b.DisplayPageId = @DisplayPageId
                       AND b.ClubId = a.ClubId
                       AND ISNULL(a.Status, '') = 'A'
            WHERE a.RecommendationId = @RecommendationId
                  AND b.HostId = @HostId
                  AND b.RecommendationHostId = @RecommendationHostId
                  AND a.ClubId = @ClubId
                  AND ISNULL(b.Status, '') = 'A'
        )
        BEGIN
            SELECT 1 Code,
                   'Invalid request' Message;
            RETURN;
        END;

        UPDATE dbo.tbl_host_recommendation_detail
        SET Status = 'D',
            UpdatedBy = @ActionUser,
            UpdatedDate = GETDATE(),
            UpdatedPlatform = @ActionPlatform,
            UpdatedIP = @ActionIP
        FROM dbo.tbl_recommendation_detail a WITH (NOLOCK)
            INNER JOIN dbo.tbl_host_recommendation_detail b WITH (NOLOCK)
                ON b.RecommendationId = a.RecommendationId
                   AND b.DisplayPageId = @DisplayPageId
                   AND b.ClubId = a.ClubId
                   AND ISNULL(a.Status, '') = 'A'
        WHERE a.RecommendationId = @RecommendationId
              AND b.HostId = @HostId
              AND b.RecommendationHostId = @RecommendationHostId
              AND a.ClubId = @ClubId
              AND ISNULL(b.Status, '') = 'A';

        SELECT 0 Code,
               'Host removed successfully from recommendation' Message;
        RETURN;
    END;

    ELSE IF @Flag = 'gmprh' --get main page recommended hosts
    BEGIN
        SELECT a.RecommendationId,
               d.HostId,
               c.RecommendationHostId,
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
               ISNULL(FORMAT(c.UpdatedDate, 'MMM dd, yyyy hh:mm:ss'), '-') AS UpdatedOn
        FROM dbo.tbl_recommendation_detail a WITH (NOLOCK)
            INNER JOIN dbo.tbl_display_mainpage b WITH (NOLOCK)
                ON b.RecommendationId = a.RecommendationId
                   AND b.DisplayPageId = @DisplayPageId
                   AND ISNULL(b.Status, '') = 'A'
                   AND ISNULL(a.Status, '') = 'A'
            INNER JOIN dbo.tbl_host_recommendation_detail c WITH (NOLOCK)
                ON c.RecommendationId = a.RecommendationId
                   AND c.DisplayPageId = @DisplayPageId
                   AND c.ClubId = a.ClubId
                   AND ISNULL(c.Status, '') = 'A'
            INNER JOIN dbo.tbl_host_details d WITH (NOLOCK)
                ON d.HostId = c.HostId
                   AND ISNULL(d.Status, '') = 'A'
            LEFT JOIN dbo.tbl_static_data e WITH (NOLOCK)
                ON e.StaticDataType = 5
                   AND e.StaticDataValue = b.OrderId
                   AND ISNULL(e.Status, '') = 'A'
        WHERE a.RecommendationId = @RecommendationId
              AND b.GroupId = @GroupId
              AND a.ClubId = @ClubId
              AND a.LocationId = @LocationId;
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
    (@ErrorDesc, 'sproc_admin_mainpage_recommendation_management_v2(Flag: ' + ISNULL(@Flag, '') + ')', 'SQL', 'SQL',
     'sproc_admin_mainpage_recommendation_management_v2', GETDATE());

    SELECT 1 Code,
           'ErrorId:' + CAST(SCOPE_IDENTITY() AS VARCHAR) Message;
    RETURN;
END CATCH;
GO


