USE [CRS];
GO

/****** Object:  StoredProcedure [dbo].[sproc_admin_recommendation_request_management]    Script Date: 26/11/2023 11:43:20 ******/
SET ANSI_NULLS ON;
GO

SET QUOTED_IDENTIFIER ON;
GO




ALTER PROC [dbo].[sproc_admin_recommendation_request_management]
    @Flag VARCHAR(10),
    @RecommendationHoldId VARCHAR(10) = NULL,
    @ActionUser VARCHAR(200) = NULL,
    @ActionIP VARCHAR(50) = NULL,
    @ActionPlatform VARCHAR(20) = NULL
AS
DECLARE @Sno BIGINT,
        @Sno2 BIGINT,
        @StringSQL VARCHAR(MAX),
        @StringSQL2 VARCHAR(MAX),
        @TransactionName VARCHAR(200),
        @ErrorDesc VARCHAR(MAX);
BEGIN TRY
    IF @Flag = 'grrl' --get recommendation requests list
    BEGIN
        SELECT a.RecommendationHoldId,
               c.AgentId AS ClubId,
               c.ClubName1 AS ClubName,
               c.Logo AS ClubLogo,
               'Platinum' AS ClubCategory,
               a.Status AS HoldStatus,
               a.CreatedDate AS RequestedDate,
               a.UpdatedDate AS UpdatedDate
        FROM tbl_recommendation_detail_hold a WITH (NOLOCK)
            INNER JOIN tbl_display_page_hold b WITH (NOLOCK)
                ON b.RecommendationHoldId = a.RecommendationHoldId
                   AND b.Status <> 'D'
            INNER JOIN tbl_club_details c WITH (NOLOCK)
                ON c.AgentId = a.ClubId
                   AND c.Status = 'A'
        WHERE a.Status <> 'D';

        RETURN;
    END;

    ELSE IF @Flag = 'gra' --get recommendation analytic
    BEGIN
        WITH CTE
        AS (SELECT Status,
                   COUNT(a.Sno) AS status_count
            FROM dbo.tbl_recommendation_detail_hold a WITH (NOLOCK)
            WHERE a.Status IN ( 'A', 'P', 'R' )
            GROUP BY a.Status WITH ROLLUP)
        SELECT CASE
                   WHEN a.Status IS NULL THEN
                       'Total Requests'
                   WHEN a.Status = 'P' THEN
                       'Pending Requests'
                   WHEN a.Status = 'A' THEN
                       'Approved Requests'
                   WHEN a.Status = 'R' THEN
                       'Rejected Requests'
               END AS StatusLabel,
               a.status_count AS StatusCount
        FROM CTE a WITH (NOLOCK);
        RETURN;
    END;

    ELSE IF @Flag = 'rrr' --reject recommendation request
    BEGIN
        IF NOT EXISTS
        (
            SELECT 'X'
            FROM tbl_recommendation_detail_hold a WITH (NOLOCK)
                INNER JOIN tbl_display_page_hold b WITH (NOLOCK)
                    ON b.RecommendationHoldId = a.RecommendationHoldId
                       AND b.Status <> 'D'
                INNER JOIN tbl_club_details c WITH (NOLOCK)
                    ON c.AgentId = a.ClubId
                       AND c.Status = 'A'
            WHERE a.RecommendationHoldId = @RecommendationHoldId
                  AND a.Status = 'P'
        )
        BEGIN
            SELECT 1 Code,
                   'Invalid recommendation request details' Message;
            RETURN;
        END;

        SET @TransactionName = 'Transaction_rrr';
        BEGIN TRANSACTION @TransactionName;

        UPDATE tbl_recommendation_detail_hold
        SET Status = 'R',
            UpdatedBy = @ActionUser,
            UpdatedDate = GETDATE(),
            UpdatedIP = @ActionIP,
            UpdatedPlatform = @ActionPlatform
        WHERE RecommendationHoldId = @RecommendationHoldId
              AND Status = 'P';

        UPDATE tbl_display_page_hold
        SET Status = 'R',
            UpdatedBy = @ActionUser,
            UpdatedDate = GETDATE(),
            UpdatedIP = @ActionIP,
            UpdatedPlatform = @ActionPlatform
        WHERE RecommendationHoldId = @RecommendationHoldId
              AND Status = 'P';

        UPDATE tbl_host_recommendation_detail_hold
        SET Status = 'R',
            UpdatedBy = @ActionUser,
            UpdatedDate = GETDATE(),
            UpdatedIP = @ActionIP,
            UpdatedPlatform = @ActionPlatform
        WHERE RecommendationHoldId = @RecommendationHoldId
              AND Status = 'P';

        COMMIT TRANSACTION @TransactionName;

        SELECT 0 Code,
               'Recommendation request rejected successfully' Message;
        RETURN;
    END;

    ELSE IF @Flag = 'arr' --accept recommendation request
    BEGIN
        IF NOT EXISTS
        (
            SELECT 'X'
            FROM tbl_recommendation_detail_hold a WITH (NOLOCK)
                INNER JOIN tbl_display_page_hold b WITH (NOLOCK)
                    ON b.RecommendationHoldId = a.RecommendationHoldId
                       AND b.Status <> 'D'
                INNER JOIN tbl_club_details c WITH (NOLOCK)
                    ON c.AgentId = a.ClubId
                       AND c.Status = 'A'
            WHERE a.RecommendationHoldId = @RecommendationHoldId
                  AND a.Status = 'P'
        )
        BEGIN
            SELECT 1 Code,
                   'Invalid recommendation request details' Message;
            RETURN;
        END;

        IF EXISTS
        (
            SELECT 'X'
            FROM dbo.tbl_recommendation_detail a WITH (NOLOCK)
            WHERE a.Status = 'A'
                  AND a.ClubId =
                  (
                      SELECT b.ClubId
                      FROM dbo.tbl_recommendation_detail_hold b WITH (NOLOCK)
                      WHERE b.RecommendationHoldId = @RecommendationHoldId
                            AND b.Status = 'P'
                  )
        )
        BEGIN
            SELECT 1 Code,
                   'Recommendation detail already exists for the club' Message;
            RETURN;
        END;

        SET @TransactionName = 'Transaction_arr';
        BEGIN TRANSACTION @TransactionName;

        INSERT INTO dbo.tbl_recommendation_detail
        (
            ClubId,
            GroupId,
            OrderId,
            Status,
            CreatedBy,
            CreatedDate,
            CreatedIP,
            CreatedPlatform
        )
        SELECT b.AgentId,
               a.GroupId,
               a.OrderId,
               'A',
               @ActionUser,
               GETDATE(),
               @ActionIP,
               @ActionPlatform
        FROM dbo.tbl_recommendation_detail_hold a WITH (NOLOCK)
            INNER JOIN dbo.tbl_club_details b WITH (NOLOCK)
                ON b.AgentId = a.ClubId
                   AND b.Status = 'A'
        WHERE a.RecommendationHoldId = @RecommendationHoldId
              AND a.Status = 'P';

        SET @Sno = SCOPE_IDENTITY();

        UPDATE dbo.tbl_recommendation_detail
        SET RecommendationId = @Sno
        WHERE Sno = @Sno;

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
        SELECT @Sno,
               a.DisplayPageId,
               'A',
               @ActionUser,
               GETDATE(),
               @ActionIP,
               @ActionPlatform
        FROM dbo.tbl_display_page_hold a WITH (NOLOCK)
        WHERE a.RecommendationHoldId = @RecommendationHoldId
              AND a.Status = 'P';

        INSERT INTO dbo.tbl_host_recommendation_detail
        (
            RecommendationId,
            ClubId,
            HostId,
            OrderId,
            Status,
            CreatedBy,
            CreatedDate,
            CreatedIP,
            CreatedPlatform
        )
        SELECT @Sno,
               b.AgentId,
               c.HostId,
               a.OrderId,
               'A',
               @ActionUser,
               GETDATE(),
               @ActionIP,
               @ActionPlatform
        FROM dbo.tbl_host_recommendation_detail_hold a WITH (NOLOCK)
            INNER JOIN dbo.tbl_club_details b WITH (NOLOCK)
                ON b.AgentId = a.ClubId
                   AND b.Status = 'A'
            INNER JOIN dbo.tbl_host_details c WITH (NOLOCK)
                ON c.AgentId = b.AgentId
                   AND c.HostId = a.HostId
                   AND c.Status = 'A'
        WHERE a.RecommendationHoldId = @RecommendationHoldId
              AND a.Status = 'P';

        UPDATE dbo.tbl_host_recommendation_detail
        SET RecommendationHostId = Sno
        WHERE RecommendationId = @Sno
              AND Status = 'A';


        COMMIT TRANSACTION @TransactionName;

        SELECT 0 Code,
               'Recommendation request approved successfully' Message;
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
    (@ErrorDesc, 'sproc_admin_recommendation_request_management(Flag: ' + ISNULL(@Flag, '') + ')', 'SQL', 'SQL',
     'sproc_admin_recommendation_request_management', GETDATE());

    SELECT 1 Code,
           'ErrorId:' + CAST(SCOPE_IDENTITY() AS VARCHAR) Message;
    RETURN;
END CATCH;
GO


