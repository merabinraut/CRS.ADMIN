USE [CRS];
GO

/****** Object:  StoredProcedure [dbo].[sproc_admin_recommendation_request_management_v2]    Script Date: 07/12/2023 15:55:20 ******/
SET ANSI_NULLS ON;
GO

SET QUOTED_IDENTIFIER ON;
GO



ALTER PROC [dbo].[sproc_admin_recommendation_request_management_v2]
    @Flag VARCHAR(10),
    @ActionUser VARCHAR(200) = NULL,
    @ActionIP VARCHAR(50) = NULL,
    @ActionPlatform VARCHAR(20) = NULL,
    @RecommendationHoldId VARCHAR(10) = NULL,
    @ClubId VARCHAR(10) = NULL,
    @LocationId VARCHAR(10) = NULL,
    @DisplayId VARCHAR(10) = NULL,
    @RecommendationHoldHostId VARCHAR(10) = NULL
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
        WITH CTE
        AS (SELECT a.RecommendationHoldId,
                   b.AgentId AS ClubId,
                   b.ClubName1 AS ClubName,
                   b.Logo AS ClubLogo,
                   'Gold' AS ClubCategory,
                   d.StaticDataLabel AS DisplayPage,
                   '' AS DisplayOrder,
                   a.CreatedDate AS RequestedDate,
                   a.UpdatedDate AS UpdatedDate,
                   CASE
                       WHEN ISNULL(c.Status, '') = 'A' THEN
                           'Approved'
                       WHEN ISNULL(c.Status, '') = 'P' THEN
                           'Pending'
                       WHEN ISNULL(c.Status, '') = 'R' THEN
                           'Rejected'
                       ELSE
                           '-'
                   END AS Status,
                   a.LocationId,
                   c.Sno DisplayId
            FROM dbo.tbl_recommendation_detail_hold a WITH (NOLOCK)
                INNER JOIN dbo.tbl_club_details b WITH (NOLOCK)
                    ON b.AgentId = ClubId
                       AND b.LocationId = a.LocationId
                       AND ISNULL(b.Status, '') = 'A'
                       AND ISNULL(a.Status, '') <> 'D'
                INNER JOIN dbo.tbl_display_page_hold c WITH (NOLOCK)
                    ON c.RecommendationHoldId = a.RecommendationHoldId
                       AND ISNULL(c.Status, '') = 'A'
                LEFT JOIN dbo.tbl_static_data d WITH (NOLOCK)
                    ON d.StaticDataType = 6
                       AND d.StaticDataValue = c.DisplayPageId
                       AND ISNULL(d.Status, '') = 'A'
            WHERE c.DisplayPageId IN ( 1, 2 )
            UNION
            SELECT a.RecommendationHoldId,
                   b.AgentId AS ClubId,
                   b.ClubName1 AS ClubName,
                   b.Logo AS ClubLogo,
                   'Gold' AS ClubCategory,
                   d.StaticDataLabel AS DisplayPage,
                   e.StaticDataLabel AS DisplayOrder,
                   a.CreatedDate AS RequestedDate,
                   a.UpdatedDate AS UpdatedDate,
                   CASE
                       WHEN ISNULL(c.Status, '') = 'A' THEN
                           'Approved'
                       WHEN ISNULL(c.Status, '') = 'P' THEN
                           'Pending'
                       WHEN ISNULL(c.Status, '') = 'R' THEN
                           'Rejected'
                       ELSE
                           '-'
                   END AS Status,
                   a.LocationId,
                   c.Sno DisplayId
            FROM dbo.tbl_recommendation_detail_hold a WITH (NOLOCK)
                INNER JOIN dbo.tbl_club_details b WITH (NOLOCK)
                    ON b.AgentId = ClubId
                       AND b.LocationId = a.LocationId
                       AND ISNULL(b.Status, '') = 'A'
                       AND ISNULL(a.Status, '') <> 'D'
                INNER JOIN dbo.tbl_display_mainpage_hold c WITH (NOLOCK)
                    ON c.RecommendationHoldId = a.RecommendationHoldId
                       AND ISNULL(c.Status, '') = 'A'
                LEFT JOIN dbo.tbl_static_data d WITH (NOLOCK)
                    ON d.StaticDataType = 6
                       AND d.StaticDataValue = c.DisplayPageId
                       AND ISNULL(d.Status, '') = 'A'
                LEFT JOIN dbo.tbl_static_data e WITH (NOLOCK)
                    ON e.StaticDataType = 5
                       AND e.StaticDataValue = c.OrderId
                       AND ISNULL(e.Status, '') = 'A'
            WHERE c.DisplayPageId = 3)
        SELECT *
        FROM CTE a WITH (NOLOCK)
        ORDER BY a.RecommendationHoldId DESC;

        RETURN;
    END;

    ELSE IF @Flag = 'grrshhl' --get recommendation requests search & homepage host list
    BEGIN
        SELECT a.RecommendationHoldId,
               b.AgentId AS ClubId,
               e.Sno AS RecommendationHostHoldId,
               d.HostName,
               e.CreatedDate AS CreatedDate,
               e.UpdatedDate AS UpdatedDate,
               ISNULL(
               (
                   SELECT TOP 1
                          a2.ImagePath
                   FROM dbo.tbl_gallery a2 WITH (NOLOCK)
                   WHERE a2.AgentId = d.HostId
                   ORDER BY 1 DESC
               ),
               ''
                     ) AS HostImage,
               c.Sno DisplayId
        FROM dbo.tbl_recommendation_detail_hold a WITH (NOLOCK)
            INNER JOIN dbo.tbl_club_details b WITH (NOLOCK)
                ON b.AgentId = ClubId
                   AND b.LocationId = a.LocationId
                   AND ISNULL(b.Status, '') = 'A'
                   AND ISNULL(a.Status, '') <> 'D'
            INNER JOIN dbo.tbl_display_page_hold c WITH (NOLOCK)
                ON c.RecommendationHoldId = a.RecommendationHoldId
                   AND ISNULL(c.Status, '') = 'A'
            INNER JOIN dbo.tbl_host_details d WITH (NOLOCK)
                ON d.AgentId = b.AgentId
                   AND ISNULL(d.Status, '') = 'A'
            LEFT JOIN dbo.tbl_host_recommendation_detail_hold e WITH (NOLOCK)
                ON e.RecommendationHoldId = a.RecommendationHoldId
                   AND e.ClubId = b.AgentId
                   AND e.HostId = d.HostId
                   AND e.DisplayPageId = c.DisplayPageId
                   AND ISNULL(e.Status, '') = 'A'
            LEFT JOIN dbo.tbl_static_data f WITH (NOLOCK)
                ON f.StaticDataType = 6
                   AND f.StaticDataValue = c.DisplayPageId
                   AND ISNULL(f.Status, '') = 'A'
        WHERE c.DisplayPageId IN ( 1, 2 )
              AND a.RecommendationHoldId = @RecommendationHoldId
              AND a.ClubId = @ClubId
              AND a.LocationId = @LocationId
              AND c.Sno = @DisplayId;
        RETURN;
    END;

    ELSE IF @Flag = 'grrmhl' --get recommendation requests mainpage host list
    BEGIN
        SELECT a.RecommendationHoldId,
               b.AgentId AS ClubId,
               e.Sno AS RecommendationHostHoldId,
               d.HostName,
               e.CreatedDate AS CreatedDate,
               e.UpdatedDate AS UpdatedDate,
               ISNULL(
               (
                   SELECT TOP 1
                          a2.ImagePath
                   FROM dbo.tbl_gallery a2 WITH (NOLOCK)
                   WHERE a2.AgentId = d.HostId
                   ORDER BY 1 DESC
               ),
               ''
                     ) AS HostImage,
               g.StaticDataLabel AS DisplayOrder,
               c.Sno DisplayId
        FROM dbo.tbl_recommendation_detail_hold a WITH (NOLOCK)
            INNER JOIN dbo.tbl_club_details b WITH (NOLOCK)
                ON b.AgentId = ClubId
                   AND b.LocationId = a.LocationId
                   AND ISNULL(b.Status, '') = 'A'
                   AND ISNULL(a.Status, '') <> 'D'
            INNER JOIN dbo.tbl_display_mainpage_hold c WITH (NOLOCK)
                ON c.RecommendationHoldId = a.RecommendationHoldId
                   AND ISNULL(c.Status, '') = 'A'
            INNER JOIN dbo.tbl_host_details d WITH (NOLOCK)
                ON d.AgentId = b.AgentId
                   AND ISNULL(d.Status, '') = 'A'
            INNER JOIN dbo.tbl_host_recommendation_detail_hold e WITH (NOLOCK)
                ON e.RecommendationHoldId = a.RecommendationHoldId
                   AND e.ClubId = b.AgentId
                   AND e.HostId = d.HostId
                   AND e.DisplayPageId = c.DisplayPageId
                   AND ISNULL(e.Status, '') = 'A'
            LEFT JOIN dbo.tbl_static_data f WITH (NOLOCK)
                ON f.StaticDataType = 6
                   AND f.StaticDataValue = c.DisplayPageId
                   AND ISNULL(f.Status, '') = 'A'
            LEFT JOIN dbo.tbl_static_data g WITH (NOLOCK)
                ON g.StaticDataType = 5
                   AND g.StaticDataValue = c.OrderId
                   AND ISNULL(g.Status, '') = 'A'
        WHERE c.DisplayPageId = 3
              AND a.RecommendationHoldId = @RecommendationHoldId
              AND a.ClubId = @ClubId
              AND a.LocationId = @LocationId
              AND c.Sno = @DisplayId;
        RETURN;
    END;

    ELSE IF @Flag = 'rhsrr' --reject home and search page recommendation requests
    BEGIN
        IF NOT EXISTS
        (
            SELECT 'X'
            FROM dbo.tbl_recommendation_detail_hold a WITH (NOLOCK)
                INNER JOIN dbo.tbl_club_details b WITH (NOLOCK)
                    ON b.AgentId = ClubId
                       AND b.LocationId = a.LocationId
                       AND ISNULL(b.Status, '') = 'A'
                       AND ISNULL(a.Status, '') <> 'D'
                INNER JOIN dbo.tbl_display_page_hold c WITH (NOLOCK)
                    ON c.RecommendationHoldId = a.RecommendationHoldId
                       AND ISNULL(c.Status, '') = 'P'
            WHERE c.DisplayPageId IN ( 1, 2 )
                  AND a.RecommendationHoldId = @RecommendationHoldId
                  AND a.ClubId = @ClubId
                  AND a.LocationId = @LocationId
                  AND c.Sno = @DisplayId
        )
        BEGIN
            SELECT 1 Code,
                   'Invalid request' Message;
            RETURN;
        END;

        UPDATE dbo.tbl_display_page_hold
        SET Status = 'R',
            UpdatedBy = @ActionUser,
            UpdatedDate = GETDATE(),
            UpdatedIP = @ActionIP,
            UpdatedPlatform = @ActionPlatform
        FROM dbo.tbl_display_page_hold a WITH (NOLOCK)
            INNER JOIN dbo.tbl_recommendation_detail_hold b WITH (NOLOCK)
                ON b.RecommendationHoldId = a.RecommendationHoldId
                   AND ISNULL(b.Status, '') = 'A'
        WHERE a.DisplayPageId IN ( 1, 2 )
              AND a.RecommendationHoldId = @RecommendationHoldId
              AND b.ClubId = @ClubId
              AND b.LocationId = @LocationId
              AND a.Sno = @DisplayId;

        SELECT 0 Code,
               'Recommendation request rejected successfully' Message;
        RETURN;
    END;

    ELSE IF @Flag = 'rmprr' --reject main page recommendation requests
    BEGIN
        IF NOT EXISTS
        (
            SELECT 'X'
            FROM dbo.tbl_recommendation_detail_hold a WITH (NOLOCK)
                INNER JOIN dbo.tbl_club_details b WITH (NOLOCK)
                    ON b.AgentId = ClubId
                       AND b.LocationId = a.LocationId
                       AND ISNULL(b.Status, '') = 'A'
                       AND ISNULL(a.Status, '') <> 'D'
                INNER JOIN dbo.tbl_display_mainpage_hold c WITH (NOLOCK)
                    ON c.RecommendationHoldId = a.RecommendationHoldId
                       AND ISNULL(c.Status, '') = 'P'
            WHERE c.DisplayPageId = 3
                  AND a.RecommendationHoldId = @RecommendationHoldId
                  AND a.ClubId = @ClubId
                  AND a.LocationId = @LocationId
                  AND c.Sno = @DisplayId
        )
        BEGIN
            SELECT 1 Code,
                   'Invalid request' Message;
            RETURN;
        END;

        UPDATE dbo.tbl_display_mainpage_hold
        SET Status = 'R',
            UpdatedBy = @ActionUser,
            UpdatedDate = GETDATE(),
            UpdatedIP = @ActionIP,
            UpdatedPlatform = @ActionPlatform
        FROM dbo.tbl_display_mainpage_hold a WITH (NOLOCK)
            INNER JOIN dbo.tbl_recommendation_detail_hold b WITH (NOLOCK)
                ON b.RecommendationHoldId = a.RecommendationHoldId
                   AND ISNULL(b.Status, '') = 'A'
                   AND ISNULL(a.Status, '') = 'P'
        WHERE a.DisplayPageId = 3
              AND a.RecommendationHoldId = @RecommendationHoldId
              AND b.ClubId = @ClubId
              AND b.LocationId = @LocationId
              AND a.Sno = @DisplayId;

        SELECT 0 Code,
               'Recommendation request rejected successfully' Message;
        RETURN;
    END;

    ELSE IF @Flag = 'rmphrr' --remove main page single host recommendation requests
    BEGIN
        IF NOT EXISTS
        (
            SELECT 'X'
            FROM dbo.tbl_recommendation_detail_hold a WITH (NOLOCK)
                INNER JOIN dbo.tbl_host_recommendation_detail_hold b WITH (NOLOCK)
                    ON b.RecommendationHoldId = a.RecommendationHoldId
                       AND b.ClubId = a.ClubId
                       AND b.DisplayPageId = 3
                       AND ISNULL(a.Status, '') <> 'D'
                       AND ISNULL(b.Status, '') = 'P'
            WHERE a.RecommendationHoldId = @RecommendationHoldId
                  AND a.ClubId = @ClubId
                  AND b.Sno = @RecommendationHoldHostId
        )
        BEGIN
            SELECT 1 Code,
                   'Invalid request' Message;
            RETURN;
        END;

        UPDATE dbo.tbl_host_recommendation_detail_hold
        SET Status = 'R',
            UpdatedBy = @ActionUser,
            UpdatedDate = GETDATE(),
            UpdatedIP = @ActionIP,
            UpdatedPlatform = @ActionPlatform
        FROM dbo.tbl_host_recommendation_detail_hold a WITH (NOLOCK)
            INNER JOIN dbo.tbl_recommendation_detail_hold b WITH (NOLOCK)
                ON b.RecommendationHoldId = a.RecommendationHoldId
                   AND ISNULL(b.Status, '') <> 'D'
                   AND ISNULL(a.Status, '') = 'P'
        WHERE a.DisplayPageId = 3
              AND a.RecommendationHoldId = @RecommendationHoldId
              AND b.ClubId = @ClubId
              AND a.Sno = @RecommendationHoldHostId;

        SELECT 0 Code,
               'Host removed successfully from recommendation request' Message;
        RETURN;
    END;

    ELSE IF @Flag = 'rhsphrr' --remove home & search page single host recommendation requests
    BEGIN
        IF NOT EXISTS
        (
            SELECT 'X'
            FROM dbo.tbl_recommendation_detail_hold a WITH (NOLOCK)
                INNER JOIN dbo.tbl_host_recommendation_detail_hold b WITH (NOLOCK)
                    ON b.RecommendationHoldId = a.RecommendationHoldId
                       AND b.ClubId = a.ClubId
                       AND b.DisplayPageId IN ( 1, 2 )
                       AND ISNULL(a.Status, '') <> 'D'
                       AND ISNULL(b.Status, '') = 'P'
            WHERE a.RecommendationHoldId = @RecommendationHoldId
                  AND a.ClubId = @ClubId
                  AND b.Sno = @RecommendationHoldHostId
        )
        BEGIN
            SELECT 1 Code,
                   'Invalid request' Message;
            RETURN;
        END;

        UPDATE dbo.tbl_host_recommendation_detail_hold
        SET Status = 'R',
            UpdatedBy = @ActionUser,
            UpdatedDate = GETDATE(),
            UpdatedIP = @ActionIP,
            UpdatedPlatform = @ActionPlatform
        FROM dbo.tbl_host_recommendation_detail_hold a WITH (NOLOCK)
            INNER JOIN dbo.tbl_recommendation_detail_hold b WITH (NOLOCK)
                ON b.RecommendationHoldId = a.RecommendationHoldId
                   AND ISNULL(b.Status, '') <> 'D'
                   AND ISNULL(a.Status, '') = 'P'
        WHERE a.DisplayPageId IN ( 1, 2 )
              AND a.RecommendationHoldId = @RecommendationHoldId
              AND b.ClubId = @ClubId
              AND a.Sno = @RecommendationHoldHostId;

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
    (@ErrorDesc, 'sproc_admin_recommendation_request_management_v2(Flag: ' + ISNULL(@Flag, '') + ')', 'SQL', 'SQL',
     'sproc_admin_recommendation_request_management_v2', GETDATE());

    SELECT 1 Code,
           'ErrorId:' + CAST(SCOPE_IDENTITY() AS VARCHAR) Message;
    RETURN;
END CATCH;
GO


