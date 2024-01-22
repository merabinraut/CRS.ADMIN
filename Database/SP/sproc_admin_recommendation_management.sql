USE [CRS];
GO

/****** Object:  StoredProcedure [dbo].[sproc_admin_recommendation_management]    Script Date: 29/11/2023 20:15:23 ******/
SET ANSI_NULLS ON;
GO

SET QUOTED_IDENTIFIER ON;
GO






ALTER PROC [dbo].[sproc_admin_recommendation_management]
    @Flag VARCHAR(10) = N'',
    @ClubId VARCHAR(10) = NULL,
    @GroupId VARCHAR(10) = NULL,
    @OrderId VARCHAR(10) = NULL,
    @ActionUser VARCHAR(200) = NULL,
    @ActionIP VARCHAR(50) = NULL,
    @ActionPlatform VARCHAR(20) = NULL,
    @DisplayPageId VARCHAR(512) = NULL,
    @RecommendationId VARCHAR(10) = NULL,
    @HostRecommendationCSValue VARCHAR(MAX) = NULL,
    @DisplayId VARCHAR(10) = NULL,
    @ShufflingTimeCSValue VARCHAR(MAX) = N'',
    @LocationId VARCHAR(10) = NULL
AS
DECLARE @Sno BIGINT,
        @Sno2 BIGINT,
        @StringSQL VARCHAR(MAX),
        @StringSQL2 VARCHAR(MAX),
        @TransactionName VARCHAR(200),
        @ErrorDesc VARCHAR(MAX);
BEGIN TRY
    IF @Flag = 'ccrr' -- create club recommendation request
    BEGIN
        IF EXISTS
        (
            SELECT 'X'
            FROM tbl_recommendation_detail a WITH (NOLOCK)
            WHERE a.ClubId = @ClubId
                  AND a.Status = 'A'
        )
        BEGIN
            SELECT 1 Code,
                   'Recommendation already exists for the club' Message;
            RETURN;
        END;

        SET @TransactionName = 'Function_carr';

        BEGIN TRANSACTION @TransactionName;

        -- Club side query
        INSERT INTO tbl_recommendation_detail
        (
            LocationId,
            ClubId,
            GroupId,
            OrderId,
            Status,
            CreatedBy,
            CreatedDate,
            CreatedIP,
            CreatedPlatform
        )
        VALUES
        (@LocationId, @ClubId, @GroupId, @OrderId, 'A', @ActionUser, GETDATE(), @ActionIP, @ActionPlatform);

        SET @Sno = SCOPE_IDENTITY();

        UPDATE tbl_recommendation_detail
        SET RecommendationId = @Sno
        WHERE Sno = @Sno;

        CREATE TABLE #temp_DisplayPageTable
        (
            Id VARCHAR(MAX) NULL
        );

        INSERT INTO #temp_DisplayPageTable
        (
            Id
        )
        SELECT *
        FROM STRING_SPLIT(ISNULL(@DisplayPageId, ''), ',');

        SET @StringSQL = 'INSERT INTO dbo.tbl_display_page';
        SET @StringSQL += '( RecommendationId
							,DisplayPageId
							,Status
						    ,CreatedBy						   
						    ,CreatedIP
						    ,CreatedPlatform
							,CreatedDate)';
        SET @StringSQL += ' SELECT ' + CAST(@Sno AS VARCHAR) + ',a.Id,''A'',''' + ISNULL(@ActionUser, '') + ''',''';
        SET @StringSQL += ISNULL(@ActionIP, '') + ''',''' + ISNULL(@ActionPlatform, '')
                          + ''',GETDATE() FROM #temp_DisplayPageTable a with (NOLOCK)';
        PRINT (@StringSQL);
        EXEC (@StringSQL);

        DROP TABLE #temp_DisplayPageTable;

        UPDATE dbo.tbl_display_page
        SET DisplayPageId = Sno
        WHERE RecommendationId = @Sno;

        --Host side query
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

        SET @StringSQL2 = 'INSERT INTO dbo.tbl_host_recommendation_detail';
        SET @StringSQL2 += '(RecommendationId
						   ,ClubId
						   ,HostId
						   ,OrderId
						   ,Status
						   ,CreatedBy						   
						   ,CreatedIP
						   ,CreatedPlatform
						   ,CreatedDate)';
        SET @StringSQL2 += ' SELECT ' + CAST(@Sno AS VARCHAR) + ',' + CAST(@ClubId AS VARCHAR)
                           + ',a.HostId,a.OrderId,''A'',''' + ISNULL(@ActionUser, '') + ''',''';
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

        SELECT 0 Code,
               'Recommendation assigned successfully' Message;

        COMMIT TRANSACTION @TransactionName;
        RETURN;
    END;

    ELSE IF @Flag = 'ucrr' -- update club recommendation request
    BEGIN
        IF NOT EXISTS
        (
            SELECT 'X'
            FROM tbl_recommendation_detail a WITH (NOLOCK)
            WHERE a.ClubId = @ClubId
                  AND a.RecommendationId = @RecommendationId
                  AND a.Status = 'A'
        )
        BEGIN
            SELECT 1 Code,
                   'Invalid request' Message;
            RETURN;
        END;

        SET @TransactionName = 'Function_ucrr';

        BEGIN TRANSACTION @TransactionName;

        -- Club side query
        UPDATE dbo.tbl_recommendation_detail
        SET GroupId = ISNULL(@GroupId, GroupId),
            OrderId = ISNULL(@OrderId, OrderId),
            UpdatedBy = @ActionUser,
            UpdatedDate = GETDATE(),
            UpdatedIP = @ActionIP,
            UpdatedPlatform = @ActionPlatform
        WHERE ClubId = @ClubId
              AND RecommendationId = @RecommendationId
              AND Status = 'A';

        UPDATE dbo.tbl_display_page
        SET Status = 'D',
            UpdatedBy = @ActionUser,
            UpdatedDate = GETDATE(),
            UpdatedIP = @ActionIP,
            UpdatedPlatform = @ActionPlatform
        WHERE RecommendationId = @RecommendationId
              AND ISNULL(Status, '') = 'A';

        CREATE TABLE #temp_DisplayPageTable_ucrr
        (
            Id VARCHAR(MAX) NULL
        );

        INSERT INTO #temp_DisplayPageTable_ucrr
        (
            Id
        )
        SELECT *
        FROM STRING_SPLIT(ISNULL(@DisplayPageId, ''), ',');

        SET @StringSQL = 'INSERT INTO dbo.tbl_display_page';
        SET @StringSQL += '( RecommendationId
							,DisplayPageId
							,Status
						    ,CreatedBy						   
						    ,CreatedIP
						    ,CreatedPlatform
							,CreatedDate)';
        SET @StringSQL += ' SELECT ' + CAST(@Sno AS VARCHAR) + ',a.Id,''A'',''' + ISNULL(@ActionUser, '') + ''',''';
        SET @StringSQL += ISNULL(@ActionIP, '') + ''',''' + ISNULL(@ActionPlatform, '')
                          + ''',GETDATE() FROM #temp_DisplayPageTable_ucrr a with (NOLOCK)';
        PRINT (@StringSQL);
        EXEC (@StringSQL);

        DROP TABLE #temp_DisplayPageTable_ucrr;

        UPDATE dbo.tbl_display_page
        SET DisplayPageId = Sno
        WHERE RecommendationId = @Sno;


        UPDATE dbo.tbl_host_recommendation_detail
        SET Status = 'D',
            UpdatedBy = @ActionUser,
            UpdatedDate = GETDATE(),
            UpdatedIP = @ActionIP,
            UpdatedPlatform = @ActionPlatform
        WHERE RecommendationId = @RecommendationId
              AND ISNULL(Status, '') = 'A';
        --Host side query
        CREATE TABLE #temp_table1_ucrr
        (
            Value VARCHAR(MAX) NULL
        );

        INSERT INTO #temp_table1_ucrr
        (
            Value
        )
        SELECT *
        FROM STRING_SPLIT(ISNULL(@HostRecommendationCSValue, ''), ',');


        CREATE TABLE #temp_table2_ucrr
        (
            HostId VARCHAR(10),
            OrderId VARCHAR(10)
        );

        INSERT INTO #temp_table2_ucrr
        (
            HostId,
            OrderId
        )
        SELECT SUBSTRING(Value, 1, CHARINDEX(':', Value) - 1),
               SUBSTRING(Value, CHARINDEX(':', Value) + 1, LEN(Value) - CHARINDEX(':', Value))
        FROM #temp_table1_ucrr;

        SET @StringSQL2 = 'INSERT INTO dbo.tbl_host_recommendation_detail';
        SET @StringSQL2 += '(RecommendationId
						   ,ClubId
						   ,HostId
						   ,OrderId
						   ,Status
						   ,CreatedBy						   
						   ,CreatedIP
						   ,CreatedPlatform
						   ,CreatedDate)';
        SET @StringSQL2 += ' SELECT ' + CAST(@Sno AS VARCHAR) + ',' + CAST(@ClubId AS VARCHAR)
                           + ',a.HostId,a.OrderId,''A'',''' + ISNULL(@ActionUser, '') + ''',''';
        SET @StringSQL2 += ISNULL(@ActionIP, '') + ''',''' + ISNULL(@ActionPlatform, '')
                           + ''',GETDATE() FROM #temp_table2_ucrr a with (NOLOCK)';
        PRINT (@StringSQL2);
        EXEC (@StringSQL2);

        DROP TABLE #temp_table1_ucrr;
        DROP TABLE #temp_table2_ucrr;

        UPDATE tbl_host_recommendation_detail
        SET RecommendationHostId = Sno
        WHERE RecommendationId = @Sno
              AND ClubId = @ClubId;

        SELECT 0 Code,
               'Recommendation assigned successfully' Message;

        COMMIT TRANSACTION @TransactionName;
        RETURN;
    END;

    ELSE IF @Flag = 'grds1' --get recommendation details 1
    BEGIN
        IF NOT EXISTS
        (
            SELECT 'X'
            FROM dbo.tbl_recommendation_detail a WITH (NOLOCK)
            WHERE a.LocationId = @LocationId
                  AND a.ClubId = @ClubId
                  AND a.RecommendationId = @RecommendationId
                  AND ISNULL(a.Status, '') = 'A'
        )
        BEGIN
            SELECT 1 Code,
                   'Invalid request' Message;
            RETURN;
        END;

        SELECT a.RecommendationId,
               a.LocationId,
               a.ClubId,
               a.GroupId,
               a.OrderId
        FROM dbo.tbl_recommendation_detail a WITH (NOLOCK)
        WHERE a.LocationId = @LocationId
              AND a.ClubId = @ClubId
              AND a.RecommendationId = @RecommendationId
              AND ISNULL(a.Status, '') = 'A';
        RETURN;
    END;

    ELSE IF @Flag = 'gdpd2' --get display page detail 2
    BEGIN
        SELECT a.RecommendationId,
               a.DisplayPageId
        FROM dbo.tbl_display_page a WITH (NOLOCK)
        WHERE a.RecommendationId = @RecommendationId
              AND ISNULL(a.Status, '') = 'A';
        RETURN;
    END;

    ELSE IF @Flag = 'ghrd3' --get host recommendation detail 3
    BEGIN
        SELECT a.RecommendationId,
               a.RecommendationHostId,
               a.ClubId,
               a.HostId,
               a.OrderId
        FROM dbo.tbl_host_recommendation_detail a WITH (NOLOCK)
        WHERE a.RecommendationId = @RecommendationId
              AND ISNULL(a.Status, '') = 'A';
        RETURN;
    END;

    ELSE IF @Flag = 'grgcl' --get recommendated group club list
    BEGIN
        SELECT DISTINCT
               a.RecommendationId AS RecommendationId,
               c.AgentId AS ClubId,
               c.ClubName1 AS ClubName,
               'Platinum' AS ClubCategory,
               ISNULL(c.Logo, '/Content/assets_new/images/demo-image.jpeg') AS ClubLogo,
               d.StaticDataLabel AS DisplayOrder,
               ISNULL(a.UpdatedDate, a.CreatedDate) AS ActionDate,
               (STUFF(
                (
                    SELECT ',' + CONVERT(VARCHAR(MAX), ISNULL(f.StaticDataLabel, e.DisplayPageId))
                    FROM dbo.tbl_display_page e WITH (NOLOCK)
                        LEFT JOIN tbl_static_data f
                            ON f.StaticDataType = 5
                               AND f.StaticDataValue = e.DisplayPageId
                               AND f.Status = 'A'
                    WHERE e.RecommendationId = a.RecommendationId
                          AND e.Status = 'A'
                    FOR XML PATH('')
                ),
                1,
                1,
                ''
                     )
               ) AS DisplayPage,
               a.LocationId AS LocationId
        FROM dbo.tbl_recommendation_detail a WITH (NOLOCK)
            INNER JOIN dbo.tbl_display_page b WITH (NOLOCK)
                ON b.RecommendationId = a.RecommendationId
                   AND b.Status = 'A'
                   AND a.Status = 'A'
            INNER JOIN dbo.tbl_club_details c WITH (NOLOCK)
                ON c.AgentId = a.ClubId
                   AND c.Status = 'A'
            INNER JOIN dbo.tbl_static_data d WITH (NOLOCK)
                ON d.StaticDataType = 5
                   AND d.StaticDataValue = a.OrderId
                   AND a.LocationId = @LocationId;
        RETURN;
    END;

    ELSE IF @Flag = 'gstl' --get shuffling time list
    BEGIN
        SELECT a.StaticDataValue AS DisplayId,
               a.StaticDataLabel AS DisplayName,
               a.AdditionalValue1 AS DisplayTime
        FROM dbo.tbl_static_data a WITH (NOLOCK)
            INNER JOIN dbo.tbl_static_data_type b WITH (NOLOCK)
                ON b.StaticDataType = a.StaticDataType
                   AND a.StaticDataType = 5
                   AND b.Status = 'A'
                   AND a.Status = 'A'
        ORDER BY a.StaticDataValue ASC;
        RETURN;
    END;

    ELSE IF @Flag = 'mst' --manage shuffling time
    BEGIN
        CREATE TABLE #temp_mst_1
        (
            Value VARCHAR(20) NULL
        );

        INSERT INTO #temp_mst_1
        (
            Value
        )
        SELECT *
        FROM STRING_SPLIT(ISNULL(@ShufflingTimeCSValue, ''), ',');

        CREATE TABLE #temp_mst_2
        (
            StaticDataValue VARCHAR(10),
            AdditionalValue1 VARCHAR(10)
        );

        INSERT INTO #temp_mst_2
        (
            StaticDataValue,
            AdditionalValue1
        )
        SELECT SUBSTRING(Value, 1, CHARINDEX(':', Value) - 1),
               SUBSTRING(Value, CHARINDEX(':', Value) + 1, LEN(Value) - CHARINDEX(':', Value))
        FROM #temp_mst_1;

        UPDATE tbl_static_data
        SET AdditionalValue1 = a.AdditionalValue1,
            ActionUser = @ActionUser,
            ActionDate = GETDATE()
        FROM tbl_static_data
            JOIN #temp_mst_2 a WITH (NOLOCK)
                ON tbl_static_data.StaticDataValue = a.StaticDataValue
        WHERE tbl_static_data.StaticDataType = 5
              AND tbl_static_data.Status = 'A';

        DROP TABLE #temp_mst_1;
        DROP TABLE #temp_mst_2;

        SELECT 0 Code,
               'Shuffling time updated successfully';
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
    (@ErrorDesc, 'sproc_admin_recommendation_management(Flag: ' + ISNULL(@Flag, '') + ')', 'SQL', 'SQL',
     'sproc_admin_recommendation_management', GETDATE());

    SELECT 1 Code,
           'ErrorId:' + CAST(SCOPE_IDENTITY() AS VARCHAR) Message;
    RETURN;
END CATCH;
GO


