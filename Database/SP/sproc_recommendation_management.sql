USE [CRS];
GO

/****** Object:  StoredProcedure [dbo].[sproc_recommendation_management]    Script Date: 18/10/2023 13:25:46 ******/
SET ANSI_NULLS ON;
GO

SET QUOTED_IDENTIFIER ON;
GO


ALTER PROC [dbo].[sproc_recommendation_management]
    @Flag VARCHAR(10),
    @HoldId VARCHAR(20) = NULL,
    @ActionUser VARCHAR(200) = NULL,
    @ActionIP VARCHAR(50) = NULL,
    @ActionPlatform VARCHAR(20) = NULL,
    @RecommendationId VARCHAR(20) = NULL,
    @OrderId VARCHAR(20) = NULL,
    @GroupId VARCHAR(20) = NULL,
    @DisplayPageId VARCHAR(512) = NULL
AS
DECLARE @Sno VARCHAR(20),
        @StringSQL VARCHAR(MAX),
        @TransactionName VARCHAR(200),
        @ErrorDesc VARCHAR(MAX);
BEGIN TRY
    IF ISNULL(@Flag, '') = 'rl' --recommendation list
    BEGIN
        SELECT a.RecommendationId,
               b.StaticDataLabel AS GroupName,
               1 AS TotalClub,
               a.CreatedBy,
               a.CreatedDate,
               a.CreatedIP,
               a.CreatedPlatform,
               a.UpdatedBy,
               a.UpdatedDate,
               a.UpdatedIP,
               a.UpdatedPlatform
        FROM tbl_recommendation_detail a WITH (NOLOCK)
            INNER JOIN dbo.tbl_static_data b WITH (NOLOCK)
                ON b.StaticDataValue = a.GroupId
                   AND b.StaticDataValue = 4
        WHERE ISNULL(a.Status, '') = 'A';
        RETURN;
    END;
    ELSE IF ISNULL(@Flag, '') = 'arr' --accept recommendation request
    BEGIN
        IF NOT EXISTS
        (
            SELECT 'X'
            FROM dbo.tbl_admin a WITH (NOLOCK)
            WHERE a.ActionUser = @ActionUser
                  AND ISNULL(a.Status, '') = 'A'
        )
        BEGIN
            SELECT 1 Code,
                   'Invalid action user' Message;
            RETURN;
        END;

        IF NOT EXISTS
        (
            SELECT 'X'
            FROM dbo.tbl_recommendation_detail_hold a WITH (NOLOCK)
                INNER JOIN tbl_display_page_hold b WITH (NOLOCK)
                    ON b.HoldId = a.Sno
                       AND ISNULL(b.[Status], '') = 'P'
            WHERE a.Sno = @HoldId
                  AND ISNULL(a.[Status], '') = 'P'
        )
        BEGIN
            SELECT 1 Code,
                   'Invalid details' Message;
            RETURN;
        END;

        SET @TransactionName = 'Function_arr';

        BEGIN TRANSACTION @TransactionName;

        INSERT INTO tbl_recommendation_detail
        (
            AgentId,
            AgentType,
            GroupId,
            OrderId,
            [Status],
            CreatedBy,
            CreatedDate,
            CreatedIP,
            CreatedPlatform
        )
        SELECT a.AgentId,
               a.AgentType,
               a.GroupId,
               a.OrderId,
               'A',
               @ActionUser,
               GETDATE(),
               @ActionIP,
               @ActionPlatform
        FROM dbo.tbl_recommendation_detail_hold a WITH (NOLOCK)
        WHERE a.Sno = @HoldId
              AND ISNULL(a.[Status], '') = 'P';

        SET @Sno = SCOPE_IDENTITY();

        UPDATE tbl_recommendation_detail
        SET RecommendationId = @Sno
        WHERE Sno = @Sno;

        INSERT INTO tbl_display_page
        (
            RecommendationId,
            DisplayPageId,
            Status,
            CreatedBy,
            CreatedDate,
            CreatedIP,
            CreatedPlatform
        )
        SELECT @Sno,
               a.DisplayPageId,
               'A',
               @ActionUser,
               GETDATE(),
               @ActionIP,
               'Admin'
        FROM tbl_display_page_hold a WITH (NOLOCK)
        WHERE a.HoldId = @HoldId
              AND ISNULL(a.[Status], '') = 'P';

        SELECT 0 Code,
               'Recommendation request approved successfully' Message;

        COMMIT TRANSACTION @TransactionName;
        RETURN;
    END;
    ELSE IF ISNULL(@Flag, '') = 'rrr' --reject recommendation request
    BEGIN
        IF NOT EXISTS
        (
            SELECT 'X'
            FROM dbo.tbl_admin a WITH (NOLOCK)
            WHERE a.ActionUser = @ActionUser
                  AND ISNULL(a.Status, '') = 'A'
        )
        BEGIN
            SELECT 1 Code,
                   'Invalid action user' Message;
            RETURN;
        END;

        IF NOT EXISTS
        (
            SELECT 'X'
            FROM dbo.tbl_recommendation_detail_hold a WITH (NOLOCK)
                INNER JOIN tbl_display_page_hold b WITH (NOLOCK)
                    ON b.HoldId = a.Sno
                       AND ISNULL(b.[Status], '') = 'P'
            WHERE a.Sno = @HoldId
                  AND ISNULL(a.[Status], '') = 'P'
        )
        BEGIN
            SELECT 1 Code,
                   'Invalid details' Message;
            RETURN;
        END;

        UPDATE dbo.tbl_recommendation_detail_hold
        SET Status = 'R',
            UpdatedBy = @ActionUser,
            UpdatedDate = GETDATE(),
            UpdatedIP = @ActionIP,
            UpdatedPlatform = 'Admin'
        WHERE Sno = @HoldId
              AND ISNULL([Status], '') = 'P';

        SELECT 0 Code,
               'Recommendation request rejected successfully' Message;
        RETURN;
    END;
    ELSE IF ISNULL(@Flag, '') = 'grrd' --get recommendation request detail
    BEGIN
        SELECT a.RecommendationId,
               a.AgentId,
               a.AgentType,
               a.GroupId,
               a.OrderId,
               b.DisplayPageId,
               b.[Status] AS DisplayPageStatus
        FROM dbo.tbl_recommendation_detail a WITH (NOLOCK)
            INNER JOIN tbl_display_page b WITH (NOLOCK)
                ON b.RecommendationId = a.RecommendationId;
        RETURN;
    END;
    ELSE IF ISNULL(@Flag, '') = 'mrr' --manage recommendation request
    BEGIN
        IF NOT EXISTS
        (
            SELECT 'X'
            FROM dbo.tbl_admin a WITH (NOLOCK)
            WHERE a.ActionUser = @ActionUser
                  AND ISNULL(a.Status, '') = 'A'
        )
        BEGIN
            SELECT 1 Code,
                   'Invalid action user' Message;
            RETURN;
        END;

        IF NOT EXISTS
        (
            SELECT 'X'
            FROM dbo.tbl_recommendation_detail a WITH (NOLOCK)
                INNER JOIN tbl_display_page b WITH (NOLOCK)
                    ON b.RecommendationId = a.RecommendationId
                       AND ISNULL(b.[Status], '') = 'A'
            WHERE a.Sno = @HoldId
                  AND ISNULL(a.[Status], '') = 'A'
        )
        BEGIN
            SELECT 1 Code,
                   'Invalid details' Message;
            RETURN;
        END;

        SET @TransactionName = 'Function_mrr';

        BEGIN TRANSACTION @TransactionName;

        UPDATE dbo.tbl_recommendation_detail
        SET GroupId = ISNULL(@GroupId, GroupId),
            OrderId = ISNULL(@OrderId, OrderId),
            UpdatedBy = @ActionUser,
            UpdatedDate = GETDATE(),
            UpdatedIP = @ActionIP,
            UpdatedPlatform = @ActionPlatform
        WHERE RecommendationId = @RecommendationId
              AND ISNULL([Status], '') = 'A';

        UPDATE tbl_display_page
        SET [Status] = 'D',
            UpdatedBy = @ActionUser,
            UpdatedDate = GETDATE(),
            UpdatedIP = @ActionIP,
            UpdatedPlatform = @ActionPlatform
        WHERE RecommendationId = @RecommendationId
              AND ISNULL([Status], '') = 'A';

        CREATE TABLE #temp_displayidtable_u
        (
            sno BIGINT IDENTITY(1, 1),
            Id VARCHAR(10) NULL
        );

        INSERT INTO #temp_displayidtable_u
        (
            Id
        )
        SELECT *
        FROM STRING_SPLIT(ISNULL(@DisplayPageId, ''), ',');

        SET @StringSQL = 'INSERT INTO dbo.tbl_display_page';
        SET @StringSQL += '(RecommendationId,DisplayPageId,[Status],CreatedBy,CreatedIP,CreatedPlatform,CreatedDate)';
        SET @StringSQL += 'SELECT ' + CAST(@RecommendationId AS VARCHAR) + ',a.Id,''A'',''' + ISNULL(@ActionUser, '')
                          + ''',''';
        SET @StringSQL += @ActionIP + ',''' + @ActionPlatform
                          + ''',GETDATE() FROM #temp_displayidtable_u a with (NOLOCK)';

        PRINT (@StringSQL);
        EXEC (@StringSQL);

        SELECT 0 Code,
               'Recommendation request updated successfully' Message;

        COMMIT TRANSACTION @TransactionName;
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
    (@ErrorDesc, 'sproc_recommendation_management(Flag: ' + ISNULL(@Flag, '') + ')', 'SQL', 'SQL',
     'sproc_recommendation_management', GETDATE());

    SELECT 1 Code,
           'ErrorId:' + CAST(SCOPE_IDENTITY() AS VARCHAR) Message;
    RETURN;
END CATCH;
GO



