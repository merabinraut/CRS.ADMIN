USE [CRS];
GO

/****** Object:  StoredProcedure [dbo].[sproc_recommendation_request_management]    Script Date: 18/10/2023 13:30:01 ******/
SET ANSI_NULLS ON;
GO

SET QUOTED_IDENTIFIER ON;
GO





ALTER PROC [dbo].[sproc_recommendation_request_management]
    @Flag VARCHAR(10),
    @AgentId VARCHAR(10) = NULL,
    @AgentType VARCHAR(10) = NULL,
    @GroupId VARCHAR(10) = NULL,
    @DisplayId VARCHAR(10) = NULL,
    @OrderId VARCHAR(10) = NULL,
    @ActionUser VARCHAR(200) = NULL,
    @ActionIP VARCHAR(50) = NULL,
    @ActionPlatform VARCHAR(20) = NULL,
    @DisplayPageId VARCHAR(512) = NULL
AS
DECLARE @Sno BIGINT,
        @StringSQL VARCHAR(MAX),
        @TransactionName VARCHAR(200),
        @ErrorDesc VARCHAR(MAX);
BEGIN TRY
    IF ISNULL(@Flag, '') = 'irr' --insert recommendation request
    BEGIN
        IF EXISTS
        (
            SELECT 'X'
            FROM tbl_recommendation_detail_hold a WITH (NOLOCK)
            WHERE a.AgentId = @AgentId
                  AND a.AgentType = @AgentType
                  AND ISNULL(a.Status, '') NOT IN ( 'P' )
        )
        BEGIN
            SELECT 1 Code,
                   'Recommendation request already pending' Message;
            RETURN;
        END;

        SET @TransactionName = 'Function_irr';

        BEGIN TRANSACTION @TransactionName;

        INSERT INTO tbl_recommendation_detail_hold
        (
            AgentId,
            AgentType,
            GroupId,
            OrderId,
            Status,
            CreatedBy,
            CreatedDate,
            CreatedIP,
            CreatedPlatform
        )
        VALUES
        (@AgentId, @AgentType, @GroupId, @OrderId, 'P', @ActionUser, GETDATE(), @ActionIP, @ActionPlatform);

        SET @Sno = SCOPE_IDENTITY();

        CREATE TABLE #temp_displayidtable
        (
            sno BIGINT IDENTITY(1, 1),
            Id VARCHAR(10) NULL
        );

        INSERT INTO #temp_displayidtable
        (
            Id
        )
        SELECT *
        FROM STRING_SPLIT(ISNULL(@DisplayPageId, ''), ',');


        SET @StringSQL = 'INSERT INTO dbo.tbl_display_page_hold';
        SET @StringSQL += '(HoldId,DisplayPageId,[Status],CreatedBy,CreatedIP,CreatedPlatform,CreatedDate)';
        SET @StringSQL += 'SELECT ' + CAST(@Sno AS VARCHAR) + ',a.Id,''P'',''' + ISNULL(@ActionUser, '') + ''',''';
        SET @StringSQL += @ActionIP + ',''' + @ActionPlatform
                          + ''',GETDATE() FROM #temp_displayidtable a with (NOLOCK)';

        PRINT (@StringSQL);
        EXEC (@StringSQL);

        SELECT 0 Code,
               'Request created successfully' Message;

        COMMIT TRANSACTION @TransactionName;
        RETURN;
    END;
    ELSE IF ISNULL(@Flag, '') = 'rhl' --request hold list
    BEGIN
        SELECT a.Sno,
               a.AgentType,
               a.[Status],
               a.CreatedBy,
               a.CreatedDate,
               a.UpdatedDate,
               CASE
                   WHEN a.AgentType = 'Club' THEN
                   (
                       SELECT a1.ClubName1
                       FROM dbo.tbl_club_details a1 WITH (NOLOCK)
                       WHERE a1.AgentId = a.AgentId
                   )
                   WHEN a.AgentType = 'Host' THEN
                   (
                       SELECT b1.HostName
                       FROM dbo.tbl_host_details b1 WITH (NOLOCK)
                       WHERE b1.AgentId = a.AgentId
                   )
                   ELSE
                       ''
               END AS AgentName
        FROM tbl_recommendation_detail_hold a WITH (NOLOCK)
        WHERE ISNULL(a.[Status], '') NOT IN ( 'D', 'S' );
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
    (@ErrorDesc, 'sproc_recommendation_request_management(Flag: ' + ISNULL(@Flag, '') + ')', 'SQL', 'SQL',
     'sproc_recommendation_request_management', GETDATE());

    SELECT 1 Code,
           'ErrorId:' + CAST(SCOPE_IDENTITY() AS VARCHAR) Message;
    RETURN;
END CATCH;
GO


