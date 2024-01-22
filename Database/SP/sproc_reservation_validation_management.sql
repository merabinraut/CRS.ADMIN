USE [CRS];
GO

/****** Object:  StoredProcedure [dbo].[sproc_reservation_validation_management]    Script Date: 15/12/2023 15:50:38 ******/
SET ANSI_NULLS ON;
GO

SET QUOTED_IDENTIFIER ON;
GO








ALTER PROC [dbo].[sproc_reservation_validation_management]
    @Flag VARCHAR(10),
    @OTPCode VARCHAR(10) = NULL,
    @ReservationId VARCHAR(10) = NULL,
    @ActionUser VARCHAR(200) = NULL,
    @ActionIP VARCHAR(50) = NULL,
    @ActionPlatform VARCHAR(20) = NULL
AS
DECLARE @ReviewRedirectURL VARCHAR(MAX) = 'http://hoslog.jp/ReviewManagement/Review/';
DECLARE @Sno BIGINT,
        @Sno2 BIGINT,
        @StringSQL VARCHAR(MAX),
        @StringSQL2 VARCHAR(MAX),
        @TransactionName VARCHAR(200),
        @ErrorDesc VARCHAR(MAX),
        @CustomerId VARCHAR(10),
        @SmsEmailResponseCode INT = 1;
BEGIN TRY
    IF ISNULL(@Flag, '') = 'grdvo' --get reservation details via otp
    BEGIN
        BEGIN
            IF NOT EXISTS
            (
                SELECT 'X'
                FROM dbo.tbl_customer_reservation_otp a WITH (NOLOCK)
                    INNER JOIN dbo.tbl_reservation_detail b WITH (NOLOCK)
                        ON b.ReservationId = a.ReservationId
                           AND ISNULL(b.TransactionStatus, '') NOT IN ( 'D' )
                    INNER JOIN dbo.tbl_club_details c WITH (NOLOCK)
                        ON c.AgentId = b.ClubId
                    INNER JOIN dbo.tbl_users d WITH (NOLOCK)
                        ON d.RoleId = 5
                           AND d.AgentId = c.AgentId
                           AND d.LoginId = @ActionUser
                           AND ISNULL(d.[Status], '') = 'A'
                WHERE a.OTPCode = @OTPCode
                      AND ISNULL(b.OTPVerificationStatus, '') NOT IN ( 'D' )
            )
            BEGIN
                SELECT 1 Code,
                       'Invalid request' Message;
                RETURN;
            END;
        END;

        IF NOT EXISTS
        (
            SELECT 'X'
            FROM dbo.tbl_customer_reservation_otp a WITH (NOLOCK)
                INNER JOIN dbo.tbl_reservation_detail b WITH (NOLOCK)
                    ON b.ReservationId = a.ReservationId
                       AND ISNULL(b.TransactionStatus, '') NOT IN ( 'D' )
                INNER JOIN dbo.tbl_sms_sent c WITH (NOLOCK)
                    ON c.Sno = a.SMSId
                       AND c.SMSType = 'Res-OTP'
                       AND ISNULL(c.[Status], '') = 'S'
                INNER JOIN dbo.tbl_reservation_plan_detail d WITH (NOLOCK)
                    ON d.ReservationId = b.ReservationId
                       AND d.PlanDetailId = b.PlanDetailId
                INNER JOIN dbo.tbl_static_data e WITH (NOLOCK)
                    ON e.StaticDataType = 10
                       AND e.StaticDataValue = b.PaymentType
                INNER JOIN dbo.tbl_club_details f WITH (NOLOCK)
                    ON f.AgentId = b.ClubId
                INNER JOIN dbo.tbl_location g WITH (NOLOCK)
                    ON g.LocationId = f.LocationId
                INNER JOIN dbo.tbl_tag_detail h WITH (NOLOCK)
                    ON h.ClubId = f.AgentId
            WHERE a.OTPCode = @OTPCode
                  AND ISNULL(a.[Status], '') = ('D')
                  AND ISNULL(b.OTPVerificationStatus, '') NOT IN ( 'D' )
                  AND a.ReservationId = ISNULL(@ReservationId, a.ReservationId)
        )
        BEGIN
            SELECT 1 Code,
                   'Invalid OTP details' Message;
            RETURN;
        END;

        IF @ReservationId IS NOT NULL
        BEGIN
            IF NOT EXISTS
            (
                SELECT 'X'
                FROM dbo.tbl_reservation_detail a WITH (NOLOCK)
                WHERE a.ReservationId = @ReservationId
            )
            BEGIN
                SELECT 1 Code,
                       'Invalid reservation details' Message;
                RETURN;
            END;
        END;

        SELECT 0 Code,
               'Success' Message,
               b.ReservationId,
               b.InvoiceId,
               b.LocationVerificationStatus,
               CASE
                   WHEN ISNULL(b.OTPVerificationStatus, '') = 'P' THEN
                       'Pending'
                   WHEN ISNULL(b.OTPVerificationStatus, '') = 'A' THEN
                       'Approved'
                   WHEN ISNULL(b.OTPVerificationStatus, '') = 'R' THEN
                       'Rejected'
                   ELSE
                       ''
               END AS OTPVerificationStatus,
               e.StaticDataLabel AS PaymentType,
               b.ReservedDate,
               CONVERT(
                          DATETIME,
                          CONCAT(YEAR(GETDATE()), '-', CAST(CONCAT(b.VisitDate, ' ', b.VisitTime) AS VARCHAR)),
                          120
                      ) AS VisitDateTime,
               CASE
                   WHEN ISNULL(b.TransactionStatus, '') = 'P' THEN
                       'Pending'
                   WHEN ISNULL(b.TransactionStatus, '') = 'S' THEN
                       'Success'
                   WHEN ISNULL(b.TransactionStatus, '') = 'F' THEN
                       'Failed'
                   WHEN ISNULL(b.TransactionStatus, '') = 'A' THEN
                       'Approved'
                   ELSE
                       ''
               END AS TransactionStatus,
               g.LocationName,
               h.Tag5StoreName AS StoreName,
               d.PlanName
        FROM dbo.tbl_customer_reservation_otp a WITH (NOLOCK)
            INNER JOIN dbo.tbl_reservation_detail b WITH (NOLOCK)
                ON b.ReservationId = a.ReservationId
                   AND ISNULL(b.TransactionStatus, '') NOT IN ( 'D' )
            INNER JOIN dbo.tbl_sms_sent c WITH (NOLOCK)
                ON c.Sno = a.SMSId
                   AND c.SMSType = 'Res-OTP'
                   AND ISNULL(c.[Status], '') = 'S'
            INNER JOIN dbo.tbl_reservation_plan_detail d WITH (NOLOCK)
                ON d.ReservationId = b.ReservationId
                   AND d.PlanDetailId = b.PlanDetailId
            INNER JOIN dbo.tbl_static_data e WITH (NOLOCK)
                ON e.StaticDataType = 10
                   AND e.StaticDataValue = b.PaymentType
            INNER JOIN dbo.tbl_club_details f WITH (NOLOCK)
                ON f.AgentId = b.ClubId
            INNER JOIN dbo.tbl_location g WITH (NOLOCK)
                ON g.LocationId = f.LocationId
            INNER JOIN dbo.tbl_tag_detail h WITH (NOLOCK)
                ON h.ClubId = f.AgentId
        WHERE a.OTPCode = @OTPCode
              AND ISNULL(a.[Status], '') NOT IN ( 'D' )
              AND ISNULL(b.OTPVerificationStatus, '') NOT IN ( 'D' )
              AND a.ReservationId = ISNULL(@ReservationId, a.ReservationId);
        RETURN;
    END;
    ELSE IF ISNULL(@Flag, '') = 'ghvr' --get host via reservation details
    BEGIN
        SELECT c.HostId,
               c.HostName,
               '' AS HostLogo
        FROM dbo.tbl_reservation_detail a WITH (NOLOCK)
            INNER JOIN dbo.tbl_reservation_host_detail b WITH (NOLOCK)
                ON b.ReservationId = a.ReservationId
            INNER JOIN dbo.tbl_host_details c WITH (NOLOCK)
                ON c.HostId = b.HostId
                   AND c.AgentId = a.ClubId
        WHERE a.ReservationId = @ReservationId;

        RETURN;
    END;
    ELSE IF ISNULL(@Flag, '') = 'mros' --manage reservation otp status
    BEGIN
        BEGIN
            IF NOT EXISTS
            (
                SELECT 'X'
                FROM dbo.tbl_customer_reservation_otp a WITH (NOLOCK)
                    INNER JOIN dbo.tbl_reservation_detail b WITH (NOLOCK)
                        ON b.ReservationId = a.ReservationId
                           AND ISNULL(b.TransactionStatus, '') NOT IN ( 'D' )
                    INNER JOIN dbo.tbl_club_details c WITH (NOLOCK)
                        ON c.AgentId = b.ClubId
                    INNER JOIN dbo.tbl_users d WITH (NOLOCK)
                        ON d.RoleId = 5
                           AND d.AgentId = c.AgentId
                           AND d.LoginId = @ActionUser
                           AND ISNULL(d.[Status], '') = 'A'
                WHERE a.OTPCode = @OTPCode
                      AND ISNULL(b.OTPVerificationStatus, '') = 'P'
            )
            BEGIN
                SELECT 1 Code,
                       'Invalid privilege' Message;
                RETURN;
            END;
        END;

        IF NOT EXISTS
        (
            SELECT 'X'
            FROM dbo.tbl_customer_reservation_otp a WITH (NOLOCK)
                INNER JOIN dbo.tbl_reservation_detail b WITH (NOLOCK)
                    ON b.ReservationId = a.ReservationId
                       AND ISNULL(b.TransactionStatus, '') = 'P'
                INNER JOIN dbo.tbl_sms_sent c WITH (NOLOCK)
                    ON c.Sno = a.SMSId
                       AND ISNULL(c.[Status], '') = 'S'
                       AND c.SMSType = 'Res-OTP'
            WHERE b.ReservationId = @ReservationId
                  AND a.OTPCode = @OTPCode
                  AND ISNULL(a.[Status], '') = 'P'
                  AND ISNULL(b.OTPVerificationStatus, '') = 'P'
        )
        BEGIN
            SELECT 1 Code,
                   'Invalid OTP details' Message;
            RETURN;
        END;

        SET @TransactionName = 'Transaction_mros';

        BEGIN TRANSACTION @TransactionName;

        UPDATE dbo.tbl_customer_reservation_otp
        SET [Status] = 'A',
            UpdatedBy = @ActionUser,
            UpdatedDate = GETDATE(),
            UpdatedIP = @ActionIP,
            UpdatedPlatform = @ActionPlatform
        WHERE ReservationId = @ReservationId
              AND OTPCode = @OTPCode
              AND ISNULL([Status], '') = 'P';

        UPDATE dbo.tbl_reservation_detail
        SET OTPVerificationStatus = 'A',
            TransactionStatus = 'A',
            ActionUser = @ActionUser,
            ActionDate = GETDATE(),
            ActionIP = @ActionIP,
            ActionPlatform = @ActionPlatform
        WHERE ReservationId = @ReservationId
              AND ISNULL([TransactionStatus], '') = 'P'
              AND ISNULL(OTPVerificationStatus, '') = 'P';

        --SELECT @ReviewRedirectURL
        --    = 'http://hoslog.jp/ReviewManagement/Review?CustomerId=' + CAST(a.CustomerId AS VARCHAR)
        --      + '&&ReservationId=' + CAST(a.ReservationId AS VARCHAR)
        --FROM dbo.tbl_reservation_detail a WITH (NOLOCK)
        --WHERE a.ReservationId = @ReservationId
        --      AND ISNULL(a.[TransactionStatus], '') = 'A';

        --PRINT (@ReviewRedirectURL);

        --     INSERT INTO dbo.tbl_customer_notification
        --     (
        --         ToAgentId,
        --         NotificationType,
        --         NotificationSubject,
        --         NotificationBody,
        --         NotificationStatus,
        --         NotificationReadStatus,
        --         CreatedBy,
        --         CreatedDate,
        --NotificationURL
        --     )
        --     SELECT a.CustomerId,
        --            'Review',
        --            'Club review',
        --            'Please review',
        --            'A',
        --            'P',
        --            @ActionUser,
        --            GETDATE(),
        --   @ReviewRedirectURL
        --     FROM dbo.tbl_reservation_detail a WITH (NOLOCK)
        --     WHERE a.ReservationId = @ReservationId
        --           AND ISNULL(a.[TransactionStatus], '') = 'A';

        --     SET @Sno = SCOPE_IDENTITY();

        --     UPDATE tbl_customer_notification
        --     SET notificationId = @Sno
        --     WHERE Sno = @Sno;

        COMMIT TRANSACTION @TransactionName;

        SELECT @CustomerId = a.CustomerId
        FROM dbo.tbl_reservation_detail a WITH (NOLOCK)
        WHERE a.ReservationId = @ReservationId
              AND ISNULL(a.[TransactionStatus], '') = 'A';

        EXEC dbo.sproc_email_sms_management @Flag = '5',
                                            @AgentId = @CustomerId,
                                            @UserId = @CustomerId,
                                            @ActionUser = @ActionUser,
                                            @ActionIP = @ActionIP,
                                            @ActionPlatform = @ActionPlatform,
                                            @ExtraDatailId1 = @ReservationId,
                                            @ResponseCode = @SmsEmailResponseCode OUTPUT;

        SELECT 0 Code,
               'OTP verified successfully' Message;
        RETURN;
    END;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0
        ROLLBACK TRANSACTION @TransactionName;

    SET @ErrorDesc = 'SQL error found: (' + ERROR_MESSAGE() + ')' + ' at ' + CAST(ERROR_LINE() AS VARCHAR);

    INSERT INTO dbo.tbl_error_log
    (
        ErrorDesc,
        ErrorScript,
        QueryString,
        ErrorCategory,
        ErrorSource,
        ActionDate
    )
    VALUES
    (@ErrorDesc, 'sproc_reservation_validation_management(Flag: ' + ISNULL(@Flag, '') + ')', 'SQL', 'SQL',
     'sproc_reservation_validation_management', GETDATE());

    SELECT 1 Code,
           'ErrorId:' + CAST(SCOPE_IDENTITY() AS VARCHAR) Message;
    RETURN;
END CATCH;
GO


