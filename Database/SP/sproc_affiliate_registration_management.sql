ALTER PROC [dbo].[sproc_affiliate_registration_management]
    @Flag VARCHAR(10),
    @MobileNumber VARCHAR(15) = NULL,
    @EmailAddress VARCHAR(500) = NULL,
    @FirstName NVARCHAR(150) = NULL,
    @LastName NVARCHAR(150) = NULL,
    @ActionUser VARCHAR(200) = NULL,
    @ActionIP VARCHAR(200) = NULL,
    @ActionPlatform VARCHAR(20) = NULL,
    @DOB VARCHAR(10) = NULL,
    @Gender VARCHAR(10) = NULL,
    @SnsType VARCHAR(10) = NULL,
    @SnsURLLink VARCHAR(MAX) = NULL,
    @VerificationCode VARCHAR(6) = NULL,
    @HoldAgentId VARCHAR(10) = NULL
AS
DECLARE @Sno BIGINT,
        @Sno2 BIGINT,
        @TransactionName VARCHAR(200),
        @ErrorDesc VARCHAR(MAX);
BEGIN TRY
    IF @Flag = 'hra' -- hold register affiliate
    BEGIN
        IF EXISTS
        (
            SELECT 'X'
            FROM dbo.tbl_customer a WITH (NOLOCK)
                INNER JOIN dbo.tbl_users b WITH (NOLOCK)
                    ON b.AgentId = a.AgentId
                       AND b.RoleType = '3'
                       AND ISNULL(b.Status, '') <> 'D'
            WHERE a.MobileNumber = @MobileNumber
            UNION
            SELECT 'X'
            FROM dbo.tbl_admin a WITH (NOLOCK)
            WHERE a.MobileNumber = @MobileNumber
                  AND ISNULL(a.Status, '') <> 'D'
            UNION
            SELECT 'X'
            FROM dbo.tbl_club_details a WITH (NOLOCK)
                INNER JOIN dbo.tbl_users b WITH (NOLOCK)
                    ON b.AgentId = a.AgentId
                       AND b.RoleType = '4'
                       AND ISNULL(b.Status, '') <> 'D'
            WHERE a.MobileNumber = @MobileNumber
        )
        BEGIN
            SELECT 1 Code,
                   'Duplicate mobile number' Message;
            RETURN;
        END;

        IF EXISTS
        (
            SELECT 'X'
            FROM dbo.tbl_customer a WITH (NOLOCK)
                INNER JOIN dbo.tbl_users b WITH (NOLOCK)
                    ON b.AgentId = a.AgentId
                       AND b.RoleType = '3'
                       AND ISNULL(b.Status, '') <> 'D'
            WHERE a.EmailAddress = @EmailAddress
            UNION
            SELECT 'X'
            FROM dbo.tbl_admin a WITH (NOLOCK)
            WHERE a.EmailAddress = @EmailAddress
                  AND ISNULL(a.Status, '') <> 'D'
            UNION
            SELECT 'X'
            FROM dbo.tbl_club_details a WITH (NOLOCK)
                INNER JOIN dbo.tbl_users b WITH (NOLOCK)
                    ON b.AgentId = a.AgentId
                       AND b.RoleType = '4'
                       AND ISNULL(b.Status, '') <> 'D'
            WHERE a.Email = @EmailAddress
        )
        BEGIN
            SELECT 1 Code,
                   'Duplicate email address' Message;
            RETURN;
        END;

        SET @TransactionName = 'Flag_hra';
        BEGIN TRANSACTION @TransactionName;

        INSERT INTO dbo.tbl_affiliate_hold
        (
            FirstName,
            LastName,
            MobileNumber,
            EmailAddress,
            DOB,
            Gender,
            SnsType,
            SnsURLLink,
            Status,
            CreatedBy,
            CreatedIP,
            CreatedPlatform,
            CreatedDate
        )
        VALUES
        (@FirstName, @LastName, @MobileNumber, @EmailAddress, @DOB, @Gender, 'I', @SnsType, @SnsURLLink, @ActionUser,
         @ActionIP, @ActionPlatform, GETDATE());

        SELECT @Sno = SCOPE_IDENTITY(),
               @VerificationCode = '111111'; --dbo.func_generate_otp_code(6);

        UPDATE dbo.tbl_affiliate_hold
        SET HoldAgentId = @Sno
        WHERE Sno = @Sno;

        INSERT INTO dbo.tbl_verification_sent
        (
            RoleId,
            AgentId,
            UserId,
            MobileNumber,
            FullName,
            VerificationCode,
            Status,
            CreatedBy,
            CreatedDate,
            CreatedIP,
            CreatedPlatform
        )
        VALUES
        (   3, @Sno, @Sno, @MobileNumber, CONCAT(@FirstName, ' ', @LastName), @VerificationCode, 'S', --sent
            @ActionUser, GETDATE(), @ActionIP, @ActionPlatform);

        INSERT INTO dbo.tbl_sms_sent
        (
            RoleId,
            AgentId,
            UserId,
            DestinationNumber,
            Message,
            Status,
            CreatedBy,
            CreatedDate,
            CreatedIP,
            CreatedPlatform
        )
        VALUES
        (   3,                                                           -- RoleId - bigint
            @Sno,                                                        -- AgentId - bigint
            @Sno,                                                        -- UserId - bigint
            @MobileNumber,                                               -- DestinationNumber - varchar(15)
            'Dear ' + CAST(ISNULL(CONCAT(@FirstName, ' ', @LastName), '') AS VARCHAR) + ', your verification code is'
            + CAST(ISNULL(@VerificationCode, '') AS VARCHAR)
            + '. Use this code to complete the registration. - Hostlog', -- Message - varchar(max)
            'P',                                                         -- Status - char(1)		   
            @ActionUser,                                                 -- CreatedBy - varchar(200)
            GETDATE(),                                                   -- CreatedDate - datetime
            @ActionIP,                                                   -- CreatedIP - varchar(50)
            @ActionPlatform                                              -- CreatedPlatform - varchar(20)
            );

        SELECT 0 Code,
               'Registration initiation successfull' Message,
               @Sno AS Extra1,
               DATEADD(MINUTE, 2, GETDATE()) AS Extra2;

        COMMIT TRANSACTION @TransactionName;
    END;

    ELSE IF @Flag = 'vrotp' --verify registration otp
    BEGIN
        IF NOT EXISTS
        (
            SELECT 'X'
            FROM dbo.tbl_affiliate_hold a WITH (NOLOCK)
            WHERE a.MobileNumber = @MobileNumber
                  AND a.HoldAgentId = @HoldAgentId
                  AND ISNULL(a.Status, '') = 'I'
        )
        BEGIN
            SELECT 1 Code,
                   'Invalid request' Message;
        END;

        IF NOT EXISTS
        (
            SELECT 'X'
            FROM dbo.tbl_verification_sent a WITH (NOLOCK)
            WHERE a.MobileNumber = @MobileNumber
                  AND a.AgentId = @HoldAgentId
                  AND a.UserId = @HoldAgentId
                  AND a.VerificationCode = @VerificationCode
                  AND a.RoleId = 3
                  AND ISNULL(a.Status, '') = 'S'
        )
        BEGIN
            SELECT 1 Code,
                   'Invalid OTP request' Message;
            RETURN;
        END;

        IF NOT EXISTS
        (
            SELECT 'X'
            FROM dbo.tbl_verification_sent a WITH (NOLOCK)
            WHERE a.MobileNumber = @MobileNumber
                  AND a.AgentId = @HoldAgentId
                  AND a.UserId = @HoldAgentId
                  AND a.VerificationCode = @VerificationCode
                  --AND (DATEDIFF(SECOND, a.SentDate, GETDATE()) > 120)
                  AND a.RoleId = 3
                  AND ISNULL(a.Status, '') = 'S'
        )
        BEGIN
            UPDATE dbo.tbl_verification_sent
            SET Status = 'E',
                UpdatedBy = @ActionUser,
                UpdatedDate = GETDATE(),
                UpdatedIP = @ActionIP,
                UpdatedPlatform = @ActionPlatform
            WHERE MobileNumber = @MobileNumber
                  AND AgentId = @HoldAgentId
                  AND UserId = @HoldAgentId
                  AND VerificationCode = @VerificationCode
                  AND RoleId = 3
                  AND ISNULL(Status, '') = 'S';

            SELECT 1 Code,
                   'OTP code has expired' Message;
            RETURN;
        END;

        IF EXISTS
        (
            SELECT 'X'
            FROM dbo.tbl_customer a WITH (NOLOCK)
                INNER JOIN dbo.tbl_users b WITH (NOLOCK)
                    ON b.AgentId = a.AgentId
                       AND b.RoleType = '3'
                       AND ISNULL(b.Status, '') <> 'D'
            WHERE a.MobileNumber = @MobileNumber
            UNION
            SELECT 'X'
            FROM dbo.tbl_admin a WITH (NOLOCK)
            WHERE a.MobileNumber = @MobileNumber
                  AND ISNULL(a.Status, '') <> 'D'
            UNION
            SELECT 'X'
            FROM dbo.tbl_club_details a WITH (NOLOCK)
                INNER JOIN dbo.tbl_users b WITH (NOLOCK)
                    ON b.AgentId = a.AgentId
                       AND b.RoleType = '4'
                       AND ISNULL(b.Status, '') <> 'D'
            WHERE a.MobileNumber = @MobileNumber
        )
        BEGIN
            SELECT 1 Code,
                   'Duplicate mobile number' Message;
            RETURN;
        END;

        IF EXISTS
        (
            SELECT 'X'
            FROM dbo.tbl_customer a WITH (NOLOCK)
                INNER JOIN dbo.tbl_users b WITH (NOLOCK)
                    ON b.AgentId = a.AgentId
                       AND b.RoleType = '3'
                       AND ISNULL(b.Status, '') <> 'D'
            WHERE a.EmailAddress = @EmailAddress
            UNION
            SELECT 'X'
            FROM dbo.tbl_admin a WITH (NOLOCK)
            WHERE a.EmailAddress = @EmailAddress
                  AND ISNULL(a.Status, '') <> 'D'
            UNION
            SELECT 'X'
            FROM dbo.tbl_club_details a WITH (NOLOCK)
                INNER JOIN dbo.tbl_users b WITH (NOLOCK)
                    ON b.AgentId = a.AgentId
                       AND b.RoleType = '4'
                       AND ISNULL(b.Status, '') <> 'D'
            WHERE a.Email = @EmailAddress
        )
        BEGIN
            SELECT 1 Code,
                   'Duplicate email address' Message;
            RETURN;
        END;

        SET @TransactionName = 'Flag_vrotp';
        BEGIN TRANSACTION @TransactionName;

        UPDATE dbo.tbl_verification_sent
        SET Status = 'V',
            UpdatedBy = @ActionUser,
            UpdatedDate = GETDATE(),
            UpdatedIP = @ActionIP,
            UpdatedPlatform = @ActionPlatform
        WHERE MobileNumber = @MobileNumber
              AND AgentId = @HoldAgentId
              AND UserId = @HoldAgentId
              AND VerificationCode = @VerificationCode
              AND RoleId = 3
              AND ISNULL(Status, '') = 'S';

        UPDATE dbo.tbl_affiliate_hold
        SET Status = 'P',
            UpdatedBy = @ActionUser,
            UpdatedDate = GETDATE(),
            UpdatedIP = @ActionIP
        FROM dbo.tbl_affiliate_hold a WITH (NOLOCK)
        WHERE a.MobileNumber = @MobileNumber
              AND a.HoldAgentId = @HoldAgentId
              AND ISNULL(a.Status, '') = 'I';
        COMMIT TRANSACTION @TransactionName;

        SELECT 0 Code,
               'Registration Successful. Please wait for admin approvel.';
        RETURN;
    END;

    ELSE IF @Flag = 'rrotp' --resend registration otp
    BEGIN
        IF NOT EXISTS
        (
            SELECT 'X'
            FROM dbo.tbl_affiliate_hold a WITH (NOLOCK)
            WHERE a.MobileNumber = @MobileNumber
                  AND a.HoldAgentId = @HoldAgentId
                  AND ISNULL(a.Status, '') = 'I'
        )
        BEGIN
            SELECT 1 Code,
                   'Invalid request' Message;
        END;

        SELECT @VerificationCode = dbo.func_generate_otp_code(6);

        INSERT INTO dbo.tbl_verification_sent
        (
            RoleId,
            AgentId,
            UserId,
            MobileNumber,
            FullName,
            VerificationCode,
            Status,
            ProcessId,
            CreatedBy,
            CreatedDate,
            CreatedIP,
            CreatedPlatform
        )
        SELECT 3,
               a.AgentId,
               a.UserId,
               a.MobileNumber,
               a.FullName,
               @VerificationCode,
               'S', --sent
               GETDATE(),
               @ActionUser,
               GETDATE(),
               @ActionIP,
               @ActionPlatform
        FROM dbo.tbl_verification_sent a WITH (NOLOCK)
        WHERE a.MobileNumber = @MobileNumber
              AND a.AgentId = @HoldAgentId
              AND a.UserId = @HoldAgentId
              AND ISNULL(a.Status, '') = 'P';

        SET @Sno = SCOPE_IDENTITY();

        UPDATE dbo.tbl_verification_sent
        SET Status = 'E', --Expired
            UpdatedBy = @ActionUser,
            UpdatedDate = GETDATE(),
            UpdatedIP = @ActionIP,
            UpdatedPlatform = @ActionPlatform
        WHERE MobileNumber = @MobileNumber
              AND AgentId = @HoldAgentId
              AND UserId = @HoldAgentId
              AND ISNULL(Status, '') = 'P'
              AND Sno <> @Sno;

        SELECT 0 Code,
               'OTP code sent successfully' Message,
               DATEADD(MINUTE, 2, GETDATE()) AS Extra2;
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
    (@ErrorDesc, 'sproc_affiliate_registration_management(Flag: ' + ISNULL(@Flag, '') + ')', 'SQL', 'SQL',
     'sproc_affiliate_registration_management', GETDATE());

    SELECT 1 Code,
           'ErrorId:' + CAST(SCOPE_IDENTITY() AS VARCHAR) Message;
    RETURN;
END CATCH;
GO


