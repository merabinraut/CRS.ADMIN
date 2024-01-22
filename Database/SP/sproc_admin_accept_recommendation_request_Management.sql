USE [CRS];
GO

/****** Object:  StoredProcedure [dbo].[sproc_admin_accept_recommendation_request_Management]    Script Date: 07/12/2023 21:55:29 ******/
SET ANSI_NULLS ON;
GO

SET QUOTED_IDENTIFIER ON;
GO


ALTER PROC [dbo].[sproc_admin_accept_recommendation_request_Management]
    @Flag VARCHAR(10),
    @RecommendationHoldId VARCHAR(10) = NULL,
    @DisplayIdHold VARCHAR(10) = NULL,
    @ActionUser VARCHAR(200) = NULL,
    @ActionIP VARCHAR(50) = NULL,
    @ActionPlatform VARCHAR(20) = NULL,
    @ClubId VARCHAR(10) = NULL,
    @RecommendationId VARCHAR(10) = NULL,
    @DisplayPageId VARCHAR(10) = NULL,
    @LocationId VARCHAR(10) = NULL,
    @GroupId VARCHAR(10) = NULL
AS
DECLARE @Sno BIGINT,
        @Sno2 BIGINT,
        @StringSQL VARCHAR(MAX),
        @StringSQL2 VARCHAR(MAX),
        @TransactionName VARCHAR(200),
        @ErrorDesc VARCHAR(MAX);
BEGIN TRY
    IF @Flag = 'ahsrr' --approve home and search page recommendation request
    BEGIN
        IF NOT EXISTS
        (
            SELECT 'X'
            FROM dbo.tbl_recommendation_detail_hold a WITH (NOLOCK)
                INNER JOIN dbo.tbl_display_page_hold b WITH (NOLOCK)
                    ON b.RecommendationHoldId = a.RecommendationHoldId
                       AND ISNULL(a.Status, '') <> 'D'
                       AND ISNULL(b.Status, '') = 'P'
                       AND b.DisplayPageId IN ( 1, 2 )
            WHERE a.RecommendationHoldId = @RecommendationHoldId
                  AND a.ClubId = @ClubId
                  AND b.Sno = @DisplayIdHold
        )
        BEGIN
            SELECT 1 Code,
                   'Invalid request' Message;
            RETURN;
        END;

        SELECT @DisplayPageId = b.DisplayPageId,
               @LocationId = a.LocationId
        FROM dbo.tbl_recommendation_detail_hold a WITH (NOLOCK)
            INNER JOIN dbo.tbl_display_page_hold b WITH (NOLOCK)
                ON b.RecommendationHoldId = a.RecommendationHoldId
                   AND ISNULL(a.Status, '') <> 'D'
                   AND ISNULL(b.Status, '') = 'P'
                   AND b.DisplayPageId IN ( 1, 2 )
        WHERE a.RecommendationHoldId = @RecommendationHoldId
              AND a.ClubId = @ClubId
              AND b.Sno = @DisplayIdHold
              AND a.LocationId = @LocationId;

        IF
        (
            SELECT COUNT(b.Sno)
            FROM dbo.tbl_recommendation_detail a WITH (NOLOCK)
                INNER JOIN dbo.tbl_display_page b WITH (NOLOCK)
                    ON b.RecommendationId = a.RecommendationId
                       AND a.LocationId = @LocationId
                       AND b.DisplayPageId = @DisplayPageId
                       AND ISNULL(b.Status, '') = 'A'
        ) >= 10
        BEGIN
            SELECT 1 Code,
                   'Page already consist max number of club for recommendation' Message;
            RETURN;
        END;

        IF
        (
            SELECT COUNT(b.Sno)
            FROM dbo.tbl_recommendation_detail a WITH (NOLOCK)
                INNER JOIN dbo.tbl_host_recommendation_detail b WITH (NOLOCK)
                    ON b.RecommendationId = a.RecommendationId
                       AND a.LocationId = @LocationId
                       AND b.DisplayPageId = @DisplayPageId
                       AND ISNULL(b.Status, '') = 'A'
        ) >= 10
        BEGIN
            SELECT 1 Code,
                   'Page already consist max number of host for recommendation' Message;
            RETURN;
        END;

        SET @TransactionName = 'Transaction_ahsrr';

        BEGIN TRANSACTION @TransactionName;

        SELECT @RecommendationId = a.RecommendationId
        FROM dbo.tbl_recommendation_detail a WITH (NOLOCK)
        WHERE a.ClubId = @ClubId
              AND a.LocationId = @LocationId
              AND ISNULL(a.Status, '') <> 'D';

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
            SELECT a.LocationId,
                   a.ClubId,
                   'A',
                   @ActionUser,
                   GETDATE(),
                   @ActionIP,
                   @ActionPlatform
            FROM dbo.tbl_recommendation_detail_hold a WITH (NOLOCK)
            WHERE a.RecommendationHoldId = @RecommendationHoldId
                  AND a.ClubId = @ClubId
                  AND a.LocationId = @LocationId
                  AND ISNULL(a.Status, '') = 'P';

            SET @RecommendationId = SCOPE_IDENTITY();
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
        SELECT @RecommendationId,
               a.DisplayPageId,
               'A',
               @ActionUser,
               GETDATE(),
               @ActionIP,
               @ActionPlatform
        FROM dbo.tbl_display_page_hold a WITH (NOLOCK)
            INNER JOIN dbo.tbl_recommendation_detail_hold b WITH (NOLOCK)
                ON b.RecommendationHoldId = a.RecommendationHoldId
                   AND ISNULL(b.Status, '') <> 'D'
                   AND ISNULL(a.Status, '') = 'P'
                   AND a.DisplayPageId = @DisplayPageId
        WHERE b.RecommendationHoldId = @RecommendationHoldId
              AND b.ClubId = @ClubId
              AND a.Sno = @DisplayIdHold;

        SET @Sno = SCOPE_IDENTITY();

        UPDATE dbo.tbl_display_page_hold
        SET Status = 'A',
            UpdatedBy = @ActionUser,
            UpdatedDate = GETDATE(),
            UpdatedIP = @ActionIP,
            UpdatedPlatform = @ActionPlatform
        FROM dbo.tbl_display_page_hold a WITH (NOLOCK)
            INNER JOIN dbo.tbl_recommendation_detail_hold b WITH (NOLOCK)
                ON b.RecommendationHoldId = a.RecommendationHoldId
                   AND ISNULL(b.Status, '') <> 'D'
                   AND ISNULL(a.Status, '') = 'P'
        WHERE b.RecommendationHoldId = @RecommendationHoldId
              AND b.ClubId = @ClubId
              AND a.Sno = @DisplayIdHold;

        IF EXISTS
        (
            SELECT 'X'
            FROM dbo.tbl_host_recommendation_detail_hold a WITH (NOLOCK)
            WHERE a.RecommendationHoldId = @RecommendationHoldId
                  AND a.ClubId = @ClubId
                  AND a.DisplayPageId = @DisplayPageId
                  AND ISNULL(a.Status, '') = 'P'
        )
        BEGIN
            IF NOT EXISTS
            (
                SELECT 'X'
                FROM dbo.tbl_host_recommendation_detail_hold a WITH (NOLOCK)
                    INNER JOIN dbo.tbl_club_details b WITH (NOLOCK)
                        ON b.AgentId = a.ClubId
                           AND b.Status = 'A'
                           AND ISNULL(a.Status, '') = 'P'
                    INNER JOIN dbo.tbl_host_details c WITH (NOLOCK)
                        ON c.AgentId = b.AgentId
                           AND c.HostId = a.HostId
                           AND ISNULL(c.Status, '') = 'A'
                WHERE a.RecommendationHoldId = @RecommendationHoldId
                      AND a.ClubId = @ClubId
                      AND a.DisplayPageId = @DisplayPageId
                      AND ISNULL(a.Status, '') = 'P'
            )
            BEGIN
                SELECT 1 Code,
                       'Invalid host details' Message;
                ROLLBACK TRANSACTION @TransactionName;
                RETURN;
            END;

            IF EXISTS
            (
                SELECT 'X'
                FROM dbo.tbl_host_recommendation_detail_hold a WITH (NOLOCK)
                WHERE a.RecommendationHoldId = @RecommendationHoldId
                      AND a.Status = 'P'
                      AND a.ClubId = @ClubId
                      AND a.DisplayPageId = @DisplayPageId
                      AND a.HostId IN
                          (
                              SELECT a2.HostId
                              FROM dbo.tbl_host_recommendation_detail a2 WITH (NOLOCK)
                              WHERE a2.Status = 'A'
                                    AND a2.ClubId = @ClubId
                                    AND a2.DisplayPageId = a.DisplayPageId
                          )
            )
            BEGIN
                SELECT 1 Code,
                       'Host details already exist' Message;
                ROLLBACK TRANSACTION @TransactionName;
                RETURN;
            END;

            INSERT INTO dbo.tbl_host_recommendation_detail
            (
                RecommendationId,
                ClubId,
                DisplayPageId,
                HostId,
                OrderId,
                Status,
                CreatedBy,
                CreatedDate,
                CreatedIP,
                CreatedPlatform
            )
            SELECT @RecommendationId,
                   b.AgentId,
                   a.DisplayPageId,
                   a.HostId,
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
                       AND ISNULL(a.Status, '') = 'P'
                INNER JOIN dbo.tbl_host_details c WITH (NOLOCK)
                    ON c.AgentId = b.AgentId
                       AND c.HostId = a.HostId
                       AND ISNULL(c.Status, '') = 'A'
            WHERE a.RecommendationHoldId = @RecommendationHoldId
                  AND a.ClubId = @ClubId
                  AND a.DisplayPageId = @DisplayPageId
                  AND ISNULL(a.Status, '') = 'P';

            UPDATE dbo.tbl_host_recommendation_detail
            SET RecommendationHostId = Sno
            WHERE RecommendationId = RecommendationId
                  AND DisplayPageId = @DisplayPageId
                  AND ClubId = @ClubId
                  AND Status = 'A';

            UPDATE dbo.tbl_host_recommendation_detail_hold
            SET Status = 'A',
                UpdatedBy = @ActionUser,
                UpdatedDate = GETDATE(),
                UpdatedIP = @ActionIP,
                UpdatedPlatform = @ActionPlatform
            WHERE RecommendationHoldId = @RecommendationHoldId
                  AND ClubId = @ClubId
                  AND DisplayPageId = @DisplayPageId
                  AND ISNULL(Status, '') = 'P';

        END;

        COMMIT TRANSACTION @TransactionName;

        SELECT 0 Code,
               'Recommendation request approved successfully' Message;
        RETURN;

    END;

    ELSE IF @Flag = 'gmprrd' --get main page recommendation request for update
    BEGIN
        IF NOT EXISTS
        (
            SELECT 'X'
            FROM dbo.tbl_recommendation_detail_hold a WITH (NOLOCK)
                INNER JOIN dbo.tbl_display_mainpage_hold b WITH (NOLOCK)
                    ON b.RecommendationHoldId = a.RecommendationHoldId
                       AND ISNULL(a.Status, '') <> 'D'
                       AND ISNULL(b.Status, '') <> 'P'
            WHERE a.ClubId = @ClubId
                  AND a.RecommendationHoldId = a.RecommendationHoldId
                  AND b.Sno = @DisplayIdHold
                  AND a.LocationId = @LocationId
                  AND b.DisplayPageId = 3
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
               b.Sno AS DisplayIdHold
        FROM dbo.tbl_recommendation_detail_hold a WITH (NOLOCK)
            INNER JOIN dbo.tbl_display_mainpage_hold b WITH (NOLOCK)
                ON b.RecommendationHoldId = a.RecommendationHoldId
                   AND ISNULL(a.Status, '') <> 'D'
                   AND ISNULL(b.Status, '') <> 'P'
        WHERE a.ClubId = @ClubId
              AND a.RecommendationHoldId = a.RecommendationHoldId
              AND b.Sno = @DisplayIdHold
              AND a.LocationId = @LocationId;
        RETURN;
    END;

    ELSE IF @Flag = 'gmphrd' --get main page host recommendation detail for update
    BEGIN
        SELECT a.RecommendationHoldId,
               b.Sno AS RecommendationHostHoldId,
               b.HostId,
               b.OrderId AS HostDisplayOrderHoldId
        FROM dbo.tbl_recommendation_detail_hold a WITH (NOLOCK)
            INNER JOIN dbo.tbl_host_recommendation_detail_hold b WITH (NOLOCK)
                ON b.RecommendationHoldId = a.RecommendationHoldId
                   AND ISNULL(a.Status, '') <> 'D'
                   AND ISNULL(b.Status, '') <> 'P'
        WHERE a.ClubId = @ClubId
              AND a.LocationId = @LocationId
              AND a.RecommendationHoldId = @RecommendationHoldId
              AND b.DisplayPageId = 3;
        RETURN;
    END;

    ELSE IF @Flag = 'amprd' -- approve main page recommendation details
    BEGIN
        IF NOT EXISTS
        (
            SELECT 'X'
            FROM dbo.tbl_recommendation_detail_hold a WITH (NOLOCK)
                INNER JOIN dbo.tbl_display_mainpage_hold b WITH (NOLOCK)
                    ON b.RecommendationHoldId = a.RecommendationHoldId
                       AND ISNULL(a.Status, '') <> 'D'
                       AND ISNULL(b.Status, '') = 'P'
                       AND b.DisplayPageId = 3
            WHERE a.RecommendationHoldId = @RecommendationHoldId
                  AND a.ClubId = @ClubId
                  AND b.Sno = @DisplayIdHold
        )
        BEGIN
            SELECT 1 Code,
                   'Invalid request' Message;
            RETURN;
        END;

        --SELECT @DisplayPageId = b.DisplayPageId,
        --             @LocationId = a.LocationId
        --      FROM dbo.tbl_recommendation_detail_hold a WITH (NOLOCK)
        --          INNER JOIN dbo.tbl_display_mainpage_hold b WITH (NOLOCK)
        --              ON b.RecommendationHoldId = a.RecommendationHoldId
        --                 AND ISNULL(a.Status, '') <> 'D'
        --                 AND ISNULL(b.Status, '') = 'P'
        --                 AND b.DisplayPageId IN ( 1, 2 )
        --      WHERE a.RecommendationHoldId = @RecommendationHoldId
        --            AND a.ClubId = @ClubId
        --            AND b.Sno = @DisplayIdHold
        --            AND a.LocationId = @LocationId;

        --      IF
        --      (
        --          SELECT COUNT(b.Sno)
        --          FROM dbo.tbl_recommendation_detail a WITH (NOLOCK)
        --              INNER JOIN dbo.tbl_display_mainpage_hold b WITH (NOLOCK)
        --                  ON b.RecommendationId = a.RecommendationId
        --                     AND a.LocationId = @LocationId
        --                     AND b.DisplayPageId = 3
        --                     AND ISNULL(b.Status, '') = 'A'
        --      ) >= 10
        --      BEGIN
        --          SELECT 1 Code,
        --                 'Page already consist max number of club for recommendation' Message;
        --          RETURN;
        --      END;

        --      IF
        --      (
        --          SELECT COUNT(b.Sno)
        --          FROM dbo.tbl_recommendation_detail a WITH (NOLOCK)
        --              INNER JOIN dbo.tbl_host_recommendation_detail b WITH (NOLOCK)
        --                  ON b.RecommendationId = a.RecommendationId
        --                     AND a.LocationId = @LocationId
        --                     AND b.DisplayPageId = 3
        --                     AND ISNULL(b.Status, '') = 'A'
        --      ) >= 10
        --      BEGIN
        --          SELECT 1 Code,
        --                 'Page already consist max number of host for recommendation' Message;
        --          RETURN;
        --      END;

        SET @TransactionName = 'Transaction_amprd';

        BEGIN TRANSACTION @TransactionName;

        SELECT @RecommendationId = a.RecommendationId
        FROM dbo.tbl_recommendation_detail a WITH (NOLOCK)
        WHERE a.ClubId = @ClubId
              AND a.LocationId = @LocationId
              AND ISNULL(a.Status, '') <> 'D';

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
            SELECT a.LocationId,
                   a.ClubId,
                   'A',
                   @ActionUser,
                   GETDATE(),
                   @ActionIP,
                   @ActionPlatform
            FROM dbo.tbl_recommendation_detail_hold a WITH (NOLOCK)
            WHERE a.RecommendationHoldId = @RecommendationHoldId
                  AND a.ClubId = @ClubId
                  AND a.LocationId = @LocationId
                  AND ISNULL(a.Status, '') = 'P';

            SET @RecommendationId = SCOPE_IDENTITY();
        END;

        INSERT INTO dbo.tbl_display_mainpage
        (
            RecommendationId,
            DisplayPageId,
            Status,
            CreatedBy,
            CreatedDate,
            CreatedIP,
            CreatedPlatform,
            GroupId
        )
        SELECT @RecommendationId,
               a.DisplayPageId,
               'A',
               @ActionUser,
               GETDATE(),
               @ActionIP,
               @ActionPlatform,
               @GroupId
        FROM dbo.tbl_display_mainpage_hold a WITH (NOLOCK)
            INNER JOIN dbo.tbl_recommendation_detail_hold b WITH (NOLOCK)
                ON b.RecommendationHoldId = a.RecommendationHoldId
                   AND ISNULL(b.Status, '') <> 'D'
                   AND ISNULL(a.Status, '') = 'P'
                   AND a.DisplayPageId = 3
        WHERE b.RecommendationHoldId = @RecommendationHoldId
              AND b.ClubId = @ClubId
              AND a.Sno = @DisplayIdHold;

        SET @Sno = SCOPE_IDENTITY();

        UPDATE dbo.tbl_display_mainpage_hold
        SET Status = 'A',
            UpdatedBy = @ActionUser,
            UpdatedDate = GETDATE(),
            UpdatedIP = @ActionIP,
            UpdatedPlatform = @ActionPlatform
        FROM dbo.tbl_display_mainpage_hold a WITH (NOLOCK)
            INNER JOIN dbo.tbl_recommendation_detail_hold b WITH (NOLOCK)
                ON b.RecommendationHoldId = a.RecommendationHoldId
                   AND ISNULL(b.Status, '') <> 'D'
                   AND ISNULL(a.Status, '') = 'P'
                   AND a.DisplayPageId = 3
        WHERE b.RecommendationHoldId = @RecommendationHoldId
              AND b.ClubId = @ClubId
              AND a.Sno = @DisplayIdHold;

        IF EXISTS
        (
            SELECT 'X'
            FROM dbo.tbl_host_recommendation_detail_hold a WITH (NOLOCK)
            WHERE a.RecommendationHoldId = @RecommendationHoldId
                  AND a.ClubId = @ClubId
                  AND a.DisplayPageId = 3
                  AND ISNULL(a.Status, '') = 'P'
        )
        BEGIN
            IF NOT EXISTS
            (
                SELECT 'X'
                FROM dbo.tbl_host_recommendation_detail_hold a WITH (NOLOCK)
                    INNER JOIN dbo.tbl_club_details b WITH (NOLOCK)
                        ON b.AgentId = a.ClubId
                           AND b.Status = 'A'
                           AND ISNULL(a.Status, '') = 'P'
                    INNER JOIN dbo.tbl_host_details c WITH (NOLOCK)
                        ON c.AgentId = b.AgentId
                           AND c.HostId = a.HostId
                           AND ISNULL(c.Status, '') = 'A'
                WHERE a.RecommendationHoldId = @RecommendationHoldId
                      AND a.ClubId = @ClubId
                      AND a.DisplayPageId = 3
                      AND ISNULL(a.Status, '') = 'P'
            )
            BEGIN
                SELECT 1 Code,
                       'Invalid host details' Message;
                ROLLBACK TRANSACTION @TransactionName;
                RETURN;
            END;

            IF EXISTS
            (
                SELECT 'X'
                FROM dbo.tbl_host_recommendation_detail_hold a WITH (NOLOCK)
                WHERE a.RecommendationHoldId = @RecommendationHoldId
                      AND a.Status = 'P'
                      AND a.ClubId = @ClubId
                      AND a.DisplayPageId = 3
                      AND a.HostId IN
                          (
                              SELECT a2.HostId
                              FROM dbo.tbl_host_recommendation_detail a2 WITH (NOLOCK)
                              WHERE a2.Status = 'A'
                                    AND a2.ClubId = @ClubId
                                    AND a2.DisplayPageId = a.DisplayPageId
                          )
            )
            BEGIN
                SELECT 1 Code,
                       'Host details already exist' Message;
                ROLLBACK TRANSACTION @TransactionName;
                RETURN;
            END;

            INSERT INTO dbo.tbl_host_recommendation_detail
            (
                RecommendationId,
                ClubId,
                DisplayPageId,
                HostId,
                OrderId,
                Status,
                CreatedBy,
                CreatedDate,
                CreatedIP,
                CreatedPlatform
            )
            SELECT @RecommendationId,
                   b.AgentId,
                   a.DisplayPageId,
                   a.HostId,
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
                       AND ISNULL(a.Status, '') = 'P'
                INNER JOIN dbo.tbl_host_details c WITH (NOLOCK)
                    ON c.AgentId = b.AgentId
                       AND c.HostId = a.HostId
                       AND ISNULL(c.Status, '') = 'A'
            WHERE a.RecommendationHoldId = @RecommendationHoldId
                  AND a.ClubId = @ClubId
                  AND a.DisplayPageId = 3
                  AND ISNULL(a.Status, '') = 'P';

            UPDATE dbo.tbl_host_recommendation_detail
            SET RecommendationHostId = Sno
            WHERE RecommendationId = RecommendationId
                  AND DisplayPageId = 3
                  AND ClubId = @ClubId
                  AND Status = 'A';

            UPDATE dbo.tbl_host_recommendation_detail_hold
            SET Status = 'A',
                UpdatedBy = @ActionUser,
                UpdatedDate = GETDATE(),
                UpdatedIP = @ActionIP,
                UpdatedPlatform = @ActionPlatform
            WHERE RecommendationHoldId = @RecommendationHoldId
                  AND ClubId = @ClubId
                  AND DisplayPageId = 3
                  AND ISNULL(Status, '') = 'P';

        END;

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
    (@ErrorDesc, 'sproc_admin_accept_recommendation_request_Management(Flag: ' + ISNULL(@Flag, '') + ')', 'SQL', 'SQL',
     'sproc_admin_accept_recommendation_request_Management', GETDATE());

    SELECT 1 Code,
           'ErrorId:' + CAST(SCOPE_IDENTITY() AS VARCHAR) Message;
    RETURN;
END CATCH;
GO


